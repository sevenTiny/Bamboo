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
                //get group name
                string group = ConfigGroupAttribute.GetGroup(typeof(T)) ?? DefaultGroup;

                var groupSetting = GetGroupSetting(group);

                var isLocalMode = AppSettingsConfig.IsLocalMode();

                Initializer(isLocalMode, groupSetting);

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

                //if local mode, not start fetch remote schedule
                if (!isLocalMode)
                {
                    _timer = new Timer(groupSetting.FetchInterval);
                    _timer.Elapsed += (s, e) => Initializer(isLocalMode, groupSetting);
                    _timer.AutoReset = true;
                    _timer.Enabled = true;
                }

                return config;
            });
        }

        private static void Initializer(bool isLocalMode, GroupSetting groupSetting)
        {
            var configFilePath = ConfigFileAttribute.GetFilePath(typeof(T));
            string configurationFullPath;

            //pull git repository
            if (!isLocalMode && DownloadConfigurationAndGetWorkSpace(groupSetting, out string workSpace))
            {
                //find file
                configurationFullPath = FindFileAndGetFullPath(workSpace, configFilePath);
            }
            else
            {
                configurationFullPath = Path.Combine("BambooConfig", configFilePath);
            }

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
            var configSetting = AppSettingsConfigurationHelper.GetGroupSetting(group);

            if (string.IsNullOrEmpty(configSetting.RemoteAddress))
                throw new ArgumentNullException(nameof(configSetting.RemoteAddress));
            if (string.IsNullOrEmpty(configSetting.Branch))
                throw new ArgumentNullException(nameof(configSetting.Branch));

            //The pulling frequency should not be too high.
            if (configSetting.FetchInterval < 5)
                configSetting.FetchInterval = 30000;

            return configSetting;
        }

        private static bool DownloadConfigurationAndGetWorkSpace(GroupSetting groupSetting, out string workSpace)
        {
            //get group name
            string group = ConfigGroupAttribute.GetGroup(typeof(T)) ?? DefaultGroup;

            workSpace = Path.Combine(GitConfigDefaultDownloadDirectoryFullPath, group);

            //Ensure directory exist
            if (!Directory.Exists(workSpace))
                Directory.CreateDirectory(workSpace);

            try
            {
                _logger.LogDebug($"git config '{ConfigurationName}' pulling...");

                //download files
                if (!Repository.IsValid(workSpace))
                    Repository.Clone(groupSetting.RemoteAddress, workSpace);

                using (var repo = new Repository(workSpace))
                {
                    //fetch --prune
                    repo.Config.Set("fetch.prune", true);
                    Commands.Fetch(repo, "origin", new string[0], null, null);

                    //get branch
                    var branch = repo.Branches.FirstOrDefault(t => t.FriendlyName.Equals(string.Concat("origin/", groupSetting.Branch)));

                    if (branch == null)
                        throw new ArgumentOutOfRangeException(nameof(branch), $"the branch '{groupSetting.Branch}'in configuration is not found");

                    //checkout
                    Commands.Checkout(repo, branch);

                    //Commands.Pull(repo, repo.Config.BuildSignature(DateTimeOffset.Now), new PullOptions { });
                }

                _logger.LogDebug($"git config '{ConfigurationName}' pulling success.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"git config '{ConfigurationName}' pull error");
                return false;
            }

            return true;
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

        protected override string SerializeConfigurationInstance()
        {
            throw new NotImplementedException();
        }
    }
}
