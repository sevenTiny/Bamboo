using SevenTiny.Bantina.Logging;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading;

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

        /// <summary>
        /// Redis locker
        /// </summary>
        /// <param name="lockName">lockName，no repeat</param>
        /// <param name="accessLockMethod">success into lock execute method</param>
        /// <param name="refuseMethod">refuse into lock execute method</param>
        public void Lock(string lockName, Action accessLockMethod, Action refuseMethod)
        {
            if (Db.LockTake(lockName, Token, LockExpirySeconds))
            {
                try
                {
                    accessLockMethod();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    LockRelease(lockName);
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
            if (Db.LockTake(lockName, Token, LockExpirySeconds))
            {
                try
                {
                    return accessLockMethod();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    LockRelease(lockName);
                }
            }
            else
            {
                return refuseMethod();
            }
        }

        /// <summary>
        /// Lock
        /// </summary>
        /// <param name="lockName"></param>
        /// <returns></returns>
        public bool Lock(string lockName)
        {
            //retry 3 times
            int retryTime = 2;
            while (true)
            {
                try
                {
                    var result = Db.LockTake(lockName, Token, LockExpirySeconds);
                    ;
                    if (!result && retryTime > 0)
                    {
                        retryTime--;
                        Thread.Sleep(3000);
                        continue;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    if (retryTime > 0)
                    {
                        retryTime--;
                        Thread.Sleep(3000);
                        continue;
                    }
                    log.Error($"redis lock release error,ex:{ex.ToString()}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Release Lock
        /// </summary>
        /// <param name="lockName"></param>
        public bool LockRelease(string lockName)
        {
            //retry 3 times
            int retryTime = 2;
            while (true)
            {
                try
                {
                    var result = Db.LockRelease(lockName, Token);
                    ;
                    if (!result && retryTime > 0)
                    {
                        retryTime--;
                        Thread.Sleep(3000);
                        continue;
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    if (retryTime > 0)
                    {
                        retryTime--;
                        Thread.Sleep(3000);
                        continue;
                    }
                    log.Error($"redis lock release error,ex:{ex.ToString()}");
                    throw ex;
                }
            }
        }
    }
}
