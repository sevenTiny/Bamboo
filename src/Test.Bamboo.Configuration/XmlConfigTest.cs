using Bamboo.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Bamboo.Configuration
{
    [ConfigFile("XmlTest.xml")]
    public class XmlTestConfig : ConfigBase<XmlTestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [ConfigFile("RuntimeXml.xml")]
    public class RuntimeXmlConfig : ConfigBase<RuntimeXmlConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [TestClass]
    public class XmlConfigTest
    {
        [TestMethod]
        public void Get()
        {
            var config = XmlTestConfig.Get();

            Assert.AreEqual("chuanqi", config.Key);
            Assert.AreEqual("111", config.Value);
            Assert.AreEqual("chuanqi", XmlTestConfig.GetValue<string>("Key"));
            Assert.AreEqual("111", XmlTestConfig.GetValue<string>("Value"));
        }

        [TestMethod]
        public void WriteToFile()
        {
            // If file exists, delete firstly
            if (RuntimeXmlConfig.FileExists())
                File.Delete(RuntimeXmlConfig.GetFileFullPath());

            var instance = new RuntimeXmlConfig()
            {
                Key = "111",
                Value = "222"
            };

            //write the configuration instance to file
            instance.WriteToFile();

            Assert.AreEqual("222", RuntimeXmlConfig.GetValue<string>("Value"));
        }
    }
}
