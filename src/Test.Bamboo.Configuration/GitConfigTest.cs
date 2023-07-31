using Bamboo.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;

namespace Test.Configuration.Git
{
    [ConfigFile("test.json")]
    [ConfigGroup("PublicRepo")]
    public class GitTestConfig : GitConfigBase<GitTestConfig>
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

    [TestClass]
    public class GitConfigTest
    {
        [TestMethod]
        public void Get()
        {
            for (int i = 0; i < 10; i++)
            {
                var config = GitTestConfig.Get();

                Trace.WriteLine(JsonConvert.SerializeObject(config));

                Thread.Sleep(500);
            }
        }

        [TestMethod]
        public void GetNotExistRepoTest()
        {
            var config = GitErrorTestConfig.Get();

            Assert.AreEqual("chuanqi", config.Key);
            Assert.AreEqual("111", config.Value);
        }

        /// <summary>
        /// if you test the local mode, please set the LocalMode = true in appsettings.json
        /// </summary>
        [TestMethod]
        public void GetLocalModeTest()
        {
            var config = LocalModeTestConfig.Get();

            Assert.AreEqual("chuanqi", config.Key);
            Assert.AreEqual("111", config.Value);
        }
    }
}
