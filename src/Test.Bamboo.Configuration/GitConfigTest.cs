using Bamboo.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Test.Configuration.Git
{
    [ConfigFile("test.json")]
    public class GitTestConfig : GitConfigBase<GitTestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [ConfigFile("test.json")]
    public class Git2TestConfig : GitConfigBase<Git2TestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [ConfigFile("LocalModeTest.json")]
    public class LocalModeTestConfig : GitConfigBase<LocalModeTestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [TestClass]
    public class GitConfigTest
    {
        /// <summary>
        /// if you test git download ability, please un-comment [TestMethod]
        /// and ensure the local mode is disable
        /// </summary>
        //[TestMethod]
        public void RemoteDebug()
        {
            for (int i = 0; i < 100; i++)
            {
                var config = GitTestConfig.Get();

                Trace.WriteLine(JsonConvert.SerializeObject(config));

                // add second config to test when many config instance use git base, only download once
                var config2 = Git2TestConfig.Get();

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// if you test the local mode, please set the LocalMode = true in appsettings.json
        /// </summary>
        [TestMethod]
        public void GetLocalModeTest()
        {
            // ensure it is local mode before local mode test
            Assert.AreEqual(true, AppSettingsConfig.IsLocalMode());

            var config = LocalModeTestConfig.Get();

            Assert.AreEqual("chuanqi", config.Key);
            Assert.AreEqual("111", config.Value);
        }
    }
}
