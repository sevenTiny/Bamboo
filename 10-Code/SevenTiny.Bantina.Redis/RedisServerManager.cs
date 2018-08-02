using SevenTiny.Bantina.Logging;
using StackExchange.Redis;
using System;
using System.Threading;

namespace SevenTiny.Bantina.Redis
{
    public abstract class RedisServerManager
    {
        protected ConnectionMultiplexer Redis { get; set; }
        protected IDatabase Db { get; set; }
        private string KeySpace { get; set; }

        protected static ILog log = new LogManager();
        protected RedisServerManager(string keySpace,string server, string port)
        {
            KeySpace = keySpace;
            //set establish retry mechanism (3 times)
            int retryCount = 2;
            while (true)
            {
                try
                {
                    Redis = ConnectionMultiplexer.Connect($"{server}:{port}");
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
                    log.Error($"Redis server {server}:{port} establish  connection error!", ex);
                    throw new TimeoutException($"redis init timeout,server {server}:{port} reject or other.ex{ex.ToString()}");
                }
            }
        }
    }
}
