using Microsoft.Extensions.Configuration;

namespace Bamboo.Configuration.Providers
{
    internal class IniConfigurationProvider : ConfigurationProviderBase
    {
        public override IConfigurationRoot GetConfigurationRoot(string configurationFullPath)
        {
            return new ConfigurationBuilder()
                .AddIniFile(configurationFullPath, optional: false, reloadOnChange: true)
                .Build();
        }

        public override string Serilize(object instance)
        {
            throw new System.NotImplementedException();
        }
    }
}
