using Bamboo.Configuration;

namespace Test.Configuration.Xml
{
    /*
    * 要测试该单元测试请手动在编译生成dll的相同输出目录添加测试配置文件
    * 文件名：XmlTest.xml
    * 格式：
<TestConfig>
	<Key>chuanqi</Key>
	<Value>111</Value>
</TestConfig>
    */

    [ConfigFile("XmlTest.xml")]
    public class XmlTestConfig : XmlConfigBase<XmlTestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [TestClass]
    public class XmlConfigTest
    {
        [TestMethod]
        public void Test()
        {
            var config = XmlTestConfig.Get();

            Assert.AreEqual("chuanqi", config.Key);
            Assert.AreEqual("111", config.Value);
        }

        [TestMethod]
        public void StoreConfigTest()
        {
            //use Bind can bind configuration entry to instance
            var instance = new XmlTestConfig().Bind();

            Assert.AreEqual("111", instance.Value);

            instance.Value = "222";

            //write the configuration instance to file
            instance.WriteToFile();

            Assert.AreEqual("222", instance.Value);

            //if writen to file, call Bind immediately is neccessary.
            //it will rebind the configuration entry to instance
            instance.Value = "333";
            instance.Bind();

            Assert.AreEqual("222", instance.Value);

            //roll back
            instance.Value = "111";
            instance.WriteToFile();
        }
    }
}
