using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Bamboo.Configuration
{
    public class JsonConfigBase<T> : ConfigBase<T> where T : class, new()
    {
        static JsonConfigBase()
        {
            RegisterInitializer(() =>
            {
                InitializeConfigurationFile();

                return new ConfigurationBuilder()
                .AddJsonFile(ConfigurationFilePath, optional: false, reloadOnChange: true)
                .Build();
            });
        }

        protected override string SerializeConfigurationInstance()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
