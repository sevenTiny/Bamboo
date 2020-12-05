using Bamboo.Configuration;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace Test.Configuration.Git
{
    [ConfigName("test")]
    [ConfigSettingUse("PrivateRepo")]
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
            for (int i = 0; i < 1000; i++)
            {
                var config = TestConfig.Instance;
                var key = config.Key;
                var value = config.Value;
                Trace.WriteLine($"key={key},value={value}");
                Thread.Sleep(3000);//为了测试配置重新从远程更新
            }
        }
    }
}
