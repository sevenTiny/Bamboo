using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using System;
using System.IO;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class BugRepaireTest
    {
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
            IScriptEngine scriptEngineProvider = new CSharpScriptEngine();

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

            var result = scriptEngineProvider.Execute<int>(script);

            Assert.Equal(111, result.Data);
        }
    }
}
