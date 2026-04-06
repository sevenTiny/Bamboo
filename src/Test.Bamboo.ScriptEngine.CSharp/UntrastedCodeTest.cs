using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.Core;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class UntrastedCodeTest
    {
        /// <summary>
        /// Execute in sandbox
        /// </summary>
        [Fact]
        public void ExecuteInSandbox()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;

            public class Test
            {
                public int GetValue(int a)
                {
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetValue";
            script.Parameters = new object[] { 1 };
            script.IsExecutionInSandbox = true;            //沙箱环境执行
            script.SandboxExecutionTimeoutMilliseconds = 100;     //沙箱环境执行超时时间

            IScriptEngine scriptEngine = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = scriptEngine.Execute<int>(script);

            Assert.Equal(1, result.Data);
        }

        /// <summary>
        /// Depp cycle execution test
        /// </summary>
        [Fact]
        public void ExecuteDeepCycle()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;

            public class Test
            {
                public int GetValue(int a)
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
            script.FunctionName = "GetValue";
            script.Parameters = new object[] { 1 };
            script.IsExecutionInSandbox = true;                 //沙箱环境执行
            script.SandboxExecutionTimeoutMilliseconds = 100;   //沙箱环境执行超时时间

            IScriptEngine scriptEngine = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();

            Assert.Throws<ScriptEngineException>(() => scriptEngine.Execute<int>(script));
        }
    }
}
