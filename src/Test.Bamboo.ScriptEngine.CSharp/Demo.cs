using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.Core;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class Demo
    {
        /// <summary>
        /// 执行受信任的脚本 execute trasted code
        /// </summary>
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

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = scriptEngineProvider.Execute<int>(script);

            Assert.Equal(111, result.Data);
        }

        /// <summary>
        /// 执行不受信任的脚本 execute untrasted code
        /// </summary>
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

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();

            Assert.Throws<ScriptEngineException>(() => scriptEngineProvider.Execute<int>(script));
        }

        /// <summary>
        /// 执行静态方法脚本
        /// </summary>
        [Fact]
        public void ExecuteStaticMethod()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;

            public class Test
            {
                public static int GetA(int a)
                {
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 111 };
            script.IsExecutionInSandbox = false;

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = scriptEngineProvider.Execute<int>(script);

            Assert.Equal(111, result.Data);
        }

        /// <summary>
        /// 执行静态方法脚本
        /// </summary>
        [Fact]
        public async Task ExecuteAsyncMethod()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using System.Threading.Tasks;

            public class Test
            {
                public static async Task<int> GetAAsync(int a)
                {
                    await Task.Delay(100);
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetAAsync";
            script.Parameters = new object[] { 111 };
            script.IsExecutionInSandbox = false;

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = await scriptEngineProvider.ExecuteAsync<int>(script);

            Assert.Equal(111, result.Data);
        }
    }
}
