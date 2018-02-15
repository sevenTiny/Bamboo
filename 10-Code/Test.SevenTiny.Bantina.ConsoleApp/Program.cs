using SevenTiny.Bantina;
using System;
using Test.SevenTiny.Bantina.Model;
using static Newtonsoft.Json.JsonConvert;
using _content = SevenTiny.Bantina.Internationalization.InternationalContext;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //School school = new School();
            //Console.WriteLine(SerializeObject(school));

            //var timespan = StopwatchHelper.Caculate(() =>
            //{
            //    for (int i = 0; i < 100; i++)
            //    {
            //        Console.WriteLine(TestConfig.Get("li"));
            //    }
            //});
            //Console.WriteLine(timespan.TotalMilliseconds);
            var result = _content.Content(1001);
            Console.WriteLine(result);

            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }
}
