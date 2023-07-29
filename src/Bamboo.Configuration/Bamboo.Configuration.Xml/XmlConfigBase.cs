using Microsoft.Extensions.Configuration;

namespace Bamboo.Configuration
{
    public class XmlConfigBase<T> : ConfigBase<T> where T : class, new()
    {
        static XmlConfigBase()
        {
            RegisterInitializer(() =>
            {
                InitializeConfigurationFile();

                return new ConfigurationBuilder()
                .AddXmlFile(ConfigurationFilePath, optional: false, reloadOnChange: true)
                .Build();
            });
        }
    }
}
