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
    public class ClassD
    {
    }
    public class ClassC
    {
        public ClassC(int a, string b) { }
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

        static Expression[] buildParameters(Type[] parameterTypes, ParameterExpression paramExp)
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
                var constructorParam = buildParameters(ptypes, lambdaParam);

                var newExpression = Expression.New(constructorInfo, constructorParam);

                funcDic.Add(key, Expression.Lambda<Func<object[], object>>(newExpression, lambdaParam).Compile());
            }
            return funcDic[key](parameters);
        }
    }


    /// <summary>
    /// 为了测试Expression代参数兼容性能损耗，特加个最简单的
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
        [InlineData(1000000)]
        public void PerformanceReport(int count)
        {
            Trace.WriteLine($"#{count} 次调用:");

            double time = StopwatchHelper.Caculate(count, () =>
            {
                ClassB b = new ClassB();
                //ClassA a = new ClassA(b);
            }).TotalMilliseconds;
            Trace.WriteLine($"‘New’耗时 {time} milliseconds");

            double time2 = StopwatchHelper.Caculate(count, () =>
            {
                ClassB b = CreateObjectFactory.CreateInstance<ClassB>();
                //ClassA a = CreateObjectFactory.CreateInstance<ClassA>(b);
            }).TotalMilliseconds;
            Trace.WriteLine($"‘Emit 工厂’耗时 {time2} milliseconds");

            double time3 = StopwatchHelper.Caculate(count, () =>
            {
                //ClassB b = ExpressionCreateObjectFactory.CreateInstance<ClassB>();
                ClassB b = ExpressionCreateObject.CreateInstance<ClassB>();
                //ClassA a = ExpressionCreateObjectFactory.CreateInstance<ClassA>(b);
            }).TotalMilliseconds;
            Trace.WriteLine($"‘Expression 工厂’耗时 {time3} milliseconds");

            double time4 = StopwatchHelper.Caculate(count, () =>
            {
                ClassB b = Activator.CreateInstance<ClassB>();
                //ClassA a = Activator.CreateInstance(typeof(ClassA), b) as ClassA;
            }).TotalMilliseconds;
            Trace.WriteLine($"‘Activator.CreateInstance’耗时 {time4} milliseconds");


            /**
              #1000000 次调用:
                ‘New’耗时 25.2717 milliseconds
                ‘Emit 工厂’耗时 157.0825 milliseconds
                ‘Expression 工厂’耗时 147.3184 milliseconds --- 单独执行不带参数的方法耗时：35.7853 milliseconds
                ‘Activator.CreateInstance’耗时 64.6714 milliseconds
             * */
        }
    }
}