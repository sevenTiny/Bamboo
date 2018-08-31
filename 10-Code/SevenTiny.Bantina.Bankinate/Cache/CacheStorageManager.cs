using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Helpers;
using System;

namespace SevenTiny.Bantina.Bankinate.Cache
{
    /// <summary>
    /// 缓存存储媒介
    /// </summary>
    public enum CacheMediaType
    {
        /// <summary>
        /// 本地缓存
        /// </summary>
        Local = 0,
        /// <summary>
        /// Redis缓存
        /// </summary>
        Redis = 1
    }

    /// <summary>
    /// 缓存存储管理器
    /// </summary>
    internal abstract class CacheStorageManager
    {
        public static bool IsExist(DbContext dbContext, string key)
        {
            return IsExist(dbContext, key,out object obj);
        }
        public static bool IsExist<TValue>(DbContext dbContext, string key,out TValue value)
        {
            switch (dbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    return MemoryCacheHelper.Exist(key,out value);
                case CacheMediaType.Redis:
                    value = default(TValue);
                    return false;
                default:
                    value = default(TValue);
                    return false;

            }
        }
        public static void Put<T>(DbContext dbContext, string key, T value, TimeSpan expiredTime)
        {
            switch (dbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    MemoryCacheHelper.Put(key, value, expiredTime);
                    break;
                case CacheMediaType.Redis:
                    break;
                default:
                    break;
            }
        }
        public static T Get<T>(DbContext dbContext, string key)
        {
            switch (dbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    return MemoryCacheHelper.Get<string, T>(key);
                case CacheMediaType.Redis:
                    return default(T);
                default:
                    return default(T);
            }
        }
        public static void Delete(DbContext dbContext, string key)
        {
            switch (dbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    break;
                case CacheMediaType.Redis:
                    break;
                default:
                    break;
            }
        }
    }
}
