using Bamboo.Configuration.Helpers;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Bamboo.Configuration.Providers
{
    internal class XmlConfigurationProvider : ConfigurationProviderBase
    {
        public override IConfigurationRoot GetConfigurationRoot(string configurationFullPath)
        {
            return new ConfigurationBuilder()
                .AddXmlFile(configurationFullPath, optional: false, reloadOnChange: true)
                .Build();
        }

        public override string Serilize(object instance)
        {
            XmlSerializer serializer = new XmlSerializer(instance.GetType());

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

                    serializer.Serialize(xmlWriter, instance, xmlNamespaces);
                    return writer.ToString();
                }
            }
        }
    }
}
