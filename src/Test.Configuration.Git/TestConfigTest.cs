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

    [ConfigFile("GitErrorTest.json")]
    [ConfigGroup("NotExistRepo")]
    public class GitErrorTestConfig : GitConfigBase<GitErrorTestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// if you test the local mode, please set the LocalMode = true in appsettings.json
    /// </summary>
    [ConfigFile("GitErrorTest.json")]
    [ConfigGroup("PublicRepo")]
    public class LocalModeTestConfig : GitConfigBase<LocalModeTestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class TestConfigTest
    {
        [Fact]
        public void Get()
        {
            for (int i = 0; i < 10; i++)
            {
                var config = TestConfig.Get();

                Trace.WriteLine(JsonConvert.SerializeObject(config));

                Thread.Sleep(500);
            }
        }

        [Fact]
        public void GetNotExistRepoTest()
        {
            var config = GitErrorTestConfig.Get();

            Assert.Equal("chuanqi", config.Key);
            Assert.Equal("111", config.Value);
        }

        /// <summary>
        /// if you test the local mode, please set the LocalMode = true in appsettings.json
        /// </summary>
        [Fact]
        public void GetLocalModeTest()
        {
            var config = LocalModeTestConfig.Get();

            Assert.Equal("chuanqi", config.Key);
            Assert.Equal("111", config.Value);
        }
    }
}
