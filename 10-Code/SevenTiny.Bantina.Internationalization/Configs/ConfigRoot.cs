using SevenTiny.Bantina.Configuration;
using SevenTiny.Bantina.Extensions;
using System.Collections.Generic;

namespace SevenTiny.Bantina.Internationalization.Configuration
{
    /// <summary>
    /// 该配置文件是SevenTiny系列组建的配置文件，如用在其他项目则忽略该类
    /// </summary>
    [ConfigName(Name = "SevenTinyBantina")]
    public class ConfigRoot : ConfigBase<ConfigRoot>
    {
        public string Key { get; set; }
        public string Value { get; set; }

        private static Dictionary<string, string> dictionary;

        private static void Initial()
        {
            dictionary = new Dictionary<string, string>();
            foreach (var item in Configs)
            {
                dictionary.AddOrUpdate(item.Key, item.Value);
            }
        }

        public static string Get(string key)
        {
            if (dictionary != null && dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            Initial();
            return dictionary.SafeGet(key);
        }
    }
}
