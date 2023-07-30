using System;

namespace SevenTiny.Bantina.Bankinate.Caching.Helpers.Redis
{
    internal interface IRedisCache
    {
        string Get(string key);
        void Set(string key, string value);
        void Set(string key, string value, TimeSpan absoluteExpirationRelativeToNow);
        void Set(string key, string value, DateTime absoluteExpiration);
        void Update(string key, string value);
        void Delete(string key);
        bool Exist(string key);
        long StringIncrement(string key);
        double StringIncrement(string key, double incrementNumber);
    }
}
