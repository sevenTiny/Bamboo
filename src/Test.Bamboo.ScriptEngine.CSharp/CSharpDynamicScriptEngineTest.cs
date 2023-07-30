using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.CSharp;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class CSharpDynamicScriptEngineTest
    {
        [Trait("desc", "多次执行")]
        [Fact]
        public void MultiExecute()
        {
            IScriptEngine scriptEngineProvider = new CSharpScriptEngine();

            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;

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
            script.Parameters = new object[] { 1 };
            //script.IsExecutionInformationCollected = true;//可以输出执行耗时，内存占用

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //结果相加
            int sum = 0;
            for (int i = 0; i < 10000; i++)
            {
                var result = scriptEngineProvider.Execute<int>(script);
                //Trace.WriteLine($"Execute{i} -> IsSuccess:{result.IsSuccess},Data={result.Data},Message={result.Message},TotalMemoryAllocated={result.TotalMemoryAllocated},ProcessorTime={result.ProcessorTime.TotalSeconds}");
                if (result.IsSuccess)
                {
                    sum += result.Data;
                }
            }
            stopwatch.Stop();
            var cos = stopwatch.ElapsedMilliseconds;

            Assert.Equal(10000, sum);
        }

        [Trait("desc", "执行同名不同类的不同方法")]
        [Fact]
        public void RepeatClassExecute()
        {
            IScriptEngine scriptEngineProvider = new CSharpScriptEngine();

            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;

            //先编译A执行A
            script.Script =
            @"
            using System;

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

            var result1 = scriptEngineProvider.Execute<int>(script);

            //编译B执行B
            script.Script =
            @"
            using System;

            public class Test
            {
                public int GetB(int a)
                {
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetB";
            script.Parameters = new object[] { 99999999 };

            var result2 = scriptEngineProvider.Execute<int>(script);

            //再执行A，这次是从B的脚本对应的Hash值去找Test类型，里面并没有A，所以报错没有找到方法A
            //也就是说，用B的脚本去调用A是错误的用法，即便类的名称是一样的，但其实不是一个类
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 333 };

            var result3 = scriptEngineProvider.Execute<int>(script);
        }

        [Trait("desc", "死循环测试")]
        [Fact]
        public void DeadCycile()
        {
            IScriptEngine scriptEngineProvider = new CSharpScriptEngine();

            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;

            public class Test
            {
                public int GetA(int a)
                {
                    for(;;){}
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 111 };
            script.IsExecutionInSandbox = false;     //因为有死循环，所以非信任脚本测试，测试是否会超时
            script.ExecutionInSandboxMillisecondsTimeout = 1000;

            var result = scriptEngineProvider.Execute<int>(script);

            Assert.False(result.IsSuccess);
            Assert.Equal("execution timed out!", result.Message);
        }

        [Trait("desc", "引用类型参数测试")]
        [Fact]
        public void ReferenceArguments()
        {
            IScriptEngine scriptEngineProvider = new CSharpScriptEngine();

            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
            using System;
            using System.Collections.Generic;

            public class Test
            {
                public List<int> GetA(List<int> a)
                {
                    a[0] = 3;
                    return a;
                }
            }
            ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { new List<int> { 1, 2 } };

            var result = scriptEngineProvider.Execute<object>(script);

            DynamicScript script2 = new DynamicScript();
            script2.Language = DynamicScriptLanguage.CSharp;
            script2.Script =
            @"
            using System;
            using System.Collections.Generic;

            public class Test
            {
                public List<int> GetA(List<int> a)
                {
                    return a;
                }
            }
            ";
            script2.ClassFullName = "Test";
            script2.FunctionName = "GetA";
            script2.Parameters = script.Parameters;

            var result2 = scriptEngineProvider.Execute<List<int>>(script2);

            Assert.Equal(3, result2.Data[0]);
            Assert.Equal(3, (script.Parameters[0] as List<int>)[0]);
        }
    }
}
