using Newtonsoft.Json;
using Bamboo.Configuration;
using Bamboo.Configuration.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;

namespace Test.Configuration.Core
{
    public class JsonConfigBase<T> : ConfigBase<T> where T : class, new()
    {
        private static string _ConfigName = ConfigNameAttribute.GetName(typeof(T));
        public static T Instance => GetConfig(_ConfigName);

        static JsonConfigBase()
        {
            RegisterGetRemoteFunction(_ConfigName, typeof(T), () =>
              {
                  var fullPath = Path.Combine(AppContext.BaseDirectory, $"{_ConfigName}.json");
                  return JsonConvert.DeserializeObject<T>(File.ReadAllText(fullPath));
              });
        }
    }

    [ConfigName("JsonTest")]
    public class TestConfig : JsonConfigBase<TestConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    [ConfigName("Test")]
    public class TestListConfig : JsonConfigBase<TestListConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    [ConfigName("Test1")]
    public class TestListConfig1 : JsonConfigBase<TestListConfig1>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    [ConfigName("Test2")]
    public class TestListConfig2 : JsonConfigBase<TestListConfig2>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    [ConfigName("Test3")]
    public class TestListConfig3 : JsonConfigBase<TestListConfig3>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    [ConfigName("Test4")]
    public class TestListConfig4 : JsonConfigBase<TestListConfig4>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    [ConfigName("Test5")]
    public class TestListConfig5 : JsonConfigBase<TestListConfig5>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    [ConfigName("Test5")]
    public class TestListConfig55 : JsonConfigBase<TestListConfig55>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }


    public class JsonConfigTest
    {
        [Fact]
        public void Test()
        {
            for (int i = 0; i < 10000; i++)
            {
                var config = TestConfig.Instance;
                var key = config?.Key;
                var value = config?.Value;
                Trace.WriteLine($"{DateTime.Now} read config succeed: key={key},value={value}.");
                Thread.Sleep(3000);
            }
        }
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
    }
}
