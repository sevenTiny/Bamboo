using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Bamboo.Configuration.Git
{
    /// <summary>
    /// setting manager
    /// </summary>
    [ConfigFile("appsettings.json")]
    internal class AppSettings : JsonConfigBase<AppSettings>
    {
        /// <summary>
        /// key in appsettings.json connection string config
        /// </summary>
        private const string DefaultAppSettingsKey = "BambooConfig";

        public static GroupSetting GetGroupSetting(string group)
        {
            var section = ConfigurationRoot.GetSection(DefaultAppSettingsKey);

            if (section == null || !section.Exists())
                throw new KeyNotFoundException($"'{DefaultAppSettingsKey}' node has not exist in the 'appsettings.json' ConnectionStrings file.");

            var configSetting = section.GetSection(group);

            if (configSetting == null || !configSetting.Exists())
                throw new KeyNotFoundException($"'{DefaultAppSettingsKey}.{group}' node has not exist in the 'appsettings.json' ConnectionStrings file.");

            var configSettingInstance = configSetting.Get<GroupSetting>();

            if (configSettingInstance == null)
                throw new FormatException($"'{DefaultAppSettingsKey}.{group}' node in 'appsettings.json' configuration is not correctly, please check your configuration item.");

            return configSettingInstance;
        }
    }

    internal class GroupSetting
    {
        /// <summary>
        /// git remote address
        /// </summary>
        public string RemoteAddress { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Branch { get; set; }
        public double FetchInterval { get; set; }
    }
}
