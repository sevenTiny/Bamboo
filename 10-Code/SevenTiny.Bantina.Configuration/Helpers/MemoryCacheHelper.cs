/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-14 01:16:58
 * Modify: 2018-02-14 01:16:58
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using Microsoft.Extensions.Caching.Memory;
using System;

namespace SevenTiny.Bantina.Configuration.Helpers
{
    public class MemoryCacheHelper
    {
        private static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public static bool Put<TKey,TValue>(TKey key, TValue value)
        {
            _cache.Set(key, value);
            return true;
        }
        public static bool Put<TKey, TValue>(TKey key, TValue value,TimeSpan expiredTimeMinutes)
        {
            _cache.Set(key, value, expiredTimeMinutes);
            return true;
        }
        public static TValue Get<TKey,TValue>(TKey key)
        {
            if (Exist(key))
            {
                return _cache.Get<TValue>(key);
            }
            return default(TValue);
        }
        public static bool Exist<TKey>(TKey key)
        {
            return _cache.TryGetValue(key, out object value);
        }
    }
}
