using SevenTiny.Bantina;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
    public class ClassC
    {
        public ClassC(int a, string b) { }
        public int GetInt() => default(int);
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

        [Theory]
        [InlineData(1000000)]
        public void PerformanceReport(int count)
        {
            double time = StopwatchHelper.Caculate(count, () =>
            {
                ClassB b = new ClassB();

            }).TotalMilliseconds;
            Trace.WriteLine($"‘直接实例化’{count} 次调用耗时 {time} milliseconds");//‘直接实例化’1000000 次调用耗时 23.8736 milliseconds

            double time2 = StopwatchHelper.Caculate(count, () =>
            {
                ClassB b = CreateObjectFactory.CreateInstance<ClassB>();
            }).TotalMilliseconds;
            Trace.WriteLine($"‘工厂创建’{count} 次调用耗时 {time2} milliseconds");//‘工厂创建’1000000 次调用耗时 149.5811 milliseconds
        }
    }
}