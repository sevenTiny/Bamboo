using Bamboo.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Bamboo.Configuration
{
    [ConfigFile("JsonTest.json")]
    public class JsonTestConfig : ConfigBase<JsonTestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [ConfigFile("RuntimeJson.json")]
    public class RuntimeJsonConfig : ConfigBase<RuntimeJsonConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [TestClass]
    public class JsonConfigTest
    {
        [TestMethod]
        public void Test()
        {
            var config = JsonTestConfig.Get();

            Assert.AreEqual("chuanqi", config.Key);
            Assert.AreEqual("111", config.Value);
            Assert.AreEqual("chuanqi", JsonTestConfig.GetValue<string>("Key"));
            Assert.AreEqual("111", JsonTestConfig.GetValue<string>("Value"));
        }

        [TestMethod]
        public void WriteToFile()
        {
            // If file exists, delete firstly
            if (RuntimeJsonConfig.FileExists())
                File.Delete(RuntimeJsonConfig.GetFileFullPath());

            var instance = new RuntimeJsonConfig()
            {
                Key = "111",
                Value = "222"
            };

            //write the configuration instance to file
            instance.WriteToFile();

            Assert.AreEqual("222", RuntimeJsonConfig.GetValue<string>("Value"));
        }
    }
}
