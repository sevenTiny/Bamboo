using Microsoft.Extensions.Configuration;

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
    }
}
