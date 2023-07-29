using Bamboo.Configuration.Git;
using Bamboo.Logging;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Timers;

namespace Bamboo.Configuration
{
    /// <summary>
    /// config base about datasource from git project 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GitConfigBase<T> : ConfigBase<T> where T : class, new()
    {
        private static ILogger _logger = new BambooLogger<GitConfigBase<T>>();
        private static Timer _timer;

        static GitConfigBase()
        {
            RegisterInitializer(() =>
            {
                Initializer();

                IConfigurationRoot config = null;

                switch (Path.GetExtension(ConfigurationFilePath)?.ToLowerInvariant())
                {
                    case ".json":
                        config = new ConfigurationBuilder().AddJsonFile(ConfigurationFilePath, optional: false, reloadOnChange: true).Build();
                        break;
                    case ".xml":
                        config = new ConfigurationBuilder().AddXmlFile(ConfigurationFilePath, optional: false, reloadOnChange: true).Build();
                        break;
                    case ".ini":
                        config = new ConfigurationBuilder().AddIniFile(ConfigurationFilePath, optional: false, reloadOnChange: true).Build();
                        break;
                    default:
                        throw new NotSupportedException($"the extension of configuration file '{ConfigurationFilePath}' is not support");
                }

                _timer = new Timer(GetGroupSetting().FetchInterval);
                _timer.Elapsed += (s, e) => Initializer();
                _timer.AutoReset = true;
                _timer.Enabled = true;

                return config;
            });
        }

        private static void Initializer()
        {
            //download
            var workSpace = DownloadConfigurationAndGetWorkSpace();

            var configFilePath = ConfigFileAttribute.GetFilePath(typeof(T));

            //find file
            var configurationFullPath = FindFileAndGetFullPath(workSpace, configFilePath);

            InitializeConfigurationFile(configurationFullPath);
        }

        /// <summary>
        /// default group in appsettings config file
        /// </summary>
        private const string DefaultGroup = "Default";
        /// <summary>
        /// Default fit config download directory
        /// </summary>
        private const string GitConfigDefaultDownloadDirectory = "BambooConfigDownload";

        private static string[] SupportedConfigurationExtensions = new[] { ".json", ".xml", ".ini" };

        private static string GitConfigDefaultDownloadDirectoryFullPath = Path.Combine(AppContext.BaseDirectory, GitConfigDefaultDownloadDirectory);

        private static GroupSetting GetGroupSetting(string group = null)
        {
            //get group name
            if (string.IsNullOrEmpty(group))
                group = ConfigGroupAttribute.GetGroup(typeof(T)) ?? DefaultGroup;

            //get config
            var configSetting = AppSettings.GetGroupSetting(group);

            if (string.IsNullOrEmpty(configSetting.RemoteAddress))
                throw new ArgumentNullException(nameof(configSetting.RemoteAddress));
            if (string.IsNullOrEmpty(configSetting.Branch))
                throw new ArgumentNullException(nameof(configSetting.Branch));

            //The pulling frequency should not be too high.
            if (configSetting.FetchInterval < 5)
                configSetting.FetchInterval = 30000;

            return configSetting;
        }

        private static string DownloadConfigurationAndGetWorkSpace()
        {
            //get group name
            string group = ConfigGroupAttribute.GetGroup(typeof(T)) ?? DefaultGroup;

            var configSetting = GetGroupSetting(group);

            var workSpace = Path.Combine(GitConfigDefaultDownloadDirectoryFullPath, group);

            //Ensure directory exist
            if (!Directory.Exists(workSpace))
                Directory.CreateDirectory(workSpace);

            try
            {
                _logger.LogDebug($"git config '{ConfigurationName}' pulling...");

                //download files
                if (!Repository.IsValid(workSpace))
                    Repository.Clone(configSetting.RemoteAddress, workSpace);

                using (var repo = new Repository(workSpace))
                {
                    //fetch --prune
                    repo.Config.Set("fetch.prune", true);
                    Commands.Fetch(repo, "origin", new string[0], null, null);

                    //get branch
                    var branch = repo.Branches.FirstOrDefault(t => t.FriendlyName.Equals(string.Concat("origin/", configSetting.Branch)));

                    if (branch == null)
                        throw new ArgumentOutOfRangeException(nameof(branch), $"the branch '{configSetting.Branch}'in configuration is not found");

                    //checkout
                    Commands.Checkout(repo, branch);

                    //Commands.Pull(repo, repo.Config.BuildSignature(DateTimeOffset.Now), new PullOptions { });
                }

                _logger.LogDebug($"git config '{ConfigurationName}' pulling success.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"git config '{ConfigurationName}' pull error");
                throw;
            }

            return workSpace;
        }

        private static string FindFileAndGetFullPath(string workSpace, string configName)
        {
            //if config name contains extension
            var fileExtension = Path.GetExtension(configName);

            if (string.IsNullOrEmpty(fileExtension) || !SupportedConfigurationExtensions.Contains(fileExtension?.ToLowerInvariant()))
                throw new NotSupportedException($"The file extension '{fileExtension}' is not supported");

            //if config name not contains extension
            var foundFiles = Directory.GetFiles(workSpace, configName, SearchOption.AllDirectories);

            if (!foundFiles.Any())
                throw new FileNotFoundException($"Configuration file not found with config name '{configName}'.");

            //configuration file more than one
            if (foundFiles.Length > 1)
                throw new FileNotFoundException($"More than one configuration file was found. The application does not know which one to take. configuration files:[{string.Join(",", foundFiles)}].");

            return foundFiles[0];
        }
    }
}
