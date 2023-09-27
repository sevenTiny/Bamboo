using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Bamboo.Configuration.Providers
{
    internal class JsonConfigurationProvider : ConfigurationProviderBase
    {
        public override IConfigurationRoot GetConfigurationRoot(string configurationFullPath)
        {
            return new ConfigurationBuilder()
                .AddJsonFile(configurationFullPath, optional: false, reloadOnChange: true)
                .Build();
        }

        public override string Serilize(object instance)
        {
            return JsonConvert.SerializeObject(instance);
        }
    }
}
