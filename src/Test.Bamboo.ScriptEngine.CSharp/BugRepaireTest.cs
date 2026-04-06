using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class BugRepaireTest
    {
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

            var scriptEngine = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
            var result = scriptEngine.Execute<int>(script);

            Assert.Equal(111, result.Data);
        }
    }
}
