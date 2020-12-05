using System.IO;
using System.Xml.Serialization;

namespace Bamboo.Configuration.Git.Serializer
{
    internal class XmlConfigSerializer : ConfigSerializerBase
    {
        internal override T Deserialize<T>(string fileFullPath)
        {
            using (StringReader reader = new StringReader(File.ReadAllText(fileFullPath)))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T result = (T)(serializer.Deserialize(reader));
                reader.Close();
                return result;
            }
        }
    }
}
