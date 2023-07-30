using Bamboo.Configuration;
using Xunit;

namespace Test.Configuration.Json
{
    /*
     * 要测试该单元测试请手动在生成dll的相同输出目录添加测试配置文件
     * 文件名：JsonTest.json
     * 格式：
    {"Key":"chuanqi","Value":"111"}
     */
    [ConfigFile("JsonTest.json")]
    public class TestConfig : JsonConfigBase<TestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class TestConfigTest
    {
        [Fact]
        public void Test()
        {
            var config = TestConfig.Get();

            Assert.Equal("chuanqi", config.Key);
            Assert.Equal("111", config.Value);
        }

        [Fact]
        public void StoreConfigTest()
        {
            //use Bind can bind configuration entry to instance
            var instance = new TestConfig().Bind();

            Assert.Equal("111", instance.Value);

            instance.Key = "write test key";
            instance.Value = "222";

            //write the configuration instance to file
            instance.WriteToFile();

            Assert.Equal("222", instance.Value);

            //if writen to file, call Bind immediately is neccessary.
            //it will rebind the configuration entry to instance
            instance.Value = "333";
            instance.Bind();

            Assert.Equal("222", instance.Value);
        }
    }
}
