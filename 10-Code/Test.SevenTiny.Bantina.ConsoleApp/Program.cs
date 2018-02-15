using SevenTiny.Bantina;
using System;
using Test.SevenTiny.Bantina.Model;
using static Newtonsoft.Json.JsonConvert;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //School school = new School();
            //Console.WriteLine(SerializeObject(school));

            //bankinate要改成restful风格接口和实现
            var timespan = StopwatchHelper.Caculate(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine(TestConfig.Get("li"));
                }
            });
            Console.WriteLine(timespan.TotalMilliseconds);





            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }
}
