using Microsoft.Extensions.Configuration;

namespace Bamboo.Configuration.Providers
{
    internal abstract class ConfigurationProviderBase
    {
        public abstract IConfigurationRoot GetConfigurationRoot(string configurationFullPath);

        public abstract string Serilize(object instance);
    }
}
