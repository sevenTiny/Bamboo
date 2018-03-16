using SevenTiny.Bantina;
using SevenTiny.Bantina.Internationalization;
using SevenTiny.Bantina.Logging;
using SevenTiny.Bantina.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using SevenTiny.Bantina.Security;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //ILog logger = new LogManager();


            //IRedisCache redis = RedisManager.Instance;
            //redis.Post("name", $"zhangsan");

            //var result = StopwatchHelper.Caculate(() =>
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        IRedisCache redis = RedisManager.Instance;
            //        redis.Post("name", $"zhangsan");
            //    }
            //});

            //Console.WriteLine(result.TotalMilliseconds);

            Console.WriteLine(MD5.GetMd5Hash("123456"));

            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }
}
