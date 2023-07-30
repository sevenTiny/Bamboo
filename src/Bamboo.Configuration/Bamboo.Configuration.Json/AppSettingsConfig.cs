using Microsoft.Extensions.Configuration;
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
    }
}
