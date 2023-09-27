using System;
using System.Collections.Concurrent;
using System.IO;

namespace Bamboo.Configuration.Providers
{
    internal class ConfigurationProviderManager
    {
        private static ConcurrentDictionary<string, ConfigurationProviderBase> _providers = new ConcurrentDictionary<string, ConfigurationProviderBase>();

        private static ConfigurationProviderBase GetOrSetProvider(string extension, Func<ConfigurationProviderBase> setFunc)
        {
            if (_providers.ContainsKey(extension))
                return _providers[extension];

            var provider = setFunc();

            _providers.TryAdd(extension, provider);

            return provider;
        }

        public static ConfigurationProviderBase GetProvider(string configFileName)
        {
            var extension = Path.GetExtension(configFileName)?.ToLowerInvariant();

            switch (extension)
            {
                case ".json":
                    return GetOrSetProvider(extension, () => new JsonConfigurationProvider());
                case ".xml":
                    return GetOrSetProvider(extension, () => new XmlConfigurationProvider());
                case ".ini":
                    return GetOrSetProvider(extension, () => new IniConfigurationProvider());
                default:
                    throw new NotSupportedException($"the extension '{extension}' of configuration '{configFileName}' is not support");
            }
        }
    }
}
