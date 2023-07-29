﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Bamboo.Configuration;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;

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
            for (int i = 0; i < 10000; i++)
            {
                var config = TestConfig.Get();

                Trace.WriteLine(JsonConvert.SerializeObject(config));

                Thread.Sleep(500);
            }
        }
    }
}
