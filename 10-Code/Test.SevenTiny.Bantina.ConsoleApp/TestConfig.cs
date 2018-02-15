using SevenTiny.Bantina.Configuration;
using System.Linq;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    [ConfigClass(Name ="Test")]
    public class TestConfig : ConfigBase<TestConfig>
    {
        [ConfigProperty(Name ="key")]
        public string key { get; set; }

        [ConfigProperty(Name ="value")]
        public int value { get; set; }

        public static string Get(string key)
        {
            var config = Configs;
            var result = config.FirstOrDefault(t => t.key.Contains(key));
            var value = result?.value;
            return value?.ToString();
        }
    }
}
