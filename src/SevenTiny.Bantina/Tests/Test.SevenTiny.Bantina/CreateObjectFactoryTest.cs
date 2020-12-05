using SevenTiny.Bantina;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Test.SevenTiny.Bantina
{
    public class ClassA
    {
        public ClassA(ClassB classB) { }
        public int GetInt() => default(int);
    }
    public class ClassB
    {
        public int GetInt() => default(int);
    }

    /// <summary>
    /// 为了测试Expression实现方式和直接New/Emit方式的对比，这里写一个最简单的Demo进行对比
    /// </summary>
    public class ExpressionCreateObjectFactory
    {
        private static Dictionary<string, Func<object[], object>> funcDic = new Dictionary<string, Func<object[], object>>();
        public static T CreateInstance<T>() where T : class
        {
            return CreateInstance(typeof(T), null) as T;
        }

        public static T CreateInstance<T>(params object[] parameters) where T : class
        {
            return CreateInstance(typeof(T), parameters) as T;
        }

        static Expression[] BuildParameters(Type[] parameterTypes, ParameterExpression paramExp)
        {
            List<Expression> list = new List<Expression>();
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                //从参数表达式（参数是：object[]）中取出参数
                var arg = BinaryExpression.ArrayIndex(paramExp, Expression.Constant(i));
                //把参数转化成指定类型
                var argCast = Expression.Convert(arg, parameterTypes[i]);

                list.Add(argCast);
            }
            return list.ToArray();
        }

        public static object CreateInstance(Type instanceType, params object[] parameters)
        {

            Type[] ptypes = new Type[0];
            string key = instanceType.FullName;

            if (parameters != null && parameters.Any())
            {
                ptypes = parameters.Select(t => t.GetType()).ToArray();
                key = string.Concat(key, "_", string.Concat(ptypes.Select(t => t.Name)));
            }

            if (!funcDic.ContainsKey(key))
            {
                ConstructorInfo constructorInfo = instanceType.GetConstructor(ptypes);

                //创建lambda表达式的参数
                var lambdaParam = Expression.Parameter(typeof(object[]), "_args");

                //创建构造函数的参数表达式数组
                var constructorParam = BuildParameters(ptypes, lambdaParam);

                var newExpression = Expression.New(constructorInfo, constructorParam);

                funcDic.Add(key, Expression.Lambda<Func<object[], object>>(newExpression, lambdaParam).Compile());
            }
            return funcDic[key](parameters);
        }
    }

    /// <summary>
    /// 为了测试Expression不带参数兼容性能损耗，特加个最简单的
    /// </summary>
    public class ExpressionCreateObject
    {
        private static Func<object> func;
        public static T CreateInstance<T>() where T : class
        {
            if (func == null)
            {
                var newExpression = Expression.New(typeof(T));
                func = Expression.Lambda<Func<object>>(newExpression).Compile();
            }
            return func() as T;
        }
    }


    public class CreateObjectFactoryTest
    {
        [Fact]
        public void NoArguments()
        {
            ClassB b = CreateObjectFactory.CreateInstance<ClassB>();
            Assert.Equal(default(int), b.GetInt());
        }

        [Fact]
        public void GenerateArgument()
        {
            ClassA a = CreateObjectFactory.CreateInstance<ClassA>(new ClassB());
            Assert.Equal(default(int), a.GetInt());
        }

        [Fact]
        public void ExpressionType()
        {
            var instance = ExpressionCreateObjectFactory.CreateInstance<ClassB>();
            Assert.Equal(default(int), instance.GetInt());
        }


        [Theory]
        [InlineData(10000)]
        [Trait("description", "无参构造各方法调用性能对比")]
        public void PerformanceReportWithNoArguments(int count)
        {
            Trace.WriteLine($"#{count} 次调用:");

            double time = StopwatchHelper.Caculate(count, () =>
            {
                ClassB b = new ClassB();
            }).TotalMilliseconds;
            Trace.WriteLine($"‘New’耗时 {time} milliseconds");

            double time2 = StopwatchHelper.Caculate(count, () =>
            {
                ClassB b = CreateObjectFactory.CreateInstance<ClassB>();
            }).TotalMilliseconds;
            Trace.WriteLine($"‘Emit 工厂’耗时 {time2} milliseconds");

            double time3 = StopwatchHelper.Caculate(count, () =>
            {
                ClassB b = ExpressionCreateObject.CreateInstance<ClassB>();
            }).TotalMilliseconds;
            Trace.WriteLine($"‘Expression’耗时 {time3} milliseconds");

            double time4 = StopwatchHelper.Caculate(count, () =>
            {
                ClassB b = ExpressionCreateObjectFactory.CreateInstance<ClassB>();
            }).TotalMilliseconds;
            Trace.WriteLine($"‘Expression 工厂’耗时 {time4} milliseconds");

            double time5 = StopwatchHelper.Caculate(count, () =>
            {
                ClassB b = Activator.CreateInstance<ClassB>();
                //ClassB b = Activator.CreateInstance(typeof(ClassB)) as ClassB;
            }).TotalMilliseconds;
            Trace.WriteLine($"‘Activator.CreateInstance’耗时 {time5} milliseconds");


            /**
              #1000000 次调用:
                ‘New’耗时 21.7474 milliseconds
                ‘Emit 工厂’耗时 174.088 milliseconds
                ‘Expression’耗时 42.9405 milliseconds
                ‘Expression 工厂’耗时 162.548 milliseconds
                ‘Activator.CreateInstance’耗时 67.3712 milliseconds
             * */
        }

        [Theory]
        [InlineData(10000)]
        [Trait("description", "带参构造各方法调用性能对比")]
        public void PerformanceReportWithArguments(int count)
        {
            Trace.WriteLine($"#{count} 次调用:");

            double time = StopwatchHelper.Caculate(count, () =>
            {
                ClassA a = new ClassA(new ClassB());
            }).TotalMilliseconds;
            Trace.WriteLine($"‘New’耗时 {time} milliseconds");

            double time2 = StopwatchHelper.Caculate(count, () =>
            {
                ClassA a = CreateObjectFactory.CreateInstance<ClassA>(new ClassB());
            }).TotalMilliseconds;
            Trace.WriteLine($"‘Emit 工厂’耗时 {time2} milliseconds");

            double time4 = StopwatchHelper.Caculate(count, () =>
            {
                ClassA a = ExpressionCreateObjectFactory.CreateInstance<ClassA>(new ClassB());
            }).TotalMilliseconds;
            Trace.WriteLine($"‘Expression 工厂’耗时 {time4} milliseconds");

            double time5 = StopwatchHelper.Caculate(count, () =>
            {
                ClassA a = Activator.CreateInstance(typeof(ClassA), new ClassB()) as ClassA;
            }).TotalMilliseconds;
            Trace.WriteLine($"‘Activator.CreateInstance’耗时 {time5} milliseconds");


            /**
              #1000000 次调用:
                ‘New’耗时 29.3612 milliseconds
                ‘Emit 工厂’耗时 634.2714 milliseconds
                ‘Expression 工厂’耗时 620.2489 milliseconds
                ‘Activator.CreateInstance’耗时 588.0409 milliseconds
             * */
        }
    }
}