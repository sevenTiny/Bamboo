using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bamboo.Configuration.Git
{
    /// <summary>
    /// setting manager
    /// </summary>
    internal static class BambooConfigSetting
    {
        /// <summary>
        /// locker
        /// </summary>
        private static readonly object locker = new object();

        /// <summary>
        /// store connectionString
        /// </summary>
        private static Dictionary<string, ConfigSetting> _ConnectionStringDic = new Dictionary<string, ConfigSetting>();

        private static ConfigSetting GetConfigSettingFromCache(string connectionStringName, Func<ConfigSetting> func)
        {
            //get connection string from value
            if (_ConnectionStringDic.ContainsKey(connectionStringName))
                return _ConnectionStringDic[connectionStringName];

            lock (locker)
            {
                //multi-check
                if (_ConnectionStringDic.ContainsKey(connectionStringName))
                    return _ConnectionStringDic[connectionStringName];

                var connectionString = func();

                _ConnectionStringDic.Add(connectionStringName, connectionString);

                return connectionString;
            }
        }

        /// <summary>
        /// key in appsettings.json connection string config
        /// </summary>
        private const string DefaultAppSettingsKey = "BambooConfig";

        public static ConfigSetting GetConfigSettingFromAppSettings<T>(string settingKey)
        {
            return GetConfigSettingFromCache(settingKey, () =>
            {
                //get connection string from config file
                string baseDirectory = AppContext.BaseDirectory;

                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(baseDirectory)//current base directory
                    .AddJsonFile("appsettings.json", false, false)
                    .Build();

                var section = config.GetSection(DefaultAppSettingsKey);

                if (section == null || !section.Exists())
                    throw new FileNotFoundException($"'appsettings.json' not find in {baseDirectory}, if existed,maybe '{DefaultAppSettingsKey}' node has not exist in the 'appsettings.json' ConnectionStrings file.");

                var configSetting = section.GetSection(settingKey);

                if (configSetting == null || !configSetting.Exists())
                    throw new FileNotFoundException($"'appsettings.json' not find in {baseDirectory}, if existed,maybe '{DefaultAppSettingsKey}' node has not exist in the 'appsettings.json' ConnectionStrings file.");

                var configSettingInstance = configSetting.Get<ConfigSetting>();

                if (configSettingInstance == null)
                    throw new FormatException($"'appsettings.json' config of {DefaultAppSettingsKey}.{settingKey} configuration is not correctly,please check your configuration item.");

                return configSettingInstance;
            });
        }
    }

    internal class ConfigSetting
    {
        /// <summary>
        /// git remote address
        /// </summary>
        public string RemoteAddress { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Branch { get; set; }
    }
}
