using Bamboo.Configuration.Xml;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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

        protected override string SerializeConfigurationInstance()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (var writer = new StringWriterWithEncoding(Encoding.UTF8))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    OmitXmlDeclaration = true,
                };

                using (var xmlWriter = XmlWriter.Create(writer, settings))
                {
                    //remove xml namespaces
                    var xmlNamespaces = new XmlSerializerNamespaces();
                    xmlNamespaces.Add("", "");

                    serializer.Serialize(xmlWriter, this, xmlNamespaces);
                    return writer.ToString();
                }
            }
        }
    }
}
