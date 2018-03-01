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

namespace SevenTiny.Bantina.Redis
{
    public class RedisManager : IRedisCache
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
                            _instance = new RedisManager();
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

        private RedisManager()
        {
            try
            {
                Redis = ConnectionMultiplexer.Connect($"{RedisConfig.Default.Server}:{RedisConfig.Default.Port}");
                Db = Redis.GetDatabase();
            }
            catch (Exception ex)
            {
                log.Error("Redis server connection error!", ex);
            }
        }

        public string Get(string key) => SafeProcess(() => { return Db.StringGet(key); });

        public void Post(string key, string value) => SafeProcess(() => { Db.StringSet(key, value, TimeSpan.FromMinutes(30)); });

        public void Post(string key, string value, TimeSpan absoluteExpirationRelativeToNow) => SafeProcess(() => { Db.StringSet(key, value, absoluteExpirationRelativeToNow); });

        public void Post(string key, string value, DateTime absoluteExpiration) => SafeProcess(() => { Db.StringSet(key, value, absoluteExpiration - DateTime.Now); });

        public void Put(string key, string value) => SafeProcess(() => { Db.StringSet(key, value); });

        public void Delete(string key) => SafeProcess(() => { Db.KeyDelete(key); });

        public bool Exist(string key) => SafeProcess(() => { return Db.KeyExists(key); });

        #region SafeProcess
        private void SafeProcess(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                log.Error("redis server error!", ex);
            }
        }

        private T SafeProcess<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                log.Error("redis server error!", ex);
            }
            return default(T);
        }
        #endregion
    }
}
