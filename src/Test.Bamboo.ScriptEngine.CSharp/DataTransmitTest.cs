using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using Chameleon.Common.Context;
using System.Diagnostics;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class DataTransmitTest
    {
        [Trait("desc", "对象跨脚本传递")]
        [Fact]
        public void DownloadPackage()
        {
            IScriptEngine scriptEngineProvider = new CSharpScriptEngine();

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
                    string stringProp = ChameleonContext.Current.Get(""StrKey"");
                    int intProp = ChameleonContext.Current.Get(""IntKey"");
                    return (stringProp+intProp);
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "Method";
            script.IsExecutionInSandbox = false;

            ChameleonContext.Current.Put("StrKey", "888");
            ChameleonContext.Current.Put("IntKey", 999);

            var result = scriptEngineProvider.Execute<string>(script);

            Assert.True(result.IsSuccess);
            Assert.Equal("888999", result.Data);
        }
    }
}
