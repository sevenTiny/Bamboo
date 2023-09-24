using Bamboo.Configuration.Helpers;
using Bamboo.Logging;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace Bamboo.Configuration
{
    /// <summary>
    /// config base about datasource from git project 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GitConfigBase<T> : ConfigBase<T> where T : class, new()
    {
        private static ILogger _logger = new BambooLogger<GitConfigBase<T>>();

        static GitConfigBase()
        {
            RegisterInitializer(() =>
            {
                var gitSetting = GetGitSetting();

                var isLocalMode = AppSettingsConfig.IsLocalMode();

                Initializer(isLocalMode, gitSetting);

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
                    ConfigDownloadManager.StartTimerIfNotExist($"{gitSetting.RemoteAddress}_{gitSetting.Branch}_{gitSetting.FetchInterval}".Trim(), gitSetting.FetchInterval, (s, e) => Initializer(isLocalMode, gitSetting));
                }

                return config;
            });
        }

        private static void Initializer(bool isLocalMode, GitSetting gitSetting)
        {
            var configFilePath = ConfigFileAttribute.GetFilePath(typeof(T));
            string configurationFullPath;

            //pull git repository
            if (!isLocalMode && DownloadConfiguration(gitSetting))
            {
                //find file
                configurationFullPath = FindFileAndGetFullPath(GitConfigDownloadDirectoryFullPath, configFilePath);
            }
            //local mode or download failure
            else
            {
                //maybe old configuration exist path (It's a compatible solution)
                configurationFullPath = Path.Combine(AppContext.BaseDirectory, "BambooConfig", Path.GetFileName(configFilePath));
            }

            InitializeConfigurationFile(configurationFullPath);
        }

        /// <summary>
        /// Default fit config download directory
        /// </summary>
        private const string GitConfigDefaultDownloadDirectory = "BambooConfigDownload";

        private static string[] SupportedConfigurationExtensions = new[] { ".json", ".xml", ".ini" };

        private static string GitConfigDownloadDirectoryFullPath = Path.Combine(AppContext.BaseDirectory, GitConfigDefaultDownloadDirectory);

        private static GitSetting GetGitSetting()
        {
            var gitSetting = AppSettingsConfig.GetGitSetting();

            if (string.IsNullOrEmpty(gitSetting.RemoteAddress))
                throw new ArgumentNullException(nameof(gitSetting.RemoteAddress));

            if (string.IsNullOrEmpty(gitSetting.Branch))
                throw new ArgumentNullException(nameof(gitSetting.Branch));

            //The pulling frequency should not be too high.
            if (gitSetting.FetchInterval < 5)
                gitSetting.FetchInterval = 30000;

            return gitSetting;
        }

        private static bool DownloadConfiguration(GitSetting groupSetting)
        {
            var workSpace = GitConfigDownloadDirectoryFullPath;

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
