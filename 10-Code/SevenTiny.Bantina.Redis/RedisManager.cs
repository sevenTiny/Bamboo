/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-28 23:21:57
 * Modify: 2018-02-28 23:21:57
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using StackExchange.Redis;
using System;

namespace SevenTiny.Bantina.Redis
{
    public class RedisManager : IRedisCache
    {
        private ConnectionMultiplexer Redis { get; set; }
        private IDatabase Db { get; set; }

        public static IRedisCache Instance => new RedisManager();

        private RedisManager()
        {
            Redis = ConnectionMultiplexer.Connect($"{RedisConfig.Default.Server}:{RedisConfig.Default.Port}");
            Db = Redis.GetDatabase();
        }

        public string Get(string key) => Db.StringGet(key);

        public void Post(string key, string value) => Db.StringSet(key, value);

        public void Post(string key, string value, TimeSpan absoluteExpirationRelativeToNow) => Db.StringSet(key, value, absoluteExpirationRelativeToNow);

        public void Post(string key, string value, DateTime absoluteExpiration) => Db.StringSet(key, value, absoluteExpiration - DateTime.Now);

        public void Put(string key, string value) => Db.StringSet(key, value);

        public void Delete(string key) => Db.KeyDelete(key);

        public bool Exist(string key) => Db.KeyExists(key);
    }
}
