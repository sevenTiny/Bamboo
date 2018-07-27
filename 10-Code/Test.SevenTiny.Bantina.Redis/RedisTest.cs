using SevenTiny.Bantina;
using SevenTiny.Bantina.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    public class RedisTest
    {
        [Fact]
        public void Test()
        {
            var result = StopwatchHelper.Caculate(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    IRedisCache redis = RedisCacheManager.Instance;
                    redis.Post("name", $"zhangsan");
                }
            });

            Console.WriteLine(result.TotalMilliseconds);
        }
    }
}
