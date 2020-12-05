using Newtonsoft.Json;
using Bamboo.Configuration.Core.Helpers;
using System;
using System.IO;

namespace Bamboo.Configuration
{
    internal class LocalConfigurationManager : ConfigurationManagerBase
    {
        public static LocalConfigurationManager Instance => new LocalConfigurationManager();

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
            try
            {
                var fileChangedEventArgs = (FileChangedEventArgs)args;
                UpdateEntry(Path.GetFileNameWithoutExtension(fileChangedEventArgs.FileFullPath), fileChangedEventArgs.ConfigInstance, fileChangedEventArgs.ConfigType);
            }
            catch (Exception)
            {
                //log
            }
        }
    }
}
