using SevenTiny.Bantina;
using SevenTiny.Bantina.Internationalization;
using SevenTiny.Bantina.Logging;
using SevenTiny.Bantina.Redis;
using System;
using System.Threading.Tasks;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //ILog logger = new LogManager();

            //var result = StopwatchHelper.Caculate(() =>
            //{
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        Console.WriteLine(InternationalContext.Description(1001));
            //        //Console.WriteLine(111);
            //    }
            //});

            //Console.WriteLine(result.TotalMilliseconds);

            IRedisCache redis = RedisManager.Instance;
            redis.Post("name", $"zhangsan");


            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }
}
