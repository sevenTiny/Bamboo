using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class DemoTest
    {
        /// <summary>
        /// Execute Sample Script
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
                public int GetValue(int a)
                {
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetValue";
            script.Parameters = new object[] { 111 };

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = scriptEngineProvider.Execute<int>(script);

            Assert.Equal(111, result.Data);
        }

        /// <summary>
        /// Execute Sample Script
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
                public static int GetValue(int a)
                {
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetValue";
            script.Parameters = new object[] { 111 };

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = scriptEngineProvider.Execute<int>(script);

            Assert.Equal(111, result.Data);
        }

        /// <summary>
        /// Execute Sample Script with Async Method
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
                public static async Task<int> GetValueAsync(int a)
                {
                    await Task.Delay(100);
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetValueAsync";
            script.Parameters = new object[] { 111 };
            script.IsExecutionInSandbox = false;

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = await scriptEngineProvider.ExecuteAsync<int>(script);

            Assert.Equal(111, result.Data);
        }
    }
}
