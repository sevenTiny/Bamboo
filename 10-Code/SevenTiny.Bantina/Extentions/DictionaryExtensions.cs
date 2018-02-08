/*********************************************************
 * CopyRight: QIXIAO CODE BUILDER. 
 * Version: 5.0.0
 * Author: sevenTiny
 * Address: Earth
 * Create: 2017-12-03 21:12:20
 * Update: 2017-12-03 21:12:20
 * E-mail: dong@qixiao.me | wd8622088@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System.Collections.Generic;

namespace SevenTiny.Bantina.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// If exist the key , update the value.
        /// </summary>
        /// <typeparam name="TKey">type of Key</typeparam>
        /// <typeparam name="TValue">type of Value</typeparam>
        /// <param name="dictionary">dictionary</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }
}
