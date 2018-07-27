using SevenTiny.Bantina;
using SevenTiny.Bantina.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    public class ConfigurationTest
    {
        public static void Test()
        {
            GetConfigTest();
        }

        public static void GetConfigTest()
        {
            var time = StopwatchHelper.Caculate(10000, () =>
            {
                var result = SevenTinyConfig.Config;

                var result2 = ConnectionStringsConfig.Config;
            });
            Console.WriteLine(time.TotalMilliseconds);
        }

    }

    [ConfigName("ConfigRoot")]
    public class SevenTinyConfig : ConfigBase<SevenTinyConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public static IEnumerable<SevenTinyConfig> Config => Configs;
    }

    [ConfigName("ConnectionStrings")]
    public class ConnectionStringsConfig : ConfigBase<ConnectionStringsConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public static IEnumerable<ConnectionStringsConfig> Config => Configs;
    }
}
