using Bamboo.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace Test.Configuration.Git
{
    [ConfigFile("test.json")]
    [ConfigGroup("PublicRepo")]
    public class TestConfig : GitConfigBase<TestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class TestConfigTest
    {
        [Fact]
        public void Get()
        {
            for (int i = 0; i < 10000; i++)
            {
                var config = TestConfig.Get();

                Trace.WriteLine(JsonConvert.SerializeObject(config));

                Thread.Sleep(500);
            }
        }
    }
}
