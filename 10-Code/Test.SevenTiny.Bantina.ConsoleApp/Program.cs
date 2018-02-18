using SevenTiny.Bantina;
using SevenTiny.Bantina.Logging;
using System;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    class Program
    {
        

        static void Main(string[] args)
        {
            ILog logger = new LogManager();

            var result = StopwatchHelper.Caculate(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine(i);
                    logger.Info($"88888888888888={i}=");
                }
            });

            Console.WriteLine(result.TotalMilliseconds);
            

            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }
}
