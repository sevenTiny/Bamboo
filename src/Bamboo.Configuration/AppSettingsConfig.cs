using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Bamboo.Configuration
{
    /// <summary>
    /// setting manager
    /// </summary>
    [ConfigFile("appsettings.json")]
    public class AppSettingsConfig : JsonConfigBase<AppSettingsConfig>
    {
        /// <summary>
        /// bamboo configuration section
        /// </summary>
        private const string BambooConfig = "BambooConfig";
        private const string Git = "Git";

        /// <summary>
        /// get bamboo configuration section
        /// </summary>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static IConfigurationSection GetBambooConfigurationSection()
        {
            var section = ConfigurationRoot.GetSection(BambooConfig);

            if (section == null || !section.Exists())
                throw new KeyNotFoundException($"'{BambooConfig}' node has not exist in the 'appsettings.json' ConnectionStrings file.");

            return section;
        }

        /// <summary>
        /// verify the bamboo configuration component localmode is open
        /// </summary>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static bool IsLocalMode()
        {
            var section = GetBambooConfigurationSection().GetSection("LocalMode");

            if (section == null || !section.Exists())
                throw new KeyNotFoundException($"'LocalMode' node has not exist in the 'appsettings.json' BambooConfig Node.");

            return GetBambooConfigurationSection().GetValue<bool>("LocalMode");
        }

        /// <summary>
        /// get group setting
        /// </summary>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="FormatException"></exception>
        internal static GitSetting GetGitSetting()
        {
            var section = AppSettingsConfig.GetBambooConfigurationSection();

            var gitSection = section.GetSection(Git);

            if (gitSection == null || !gitSection.Exists())
                throw new KeyNotFoundException($"'{BambooConfig}.{Git}' node has not exist in the 'appsettings.json' configuration file.");

            var gitSectionInstance = gitSection.Get<GitSetting>();

            if (gitSectionInstance == null)
                throw new FormatException($"'{BambooConfig}.{Git}' node in 'appsettings.json' configuration is not correctly, please check your configuration item.");

            return gitSectionInstance;
        }
    }

    internal class GitSetting
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
