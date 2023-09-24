using Bamboo.Configuration;

namespace Test.Configuration.Json
{
    /*
     * 要测试该单元测试请手动在生成dll的相同输出目录添加测试配置文件
     * 文件名：JsonTest.json
     * 格式：
    {"Key":"chuanqi","Value":"111"}
     */
    [ConfigFile("JsonTest.json")]
    public class JsonTestConfig : JsonConfigBase<JsonTestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [ConfigFile("NotExistJsonConfigFile.json")]
    public class NotExistJsonConfigFileConfig : JsonConfigBase<NotExistJsonConfigFileConfig>
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
        }

        [TestMethod]
        public void StoreConfigTest()
        {
            //use Bind can bind configuration entry to instance
            var instance = new JsonTestConfig().Bind();

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

        [TestMethod]
        public void AppSettingsConfigTest()
        {
            Assert.AreEqual("123", AppSettingsConfig.GetValue<string>("Key1"));
        }

        [TestMethod]
        public void NotExistFileTest()
        {
            var exist = NotExistJsonConfigFileConfig.FileExists();

            Assert.AreEqual(false, exist);
        }

        [TestMethod]
        public void GetConfigurationFileFullPathTest()
        {
            var path = NotExistJsonConfigFileConfig.GetFileFullPath();

            Assert.AreNotEqual(string.Empty, path);
        }
    }
}
