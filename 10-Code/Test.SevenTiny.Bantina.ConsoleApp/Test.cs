using SevenTiny.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    public class Test : ConfigBase<Test>
    {
        public string key { get; set; }
        public int value { get; set; }

        public static string Get(string key)
        {
            var config = Configs;
            var result = config.FirstOrDefault(t => t.key.Contains(key));
            var value = result?.value;
            return value?.ToString();
        }

        public override string GetConnectionString()
        {
            return "server=101.201.66.247;Port=39901;database=SevenTinyConfig;uid=root;pwd=CYj(9yyz*8;Allow User Variables=true;";
        }
    }
}
