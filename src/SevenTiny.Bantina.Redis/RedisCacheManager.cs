/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-28 23:21:57
 * Modify: 2018-3-1 23:24:10
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;

namespace SevenTiny.Bantina.Redis
{
    public class RedisCacheManager : RedisServerManager, IRedisCache
    {
        private TimeSpan AbsoluteExpirationRelativeToNow { get; set; }

        public RedisCacheManager(string keySpace)
            : base(keySpace, RedisConfig.Get(keySpace, "Server"), RedisConfig.Get(keySpace, "Port"))
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Convert.ToDouble(RedisConfig.Get(keySpace, "DefaultExpirySeconds")));
        }

        public RedisCacheManager(string server, string port, int defaultExpireSecond = 1800)
            : base("Default", server, port)
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(defaultExpireSecond);
        }

        public string Get(string key) => Db.StringGet(key);

        public void Set(string key, string value) => Db.StringSet(key, value, AbsoluteExpirationRelativeToNow);

        public void Set(string key, string value, TimeSpan absoluteExpirationRelativeToNow) => Db.StringSet(key, value, absoluteExpirationRelativeToNow);

        public void Set(string key, string value, DateTime absoluteExpiration) => Db.StringSet(key, value, absoluteExpiration - DateTime.Now);

        public void Update(string key, string value) => Db.StringSet(key, value);

        public void Delete(string key) => Db.KeyDelete(key);

        public bool Exist(string key) => Db.KeyExists(key);

        public long StringIncrement(string key) => Db.StringIncrement(key);

        public double StringIncrement(string key, double incrementNumber) => Db.StringIncrement(key, incrementNumber);
    }
}
