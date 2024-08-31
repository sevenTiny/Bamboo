using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.Logging;
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

            Assert.Equal("111", result.Data);
        }

        [Trait("desc", "手动注册依赖程序集")]
        [Fact]
        public void DependentAssembly()
        {
            ReferenceManager.RegisterDependentAssembly(typeof(ILogger).Assembly);

            IScriptEngine scriptEngineProvider = new CSharpScriptEngine();

            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using Newtonsoft.Json;
            using Microsoft.Extensions.Logging;

            public class Test
            {
                public int GetA(int a)
                {
                    ILogger logger = null;
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
    }
}
