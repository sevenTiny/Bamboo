using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.Core;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class CSharpDynamicScriptEngineAsyncTest
    {
        [Fact]
        public async Task AsyncSimpleReturn()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using System.Threading.Tasks;

            public class Test
            {
                public async Task<int> GetAAsync(int a)
                {
                    await Task.Delay(10);
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetAAsync";
            script.Parameters = new object[] { 1 };

            var engine = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = await engine.ExecuteAsync<int>(script);

            Assert.Equal(1, result.Data);
        }

        [Fact]
        public async Task AsyncSimpleStaticMethodReturn()
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
                    await Task.Delay(10);
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetAAsync";
            script.Parameters = new object[] { 1 };

            var engine = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = await engine.ExecuteAsync<int>(script);

            Assert.Equal(1, result.Data);
        }

        [Fact]
        public async Task AsyncValueTaskReturn()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using System.Threading.Tasks;

            public class Test
            {
                public static async ValueTask<int> GetValueAsync(int a)
                {
                    await Task.Delay(10);
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetValueAsync";
            script.Parameters = new object[] { 2 };

            var engine2 = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = await engine2.ExecuteAsync<int>(script);

            Assert.Equal(2, result.Data);
        }

        [Fact]
        public async Task AsyncTimeoutShouldThrow()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using System.Threading.Tasks;

            public class Test
            {
                public static async Task<int> GetA(int a)
                {
                    await Task.Delay(-1);
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 1 };
            script.IsExecutionInSandbox = true;
            script.ExecutionInSandboxMillisecondsTimeout = 500;

            var engine3 = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            await Assert.ThrowsAsync<ScriptEngineException>(async () => await engine3.ExecuteAsync<int>(script));
        }
    }
}
