using SevenTiny.Bantina;
using SevenTiny.Bantina.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    public class RedisTest
    {
        public static void Test()
        {
            var result = StopwatchHelper.Caculate(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    IRedisCache redis = RedisManager.Instance;
                    redis.Post("name", $"zhangsan");
                }
            });

            Console.WriteLine(result.TotalMilliseconds);
        }
    }
}
