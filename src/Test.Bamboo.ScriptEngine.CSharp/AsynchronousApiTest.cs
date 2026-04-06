using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class AsynchronousApiTest
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
                public async Task<int> GetValueAsync(int a)
                {
                    await Task.Delay(10);
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetValueAsync";
            script.Parameters = [1];

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
                public static async Task<int> GetValueAsync(int a)
                {
                    await Task.Delay(10);
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetValueAsync";
            script.Parameters = [1];

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
            script.Parameters = [2];

            var engine2 = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = await engine2.ExecuteAsync<int>(script);

            Assert.Equal(2, result.Data);
        }

        [Fact]
        public async Task AsyncCallSyncMethodReturn()
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
            script.Parameters = [1];

            var engine = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = await engine.ExecuteAsync<int>(script);

            Assert.Equal(1, result.Data);
        }
    }
}
