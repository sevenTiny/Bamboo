using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using Chameleon.Common.Context;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class DataTransmitTest
    {
        /// <summary>
        /// 对象跨脚本传递
        /// </summary>
        [Fact]
        public void DataTransmit()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using Chameleon.Common.Context;

            public class Test
            {
                public string Method()
                {
                    string stringProp = Convert.ToString(ChameleonContext.Current.Get(""StrKey""));
                    int intProp = Convert.ToInt32(ChameleonContext.Current.Get(""IntKey""));
                    return (stringProp+intProp);
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "Method";
            script.IsExecutionInSandbox = false;

            ChameleonContext.Current.Put("StrKey", "888");
            ChameleonContext.Current.Put("IntKey", 999);

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = scriptEngineProvider.Execute<string>(script);

            Assert.Equal("888999", result.Data);
        }

        /// <summary>
        /// 对象跨脚本传递
        /// </summary>
        [Fact]
        public async Task DataTransmitAsync()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using Chameleon.Common.Context;
            using System.Threading.Tasks;

            public class Test
            {
                public async Task<string> MethodAsync()
                {
                    await Task.Delay(10);
                    string stringProp = Convert.ToString(ChameleonContext.Current.Get(""StrKey""));
                    int intProp = Convert.ToInt32(ChameleonContext.Current.Get(""IntKey""));
                    return (stringProp+intProp);
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "MethodAsync";
            script.IsExecutionInSandbox = false;

            ChameleonContext.Current.Put("StrKey", "888");
            ChameleonContext.Current.Put("IntKey", 999);

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = await scriptEngineProvider.ExecuteAsync<string>(script);

            Assert.Equal("888999", result.Data);
        }
    }
}
