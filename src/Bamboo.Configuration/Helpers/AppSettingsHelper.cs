using Bamboo.Configuration.Providers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bamboo.Configuration.Helpers
{
    internal static class AppSettingsHelper
    {
        /// <summary>
        /// bamboo configuration section
        /// </summary>
        private const string BambooConfig = "BambooConfig";
        private const string Git = "Git";

        //configuration instance
        private static IConfigurationRoot Root;

        static AppSettingsHelper()
        {
            Root = new JsonConfigurationProvider().GetConfigurationRoot("appsettings.json");
        }

        /// <summary>
        /// get bamboo configuration section
        /// </summary>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        internal static IConfigurationSection GetBambooConfigurationSection()
        {
            var section = Root.GetSection(BambooConfig);

            if (section == null || !section.Exists())
                throw new KeyNotFoundException($"'{BambooConfig}' node has not exist in the 'appsettings.json' configuration file.");

            return section;
        }

        /// <summary>
        /// get git settings
        /// </summary>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="FormatException"></exception>
        internal static (bool, List<GitSetting>) GetGitSettings()
        {
            var section = GetBambooConfigurationSection();

            var gitSection = section.GetSection(Git);

            if (gitSection == null || !gitSection.Exists())
                return (false, null);

            var settings = gitSection.Get<List<GitSetting>>();

            if (settings != null && settings.Any())
                return (true, settings);

            var setting = gitSection.Get<GitSetting>();

            if (setting != null)
                return (true, new List<GitSetting> { setting });

            return (false, null);
        }
    }

    internal class GitSetting
    {
        /// <summary>
        /// mark the repo
        /// </summary>
        public string RepoName { get; set; }
        /// <summary>
        /// git remote address
        /// </summary>
        public string RemoteAddress { get; set; }
        public string Branch { get; set; }
        public bool AutoFetch { get; set; }
        public double AutoFetchInterval { get; set; }
    }
}