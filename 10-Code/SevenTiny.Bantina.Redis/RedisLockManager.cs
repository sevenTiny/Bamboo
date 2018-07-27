using SevenTiny.Bantina.Logging;
using StackExchange.Redis;
using System;

namespace SevenTiny.Bantina.Redis
{
    /// <summary>
    /// Redis Lock Manager
    /// </summary>
    public class RedisLockManager : IRedisLock
    {
        private ConnectionMultiplexer Redis { get; set; }

        private IDatabase Db { get; set; }

        private static readonly RedisValue Token = Environment.MachineName;

        private TimeSpan LockExpirySeconds => TimeSpan.FromSeconds(double.Parse(RedisConfig.Get("RedisLockExpirySeconds")));

        private static IRedisLock _instance;

        /// <summary>
        /// lock
        /// </summary>
        private readonly static object lockObject = new object();

        /// <summary>
        /// Singleton pattern Instance
        /// </summary>
        public static IRedisLock Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RedisLockManager();
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

        private RedisLockManager()
        {
            try
            {
                Redis = ConnectionMultiplexer.Connect($"{RedisConfig.Get("Server")}:{RedisConfig.Get("Port")}");
                Db = Redis.GetDatabase();
            }
            catch (Exception ex)
            {
                log.Error("Redis server connection error!", ex);
            }
        }

        /// <summary>
        /// Redis locker
        /// </summary>
        /// <param name="lockName">lockName，no repeat</param>
        /// <param name="accessLockMethod">success into lock execute method</param>
        /// <param name="refuseMethod">refuse into lock execute method</param>
        public void Lock(string lockName, Action accessLockMethod, Action refuseMethod)
        {
            if (Db.LockTake("key", Token, LockExpirySeconds))
            {
                try
                {
                    accessLockMethod();
                }
                catch (Exception ex)
                {
                    log.Error($"redis lock method block exception.ex:{ex.ToString()}");
                    throw ex;
                }
                finally
                {
                    Db.LockRelease("key", Token);
                }
            }
            else
            {
                refuseMethod();
            }
        }

        /// <summary>
        /// Redis locker
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lockName">lockName，no repeat</param>
        /// <param name="accessLockMethod">success into lock execute method</param>
        /// <param name="refuseMethod">refuse into lock execute method</param>
        /// <returns></returns>
        public T Lock<T>(string lockName, Func<T> accessLockMethod, Func<T> refuseMethod)
        {
            if (Db.LockTake("key", Token, LockExpirySeconds))
            {
                try
                {
                    return accessLockMethod();
                }
                catch (Exception ex)
                {
                    log.Error($"redis lock method block exception.ex:{ex.ToString()}");
                    throw ex;
                }
                finally
                {
                    Db.LockRelease("key", Token);
                }
            }
            else
            {
                return refuseMethod();
            }
        }
    }
}
