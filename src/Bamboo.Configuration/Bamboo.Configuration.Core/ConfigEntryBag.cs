using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Bamboo.Configuration
{
    internal static class ConfigEntryBag
    {
        /// <summary>
        /// config key and instance container
        /// </summary>
        internal static ConcurrentDictionary<string, ConfigEntry> ConfigEntries = new ConcurrentDictionary<string, ConfigEntry>();

        /// <summary>
        /// config filename and entry mapping container
        /// </summary>
        internal static ConcurrentDictionary<string, List<string>> ConfigNameAndEntriesKeysMapping = new ConcurrentDictionary<string, List<string>>();

        public static void AddOrUpdateConfigNameAndEntriesKeysMapping(string configName, string configEntryKey)
        {
            if (ConfigNameAndEntriesKeysMapping.TryGetValue(configName, out List<string> keys))
            {
                keys.Add(configEntryKey);
            }
            else
            {
                ConfigNameAndEntriesKeysMapping[configName] = new List<string>(1) { configEntryKey };
            }
        }
    }
}
