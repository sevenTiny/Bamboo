using Newtonsoft.Json;
using SevenTiny.Bantina.Bankinate.Caching.Helpers;
using SevenTiny.Bantina.Bankinate.Caching.Helpers.Redis;
using System;

namespace SevenTiny.Bantina.Bankinate.Caching
{
    /// <summary>
    /// 缓存存储管理器
    /// </summary>
    internal class CacheStorageManager
    {
        internal CacheOptions _CacheOptions;

        internal CacheStorageManager(CacheOptions cacheOptions)
        {
            _CacheOptions = cacheOptions;
        }

        private static IRedisCache redisCache = null;
        private IRedisCache GetRedisCacheProvider()
        {
            if (redisCache != null)
                return redisCache;

            //配置的异常是参数异常，在处理数据时应抛出异常，其他连接异常应该忽略返回获取缓存失败
            if (string.IsNullOrEmpty(_CacheOptions.CacheMediaServer))
                throw new ArgumentException("Cache server address error", "_CacheOptions.CacheMediaServer");

            redisCache = new RedisCacheManager(_CacheOptions.CacheMediaServer);

            if (redisCache == null)
                throw new Exception("redis init timeout");

            return redisCache;
        }

        public bool IsExist(string key)
        {
            return IsExist(key, out object obj);
        }
        public bool IsExist<TValue>(string key, out TValue value)
        {
            switch (_CacheOptions.CacheMediaType)
            {
                case CacheMediaType.Local:
                    return MemoryCacheHelper.Exist(key, out value);
                case CacheMediaType.Redis:
                    try
                    {
                        var redisResult = GetRedisCacheProvider().Get(key);
                        if (!string.IsNullOrEmpty(redisResult))
                        {
                            value = JsonConvert.DeserializeObject<TValue>(redisResult);
                            return true;
                        }
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    value = default(TValue);
                    return false;
                default:
                    value = default(TValue);
                    return false;

            }
        }
        public void Put<T>(string key, T value, TimeSpan expiredTime)
        {
            switch (_CacheOptions.CacheMediaType)
            {
                case CacheMediaType.Local:
                    MemoryCacheHelper.Put(key, value, expiredTime);
                    break;
                case CacheMediaType.Redis:
                    try
                    {
                        GetRedisCacheProvider().Set(key, JsonConvert.SerializeObject(value), expiredTime);
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    break;
                default:
                    break;
            }
        }
        public T Get<T>(string key)
        {
            switch (_CacheOptions.CacheMediaType)
            {
                case CacheMediaType.Local:
                    return MemoryCacheHelper.Get<string, T>(key);
                case CacheMediaType.Redis:
                    try
                    {
                        var redisResult = GetRedisCacheProvider().Get(key);
                        if (!string.IsNullOrEmpty(redisResult))
                        {
                            return JsonConvert.DeserializeObject<T>(redisResult);
                        }
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    return default(T);
                default:
                    return default(T);
            }
        }
        public void Delete(string key)
        {
            switch (_CacheOptions.CacheMediaType)
            {
                case CacheMediaType.Local:
                    MemoryCacheHelper.Delete<string>(key);
                    break;
                case CacheMediaType.Redis:
                    try
                    {
                        GetRedisCacheProvider().Delete(key);
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
