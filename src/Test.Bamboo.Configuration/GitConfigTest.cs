using Bamboo.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Test.Bamboo.Configuration
{
    [ConfigFile("test.json")]
    public class GitTestConfig : ConfigBase<GitTestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [ConfigFile("subfolder\\sub_user_info.json")]
    public class SubUserInfoConfig : ConfigBase<SubUserInfoConfig>
    {
        public string Name { get; set; }
        public string Age { get; set; }
    }

    [TestClass]
    public class GitConfigTest
    {
        /// <summary>
        /// To Debug fetch logic
        /// </summary>
        //[TestMethod]
        public void Debug()
        {
            for (int i = 0; i < 100; i++)
            {
                var config = GitTestConfig.Get();

                Trace.WriteLine(JsonConvert.SerializeObject(config));

                Thread.Sleep(1000);
            }
        }

        [TestMethod]
        public void GetTest()
        {
            var config = GitTestConfig.Get();

            Assert.AreEqual("chuanqi", config.Key);
            Assert.AreEqual("333", config.Value);
            Assert.AreEqual("chuanqi", GitTestConfig.GetValue<string>("Key"));
            Assert.AreEqual("333", GitTestConfig.GetValue<string>("Value"));
        }

        [TestMethod]
        public void GetUserInfo()
        {
            var config = SubUserInfoConfig.Get();

            Assert.AreEqual("john", config.Name);
            Assert.AreEqual("13", config.Age);
            Assert.AreEqual("john", SubUserInfoConfig.GetValue<string>("Name"));
            Assert.AreEqual("13", SubUserInfoConfig.GetValue<string>("Age"));
        }
    }
}
