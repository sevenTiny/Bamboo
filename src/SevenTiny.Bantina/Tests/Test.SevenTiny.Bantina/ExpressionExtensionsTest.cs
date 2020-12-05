using SevenTiny.Bantina.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Test.SevenTiny.Bantina
{
    public class ExpressionExtensionsTest
    {
        [Fact]
        public void And()
        {
            var testDatas = Student.GetTestData();

            Expression<Func<Student, bool>> func = t => t.Id > 5;

            Func<Student, bool> where1 = func.And(tt => tt.Name.Contains("5")).And(tt => tt.Age < 20).Compile();

            Func<Student, bool> where2 = t => t.Age < 20 && t.Name.Contains("5") && t.Id > 5;

            var result1 = testDatas.Where(where1)?.FirstOrDefault()?.GetName();

            var result2 = testDatas.Where(where2)?.FirstOrDefault()?.GetName();

            Assert.Equal(result1, result2);
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

        public Student(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }

        public string GetName()
        {
            return this.Name;
        }

        internal static IEnumerable<Student> GetTestData()
        {
            foreach (var item in Enumerable.Range(1, 100))
            {
                yield return new Student(item, item.ToString(), item);
            }
        }
    }
}
