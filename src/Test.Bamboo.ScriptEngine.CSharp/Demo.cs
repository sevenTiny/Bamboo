﻿using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class Demo
    {
        private IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();

        [Trait("desc", "执行受信任的脚本 execute trasted code")]
        [Fact]
        public void Execute()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;

            public class Test
            {
                public int GetA(int a)
                {
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 111 };
            script.IsExecutionInSandbox = false;

            var result = scriptEngineProvider.Execute<int>(script);

            Assert.Equal(111, result.Data);
        }

        [Trait("desc", "执行不受信任的脚本 execute untrasted code")]
        [Fact]
        public void ExecuteUntrastedCode()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;

            public class Test
            {
                public int GetC(int a)
                {
                    int c = 0;
                    for(; ; )
                    {
                           c += 1;
                    }
                    return c;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetC";
            script.Parameters = new object[] { 1 };
            script.IsExecutionInSandbox = true;                    //沙箱环境执行
            script.ExecutionInSandboxMillisecondsTimeout = 100;     //沙箱环境执行超时时间

            var result = scriptEngineProvider.Execute<int>(script);

            Assert.Equal("execution timed out!", result.Message);
        }
    }
}
