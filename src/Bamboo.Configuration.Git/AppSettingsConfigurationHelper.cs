using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Bamboo.Configuration.Git
{
    internal class AppSettingsConfigurationHelper
    {
        /// <summary>
        /// bamboo configuration section
        /// </summary>
        private const string BambooConfig = "BambooConfig";
        private const string GitGroup = "GitGroup";

        public static GroupSetting GetGroupSetting(string group)
        {
            var section = AppSettingsConfig.GetBambooConfigurationSection();

            var gitGroup = section.GetSection(GitGroup);

            if (gitGroup == null || !gitGroup.Exists())
                throw new KeyNotFoundException($"'{BambooConfig}.{GitGroup}' node has not exist in the 'appsettings.json' configuration file.");

            var configSetting = gitGroup.GetSection(group);

            if (configSetting == null || !configSetting.Exists())
                throw new KeyNotFoundException($"'{BambooConfig}.{GitGroup}.{group}' node has not exist in the 'appsettings.json' configuration file.");

            var configSettingInstance = configSetting.Get<GroupSetting>();

            if (configSettingInstance == null)
                throw new FormatException($"'{BambooConfig}.{GitGroup}.{group}' node in 'appsettings.json' configuration is not correctly, please check your configuration item.");

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
