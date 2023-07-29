using Bamboo.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace Test.Configuration.Xml
{
    /*
    * 要测试该单元测试请手动在编译生成dll的相同输出目录添加测试配置文件
    * 文件名：XmlTest.xml
    * 格式：
<TestConfig>
	<Key>TestKey</Key>
	<Value>TestValue</Value>
</TestConfig>
    */

    [ConfigFile("XmlTest.xml")]
    public class TestConfig : XmlConfigBase<TestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class TestConfigTest
    {
        [Fact]
        public void Test()
        {
            for (int i = 0; i < 10000; i++)
            {
                var config = TestConfig.Get();

                Trace.WriteLine(JsonConvert.SerializeObject(config));

                Thread.Sleep(500);
            }
        }
    }
}
