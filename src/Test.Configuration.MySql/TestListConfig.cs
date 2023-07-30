using Newtonsoft.Json;
using SevenTiny.Bantina;
using Bamboo.Configuration;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace Test.Configuration.MySql
{
    /*
     * 要测试该单元测试请手动在编译生成dll的相同输出目录添加测试配置文件
     * 文件名：appsettings.json
     * 格式：
{
	"ConnectionStrings":
	{
		"BambooConfig":"server=127.0.0.1;Port=39901;database=BambooConfig;uid=username;pwd=123456;Allow User Variables=true;SslMode=none;"
	},
  	"Logging": {
  	  "IncludeScopes": false,
  	  "LogLevel": {
  	    "Default": "Warning"
  	  }
  	}
}

     * 然后数据库中的结构要按照类的结构设计 MySqlRowConfigBase 的结构：数据库表字段和实体类属性名一一对应 但Instance是List结构
     */

    [ConfigName("Test")]
    public class TestListConfig : MySqlRowConfigBase<TestListConfig>
    {
        [ConfigProperty]
        public string Key { get; set; }
        [ConfigProperty]
        public string Value { get; set; }
    }
    [ConfigName("Test1")]
    public class TestListConfig1 : MySqlRowConfigBase<TestListConfig1>
    {
        [ConfigProperty]
        public string Key { get; set; }
        [ConfigProperty]
        public string Value { get; set; }
    }
    [ConfigName("Test2")]
    public class TestListConfig2 : MySqlRowConfigBase<TestListConfig2>
    {
        [ConfigProperty]
        public string Key { get; set; }
        [ConfigProperty]
        public string Value { get; set; }
    }
    [ConfigName("Test3")]
    public class TestListConfig3 : MySqlRowConfigBase<TestListConfig3>
    {
        [ConfigProperty]
        public string Key { get; set; }
        [ConfigProperty]
        public string Value { get; set; }
    }
    [ConfigName("Test4")]
    public class TestListConfig4 : MySqlRowConfigBase<TestListConfig4>
    {
        [ConfigProperty]
        public string Key { get; set; }
        [ConfigProperty]
        public string Value { get; set; }
    }
    [ConfigName("Test5")]
    public class TestListConfig5 : MySqlRowConfigBase<TestListConfig5>
    {
        [ConfigProperty]
        public string Key { get; set; }
        [ConfigProperty]
        public string Value { get; set; }
    }
    [ConfigName("Test5")]
    public class TestListConfig55 : MySqlRowConfigBase<TestListConfig55>
    {
        [ConfigProperty]
        public string Key { get; set; }
        [ConfigProperty]
        public string Value { get; set; }
    }

    public class TestListConfigTest
    {
        [Fact]
        public void Get()
        {
            for (int i = 0; i < 1000000; i++)
            {
                var config = TestListConfig.Instance;
                Trace.WriteLine("config=" + JsonConvert.SerializeObject(config));
                var config1 = TestListConfig1.Instance;
                Trace.WriteLine("config1=" + JsonConvert.SerializeObject(config1));
                var config2 = TestListConfig2.Instance;
                Trace.WriteLine("config2=" + JsonConvert.SerializeObject(config2));
                var config3 = TestListConfig3.Instance;
                Trace.WriteLine("config3=" + JsonConvert.SerializeObject(config3));
                var config4 = TestListConfig4.Instance;
                Trace.WriteLine("config4=" + JsonConvert.SerializeObject(config4));
                var config5 = TestListConfig5.Instance;
                Trace.WriteLine("config5=" + JsonConvert.SerializeObject(config5));
                var config55 = TestListConfig55.Instance;
                Trace.WriteLine("config55=" + JsonConvert.SerializeObject(config55));

                Thread.Sleep(1000);//为了测试配置重新从远程更新
            }
        }

        [Fact]
        public void Performance()
        {
            var timespan = StopwatchHelper.Caculate(1000000, () =>
            {
                var configs = TestListConfig.Instance;
            }).TotalMilliseconds;

            Trace.WriteLine($"cost time :{timespan} milliseconds.");
        }
    }
}
