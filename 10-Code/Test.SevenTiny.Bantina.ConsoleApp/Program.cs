using SevenTiny.Bantina;
using SevenTiny.Bantina.Internationalization;
using SevenTiny.Bantina.Logging;
using System;
using System.Threading.Tasks;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    class Program
    {


        static void Main(string[] args)
        {
            //ILog logger = new LogManager();

            var result = StopwatchHelper.Caculate(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    //Console.WriteLine(InternationalContext.InternationalDescription(122));
                    Console.WriteLine(111);
                }
            });

            Console.WriteLine(result.TotalMilliseconds);

            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }
}
