using System;
using System.Linq.Expressions;
using Test.SevenTiny.Bantina.Model;
using Xunit;
using SevenTiny.Bantina.Extensions;

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
}
