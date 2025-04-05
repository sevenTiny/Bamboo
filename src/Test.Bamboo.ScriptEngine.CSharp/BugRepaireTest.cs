using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class BugRepaireTest
    {
        private IScriptEngine scriptEngine = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();

        public BugRepaireTest()
        {
            foreach (var item in new[] { "win-x64", "win-x86" })
            {
                var dllPath = Path.Combine(Environment.CurrentDirectory, "runtimes", item);

                if (Directory.Exists(dllPath))
                {
                    Directory.Delete(dllPath, true);
                }
            }
        }

        [Fact]
        public void ExpressionsNotFound()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using System.Linq.Expressions;

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

            var result = scriptEngine.Execute<int>(script);

            Assert.Equal(111, result.Data);
        }
    }
}
