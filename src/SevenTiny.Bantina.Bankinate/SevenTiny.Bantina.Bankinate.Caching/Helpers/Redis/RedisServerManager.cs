using StackExchange.Redis;
using System;
using System.Threading;

namespace SevenTiny.Bantina.Bankinate.Caching.Helpers.Redis
{
    internal abstract class RedisServerManager
    {
        protected ConnectionMultiplexer Redis { get; set; }
        protected IDatabase Db { get; set; }
        private string KeySpace { get; set; }

        protected RedisServerManager(string keySpace, string server)
        {
            KeySpace = keySpace;
            Redis = ConnectionMultiplexer.Connect(server);
            Db = Redis.GetDatabase();
        }
    }
}
