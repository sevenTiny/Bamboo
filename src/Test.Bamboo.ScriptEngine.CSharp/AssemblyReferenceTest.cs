using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class AssemblyReferenceTest
    {
        /// <summary>
        /// Thirdparty package
        /// </summary>
        [Fact]
        public void DownloadPackage()
        {
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

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = scriptEngineProvider.Execute<string>(script);

            Assert.Equal("111", result.Data);
        }

        /// <summary>
        /// Related dlls reference
        /// </summary>
        [Fact]
        public void DependentAssembly()
        {
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

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = scriptEngineProvider.Execute<int>(script);

            Assert.Equal(111, result.Data);
        }
    }
}
