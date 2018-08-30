using SevenTiny.Bantina.Bankinate.Helpers;
using System;

namespace SevenTiny.Bantina.Bankinate.Cache
{
    public abstract class CacheStorage
    {
        public static void Put<T>(CacheMediaType cacheMediaType,string key,T value,TimeSpan expiredTime)
        {
            switch (cacheMediaType)
            {
                case CacheMediaType.Local:
                    MemoryCacheHelper.Put(key,value,expiredTime);
                    break;
                default:
                    break;
            }
        }
        public static T Get<T>(CacheMediaType cacheMediaType, string key)
        {
            switch (cacheMediaType)
            {
                case CacheMediaType.Local:
                    return MemoryCacheHelper.Get<string,T>(key);
                default:
                    return default(T);
            }
        }
    }
}
