using System;
using Test.SevenTiny.Bantina.Model;
using static Newtonsoft.Json.JsonConvert;
using SevenTiny.Configuration;
using Microsoft.Extensions.Configuration;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //School school = new School();
            //Console.WriteLine(SerializeObject(school));

            //bankinate要改成restful风格接口和实现
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(Test.Get("li"));
            }
            

            Console.WriteLine("Hello World!");

            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }
}
