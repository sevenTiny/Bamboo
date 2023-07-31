//using SevenTiny.Bantina;
//using Bamboo.Configuration;
//using System.Diagnostics;
//using System.Threading;

//namespace Test.Configuration.MySql
//{
//    /*
//     * 要测试该单元测试请手动在编译生成dll的相同输出目录添加测试配置文件
//     * 文件名：appsettings.json
//     * 格式：
//   {
//	"ConnectionStrings":
//	{
//		"BambooConfig":"server=127.0.0.1;Port=39901;database=BambooConfig;uid=username;pwd=123456;Allow User Variables=true;SslMode=none;"
//	},
//  	"Logging": {
//  	  "IncludeScopes": false,
//  	  "LogLevel": {
//  	    "Default": "Warning"
//  	  }
//  	}
//}
//     * 然后数据库中的结构要按照类的结构设计 MySqlColumnConfigBase 的结构：数据库表字段和实体类属性名一一对应
//     */

//    [ConfigName("Test")]
//    //[AppSettingsConnectionStringKey("BambooConfig")]
//    public class MySqlColumnTestConfig : MySqlColumnConfigBase<MySqlColumnTestConfig>
//    {
//        [ConfigProperty]
//        public string Key { get; set; }
//        [ConfigProperty]
//        public string Value { get; set; }
//    }

//    [TestClass]
//    public class TestConfigTest
//    {
//        [TestMethod]
//        public void Get()
//        {
//            for (int i = 0; i < 1000; i++)
//            {
//                var config = MySqlColumnTestConfig.Instance;
//                var key = config.Key;
//                var value = config.Value;
//                Trace.WriteLine("value=" + value);
//                Thread.Sleep(3000);//为了测试配置重新从远程更新
//            }
//        }

//        [TestMethod]
//        public void Performance()
//        {
//            var timespan = StopwatchHelper.Caculate(1000000, () =>
//            {
//                var configs = MySqlColumnTestConfig.Instance.Value;
//            }).TotalMilliseconds;

//            Trace.WriteLine($"cost time :{timespan} milliseconds.");
//        }
//    }
//}
