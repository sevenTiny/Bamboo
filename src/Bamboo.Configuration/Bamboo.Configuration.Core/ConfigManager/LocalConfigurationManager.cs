using Newtonsoft.Json;
using Bamboo.Configuration.Core.Helpers;
using System;
using System.IO;
using Bamboo.Logging;
using Microsoft.Extensions.Logging;

namespace Bamboo.Configuration
{
    internal class LocalConfigurationManager : ConfigurationManagerBase
    {
        public static LocalConfigurationManager Instance => new LocalConfigurationManager();
        private ILogger _logger = new BambooLogger<LocalConfigurationManager>();
        private LocalConfigurationManager() { }

        protected override object OnCreate(string configName, Type type)
        {
            return GetConfigInstance(ConfigPathHelper.GetConfigFileFullPath(configName), type);
        }

        public object GetConfigInstance(string fileFullPath, Type type)
        {
            object result = null;

            if (File.Exists(fileFullPath))
            {
                result = JsonConvert.DeserializeObject(File.ReadAllText(fileFullPath), type);

                //listen file modify
                ConfigManagementHandler.CreateAndSetupWatcher(result, fileFullPath, OnConfigFileChanged);
            }

            return result;
        }

        public object GetConfigInstanceNoListen(string fileFullPath, Type type)
        {
            if (File.Exists(fileFullPath))
                return JsonConvert.DeserializeObject(File.ReadAllText(fileFullPath), type);

            return null;
        }

        private void OnConfigFileChanged(object sender, EventArgs args)
        {
            string configFullPath = string.Empty;
            try
            {
                var fileChangedEventArgs = (FileChangedEventArgs)args;

                configFullPath = fileChangedEventArgs.FileFullPath;

                _logger.LogDebug($"config '{configFullPath}' refresh finished.");

                UpdateEntry(Path.GetFileNameWithoutExtension(fileChangedEventArgs.FileFullPath), fileChangedEventArgs.ConfigInstance, fileChangedEventArgs.ConfigType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"config '{configFullPath}' refresh error.");
            }
        }
    }
}
