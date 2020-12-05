using Bamboo.Configuration.Core.Helpers;
using System;

namespace Bamboo.Configuration
{
    internal abstract class ConfigurationManagerBase
    {
        protected abstract object OnCreate(string configName, Type type);

        protected ConfigurationManagerBase() { }

        internal void UpdateEntry(string configName, object configInstance, Type configType)
        {
            //change reference of entry
            var key = GenerateUniqueConfigName(configName, configType);

            if (ConfigEntryBag.ConfigEntries.ContainsKey(key))
                ConfigEntryBag.ConfigEntries[key].Value = configInstance;
        }

        public T GetSection<T>(string configName)
        {
            var key = GenerateUniqueConfigName(configName, typeof(T));

            ConfigEntryBag.ConfigEntries.TryGetValue(key, out ConfigEntry entry);

            if (entry == null || entry.Value == null)
            {
                entry = new ConfigEntry(configName, OnCreate, typeof(T));

                ConfigEntryBag.AddOrUpdateConfigNameAndEntriesKeysMapping(configName, key);

                ConfigEntryBag.ConfigEntries.AddOrUpdate(key, entry);
            }

            return (T)entry.Value;
        }

        /// <summary>
        /// Generate Unique Config Name (with type) ,No Repeat
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="configActualType">which use in ConfigBase<'type'></param>
        /// <returns></returns>
        protected string GenerateUniqueConfigName(string configName, Type configActualType)
        {
            return string.Concat(configName, ConfigurationConst.SPLITE_SYMBOL, configActualType?.AssemblyQualifiedName?.Replace(" ", "").Replace(",", "").Replace("=", "")?.Trim());
        }
    }
}
