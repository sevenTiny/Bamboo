using SevenTiny.Bantina;
using System;
using System.Collections.Generic;
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

    public class ObjectHelperTest
    {
        [Fact]
        public void NoArguments()
        {
            ClassB b = ObjectHelper<ClassB>.New();
            Assert.Equal(default(int), b.GetInt());
        }

        [Fact]
        public void GenerateArgument()
        {
            ClassA a = ObjectHelper<ClassA>.New(new ClassB());
            Assert.Equal(default(int), a.GetInt());
        }
    }
}
