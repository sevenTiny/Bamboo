using Newtonsoft.Json;
using System.IO;

namespace Bamboo.Configuration.Git.Serializer
{
    internal class JsonConfigSerializer : ConfigSerializerBase
    {
        internal override T Deserialize<T>(string fileFullPath)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(fileFullPath));
        }
    }
}
