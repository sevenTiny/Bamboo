/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-27 19:23:12
 * Modify: 2018-02-27 19:23:12
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Helpers;
using SevenTiny.Bantina.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Bantina.Redis
{
    [ConfigName("Redis")]
    internal class RedisConfig : MySqlRowConfigBase<RedisConfig>
    {
        [ConfigProperty]
        public string KeySpace { get; set; }
        [ConfigProperty]
        public string Key { get; set; }
        [ConfigProperty]
        public string Value { get; set; }
        [ConfigProperty]
        public string Description { get; set; }

        private static Dictionary<string, Dictionary<string, string>> dictionary;

        private static void Initial()
        {
            var group = Instance.GroupBy(t => t.KeySpace).Select(t => new { KeySpace = t.Key, RedisConfig = t }).ToList();
            dictionary = new Dictionary<string, Dictionary<string, string>>();
            foreach (var item in group)
            {
                var innerDic = new Dictionary<string, string>();
                foreach (var config in item.RedisConfig)
                {
                    if (innerDic.ContainsKey(config.Key))
                        innerDic[config.Key] = config.Value;
                    else
                        innerDic.Add(config.Key, config.Value);
                }

                if (dictionary.ContainsKey(item.KeySpace))
                    dictionary[item.KeySpace] = innerDic;
                else
                    dictionary.Add(item.KeySpace, innerDic);
            }
        }

        /// <summary>
        /// get redis config value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string keySpace, string key)
        {
            try
            {
                if (dictionary != null && dictionary.ContainsKey(keySpace))
                {
                    if (!dictionary[keySpace].ContainsKey(key))
                    {
                        Initial();
                    }
                    return dictionary[keySpace][key] ?? throw new ArgumentNullException($"Redis Config of keyspace({keySpace}), key ({key}) not exist or error value!");
                }
                Initial();
                if (!dictionary.ContainsKey(keySpace))
                {
                    throw new ArgumentNullException($"Redis Config of keyspace({keySpace}) not exist or error value!");
                }
                return dictionary[keySpace][key];
            }
            catch (Exception)
            {
                throw new ArgumentNullException($"Redis Config of keyspace({keySpace}), key ({key}) not exist or error value!");
            }
        }
    }
}
