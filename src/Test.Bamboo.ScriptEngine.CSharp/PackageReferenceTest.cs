using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using System.Diagnostics;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class PackageReferenceTest
    {
        [Trait("desc", "下载第三方包")]
        [Fact]
        public void DownloadPackage()
        {
            IScriptEngine scriptEngineProvider = new CSharpScriptEngine();

            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using Newtonsoft.Json;

            public class Test
            {
                public string GetA(int a)
                {
                    return JsonConvert.SerializeObject(a);
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 111 };
            script.IsExecutionInSandbox = false;

            var result = scriptEngineProvider.Execute<string>(script);

            Assert.True(result.IsSuccess);
            Assert.Equal("111", result.Data);
        }
    }
}
