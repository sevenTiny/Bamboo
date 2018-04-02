using SevenTiny.Bantina.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenTiny.Bantina.Configuration
{
    [ConfigClass(Name = "SevenTinyBantina")]
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
