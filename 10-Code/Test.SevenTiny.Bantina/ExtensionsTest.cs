using SevenTiny.Bantina.Extensions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace Test.SevenTiny.Bantina
{
    public class ExtensionsTest
    {
        [Fact]
        public void ExpressionExtensions()
        {
            Expression<Func<Student, bool>> func = t => t.Id == 1;

            func = func.And(tt => tt.Name.Contains("123"));

            func = func.Or(tt => tt.HealthLevel == 2);

            Assert.NotNull(func);
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int GradeId { get; set; }
        public int BodyHigh { get; set; }
        public int HealthLevel { get; set; }

        public string GetName()
        {
            return this.Name;
        }
    }
}
