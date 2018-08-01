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
using SevenTiny.Bantina.Logging;
using StackExchange.Redis;
using System;
using System.Threading;

namespace SevenTiny.Bantina.Redis
{
    public class RedisCacheManager : IRedisCache
    {
        private ConnectionMultiplexer Redis { get; set; }
        private IDatabase Db { get; set; }

        private static IRedisCache _instance;

        /// <summary>
        /// lock
        /// </summary>
        private readonly static object lockObject = new object();

        /// <summary>
        /// Singleton pattern Instance
        /// </summary>
        public static IRedisCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RedisCacheManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// logger
        /// </summary>
        private static ILog log = new LogManager();

        private RedisCacheManager()
        {
            //set establish retry mechanism (3 times)
            int retryCount = 2;
            while (true)
            {
                try
                {
                    Redis = ConnectionMultiplexer.Connect($"{RedisConfig.Get("Server")}:{RedisConfig.Get("Port")}");
                    Db = Redis.GetDatabase();
                    break;
                }
                catch (Exception ex)
                {
                    if (retryCount > 0)
                    {
                        retryCount--;
                        Thread.Sleep(1000);
                        continue;
                    }
                    log.Error("Redis server establish  connection error!", ex);
                    throw new TimeoutException($"redis init timeout,server reject or other.ex{ex.ToString()}");
                }
            }
        }

        public string Get(string key) => Db.StringGet(key);

        public void Set(string key, string value) => Db.StringSet(key, value, TimeSpan.FromSeconds(Convert.ToDouble(RedisConfig.Get("DefaultExpirySeconds"))));

        public void Set(string key, string value, TimeSpan absoluteExpirationRelativeToNow) => Db.StringSet(key, value, absoluteExpirationRelativeToNow);

        public void Set(string key, string value, DateTime absoluteExpiration) => Db.StringSet(key, value, absoluteExpiration - DateTime.Now);

        public void Update(string key, string value) => Db.StringSet(key, value);

        public void Delete(string key) => Db.KeyDelete(key);

        public bool Exist(string key) => Db.KeyExists(key);

        public long StringIncrement(string key) => Db.StringIncrement(key);

        public double StringIncrement(string key, double incrementNumber) => Db.StringIncrement(key, incrementNumber);
    }
}
