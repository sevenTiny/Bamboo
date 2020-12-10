using LibGit2Sharp;
using Bamboo.Configuration.Git;
using Bamboo.Configuration.Git.Serializer;
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
        private static string _ConfigName = ConfigNameAttribute.GetName(typeof(T));
        public static T Instance => GetConfig(_ConfigName);

        static GitConfigBase()
        {
            //register instance
            RegisterGetRemoteFunction(_ConfigName, typeof(T), () =>
            {
                //download
                var workSpace = DownloadConfigurationAndGetWorkSpace();

                //find file
                var configurationFullPath = FindFileAndGetFullPath(workSpace, _ConfigName);

                //serialization
                var serializer = GetSerializerByConfigType(configurationFullPath);

                return serializer.Deserialize<T>(configurationFullPath);
            });
        }

        /// <summary>
        /// default setting key in appsettings config file
        /// </summary>
        private const string DefaultConfigSettingKey = "Default";

        /// <summary>
        /// Default fit config download directory
        /// </summary>
        private const string GitConfigDefaultDownloadDirectory = "BambooConfigDownload";

        private static string GitConfigDefaultDownloadDirectoryFullPath = Path.Combine(AppContext.BaseDirectory, GitConfigDefaultDownloadDirectory);

        private static string DownloadConfigurationAndGetWorkSpace()
        {
            //get setting key
            string settingKey = ConfigSettingUseAttribute.GetName(typeof(T)) ?? DefaultConfigSettingKey;

            //get config
            var configSetting = BambooConfigSetting.GetConfigSettingFromAppSettings<T>(settingKey);

            if (string.IsNullOrEmpty(configSetting.RemoteAddress))
                throw new ArgumentNullException(nameof(configSetting.RemoteAddress));
            if (string.IsNullOrEmpty(configSetting.Branch))
                throw new ArgumentNullException(nameof(configSetting.Branch));

            var workSpace = Path.Combine(GitConfigDefaultDownloadDirectoryFullPath, settingKey);

            //Ensure directory exist
            if (!Directory.Exists(workSpace))
                Directory.CreateDirectory(workSpace);

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

            return workSpace;
        }

        private static string FindFileAndGetFullPath(string workSpace, string configName)
        {
            //if config name contains extension
            var fileExtension = Path.GetExtension(configName);
            if (!string.IsNullOrEmpty(fileExtension) && SupportedConfigurationExtensions.Any(t => t.Equals(fileExtension)))
            {
                var fileFullPath = Path.Combine(workSpace, configName);

                if (!File.Exists(fileFullPath))
                    throw new FileNotFoundException($"The configuration file does not exist in the path:{fileFullPath}");
            }

            //if config name not contains extension
            var foundFiles = Directory.GetFiles(workSpace, $"{configName}.*", SearchOption.AllDirectories);

            if (!foundFiles.Any())
                throw new FileNotFoundException($"Configuration file not found with config name '{configName}'.");

            //configuration file more than one
            if (foundFiles.Length > 1)
                throw new FileNotFoundException($"More than one configuration file was found. The application does not know which one to take. configuration files:[{string.Join(",", foundFiles)}].");

            return foundFiles[0];
        }

        private static string[] SupportedConfigurationExtensions = new[] { ".json", ".xml" };

        private static ConfigSerializerBase GetSerializerByConfigType(string fileFullName)
        {
            var extension = Path.GetExtension(fileFullName);

            switch (extension)
            {
                case ".json":
                    return new JsonConfigSerializer();
                case ".xml":
                    return new XmlConfigSerializer();
                default:
                    throw new NotSupportedException($"the extension of '{extension}' is not supported.");
            }
        }
    }
}
