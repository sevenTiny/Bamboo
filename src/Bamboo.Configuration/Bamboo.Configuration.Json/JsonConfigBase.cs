using Newtonsoft.Json;
using System;
using System.IO;

namespace Bamboo.Configuration
{
    public class JsonConfigBase<T> : ConfigBase<T> where T : class, new()
    {
        private static string _ConfigName = ConfigNameAttribute.GetName(typeof(T));
        public static T Instance => GetConfig(_ConfigName);

        static JsonConfigBase()
        {
            RegisterGetRemoteFunction(_ConfigName, typeof(T), () =>
             {
                 var fullPath = Path.Combine(AppContext.BaseDirectory, $"{_ConfigName}.json");
                 return JsonConvert.DeserializeObject<T>(File.ReadAllText(fullPath));
             });
        }
    }
}
