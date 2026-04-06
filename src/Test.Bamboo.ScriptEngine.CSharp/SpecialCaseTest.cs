using Bamboo.ScriptEngine;
using Bamboo.ScriptEngine.Core;
using Bamboo.ScriptEngine.CSharp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    public class SpecialCaseTest
    {
        /// <summary>
        /// 多次执行
        /// </summary>
        [Fact]
        public void MultiExecute()
        {
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

            var builder = ServiceProviderBuilder.Build();

            //结果相加
            int sum = 0;
            for (int i = 0; i < 10000; i++)
            {
                IScriptEngine scriptEngineProvider2 = builder.GetRequiredService<ICSharpScriptEngine>();

                var result = scriptEngineProvider2.Execute<int>(script);
                //Trace.WriteLine($"Execute{i} -> IsSuccess:{result.IsSuccess},Data={result.Data},Message={result.Message},TotalMemoryAllocated={result.TotalMemoryAllocated},ProcessorTime={result.ProcessorTime.TotalSeconds}");

                sum += result.Data;
            }
            stopwatch.Stop();
            var cos = stopwatch.ElapsedMilliseconds;

            Assert.Equal(10000, sum);
        }

        /// <summary>
        /// 执行同名不同类的不同方法
        /// </summary>
        [Fact]
        public void RepeatClassExecute()
        {
            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();

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
            Assert.Equal(111, result1.Data);

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
            Assert.Equal(99999999, result2.Data);

            //再执行A，这次是从B的脚本对应的Hash值去找Test类型，里面并没有A，所以报错没有找到方法A
            //也就是说，用B的脚本去调用A是错误的用法，即便类的名称是一样的，但其实不是一个类
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 333 };

            Assert.Throws<ScriptEngineException>(() => scriptEngineProvider.Execute<int>(script));
        }

        /// <summary>
        /// 引用类型参数测试
        /// </summary>
        [Fact]
        public void ReferenceArguments()
        {
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

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();
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

        /// <summary>
        /// 抛出异常测试
        /// </summary>
        [Fact]
        public void ThrowExceptionTest()
        {
            DynamicScript script = new DynamicScript();
            script.Language = DynamicScriptLanguage.CSharp;
            script.Script =
            @"
                using System;

                public class Test
                {
                    public int GetA(int a)
                    {
                        throw new ArgumentNullException(""test"");

                        return a;
                    }
                }
                ";
            script.ClassFullName = "Test";
            script.FunctionName = "GetA";
            script.Parameters = new object[] { 111 };

            IScriptEngine scriptEngineProvider = ServiceProviderBuilder.Build().GetRequiredService<ICSharpScriptEngine>();

            Assert.Throws<ArgumentNullException>(() => scriptEngineProvider.Execute<int>(script));
        }
    }
}
