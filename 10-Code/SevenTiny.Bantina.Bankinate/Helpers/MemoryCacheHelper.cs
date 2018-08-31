/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-19 13:17:32
 * Modify: 2018-02-19 13:17:32
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using Microsoft.Extensions.Caching.Memory;
using System;

namespace SevenTiny.Bantina.Bankinate.Helpers
{
    internal static class MemoryCacheHelper
    {
        private static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public static TValue Put<TKey, TValue>(TKey key, TValue value) => _cache.Set(key, value);
        public static TValue Put<TKey, TValue>(TKey key, TValue value, TimeSpan absoluteExpirationRelativeToNow) => _cache.Set(key, value, absoluteExpirationRelativeToNow);
        public static TValue Put<TKey, TValue>(TKey key, TValue value, DateTime absoluteExpiration) => _cache.Set(key, value, absoluteExpiration - DateTime.Now);
        public static TValue Get<TKey, TValue>(TKey key)
        {
            if (Exist(key))
            {
                return _cache.Get<TValue>(key);
            }
            return default(TValue);
        }

        public static bool Exist<TKey>(TKey key) => _cache.TryGetValue(key, out object value);
        public static bool Exist<TKey, TValue>(TKey key, out TValue value)=> _cache.TryGetValue(key, out value);
        public static void Delete<TKey>(TKey key) => _cache.Remove(key);
    }
}
