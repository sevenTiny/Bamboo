using System;
using System.IO;
using System.Xml.Serialization;

namespace Bamboo.Configuration
{
    public class XmlConfigBase<T> : ConfigBase<T> where T : class, new()
    {
        private static string _ConfigName = ConfigNameAttribute.GetName(typeof(T));
        public static T Instance => GetConfig(_ConfigName);

        static XmlConfigBase()
        {
            RegisterGetRemoteFunction(_ConfigName, typeof(T), () =>
             {
                 var fullPath = Path.Combine(AppContext.BaseDirectory, $"{_ConfigName}.xml");
                 if (!File.Exists(fullPath))
                     throw new FileNotFoundException($"The configuration file does not exist in the path:{fullPath}");

                 using (StringReader reader = new StringReader(File.ReadAllText(fullPath)))
                 {
                     XmlSerializer serializer = new XmlSerializer(typeof(T));
                     T result = (T)(serializer.Deserialize(reader));
                     reader.Close();
                     return result;
                 }
             });
        }
    }
}
