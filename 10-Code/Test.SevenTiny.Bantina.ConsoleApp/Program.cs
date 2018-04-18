using SevenTiny.Bantina;
using SevenTiny.Bantina.Internationalization;
using SevenTiny.Bantina.Logging;
using SevenTiny.Bantina.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using SevenTiny.Bantina.Security;
using Test.SevenTiny.Bantina.Model;
using Test.SevenTiny.Bantina.Model.DTO;
using SevenTiny.Bantina.AutoMapper;
using static Newtonsoft.Json.JsonConvert;
using System.Reflection;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            BankinateTest.Test();

            Console.WriteLine("any key to exit ...");
            Console.ReadKey();
        }
    }
}
