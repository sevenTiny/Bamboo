using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading;

namespace SevenTiny.Bantina.Redis
{
    /// <summary>
    /// Redis Lock Manager
    /// </summary>
    public class RedisLockManager : RedisServerManager, IRedisLock
    {
        private static readonly RedisValue Token = Environment.MachineName;
        private TimeSpan LockExpirySeconds { get; set; }

        public RedisLockManager(string keySpace)
            : base(keySpace, RedisConfig.Get(keySpace, "Server"), RedisConfig.Get(keySpace, "Port"))
        {
            LockExpirySeconds = TimeSpan.FromSeconds(Convert.ToDouble(RedisConfig.Get(keySpace, "RedisLockExpirySeconds")));
        }

        public RedisLockManager(string server, string port, int lockExpirySeconds = 600)
            : base("Default", server, port)
        {
            LockExpirySeconds = TimeSpan.FromSeconds(lockExpirySeconds);
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
                    log.LogError($"redis lock release error,ex:{ex.ToString()}");
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
                    log.LogError($"redis lock release error,ex:{ex.ToString()}");
                    throw ex;
                }
            }
        }
    }
}
