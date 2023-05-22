using SevenTiny.Bantina.Bankinate.SqlStatementManagement;
using Test.Common.Model;
using Xunit;

namespace Test.Core
{
    public class LambdaToSqlTest
    {
        [Fact]
        public void And()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.IntKey < 3 && t.IntKey > 1);
            Assert.Equal(" WHERE (t.IntKey < @tIntKey)  AND  (t.IntKey > @tIntKey0)", sql);
        }

        [Fact]
        public void Or()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.IntKey < 3 || t.IntKey > 1);
            Assert.Equal(" WHERE (t.IntKey < @tIntKey)  Or  (t.IntKey > @tIntKey0)", sql);
        }

        [Fact]
        public void LessThan()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.IntKey < 3);
            Assert.Equal(" WHERE t.IntKey < @tIntKey", sql);
        }

        [Fact]
        public void LessThanOrEqual()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.IntKey <= 3);
            Assert.Equal(" WHERE t.IntKey <= @tIntKey", sql);
        }

        [Fact]
        public void GreaterThan()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.IntKey > 3);
            Assert.Equal(" WHERE t.IntKey > @tIntKey", sql);
        }

        [Fact]
        public void GreaterThanOrEqual()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.IntKey >= 3);
            Assert.Equal(" WHERE t.IntKey >= @tIntKey", sql);
        }

        [Fact]
        public void Equal()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.StringKey.Equals("3"));
            Assert.Equal(" WHERE t.StringKey = @tStringKey", sql);
        }

        [Fact]
        public void NotEqual()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.IntKey != 3);
            Assert.Equal(" WHERE t.IntKey <> @tIntKey", sql);
        }

        [Fact]
        public void Contains()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.StringKey.Contains("3"));
            Assert.Equal(" WHERE t.StringKey LIKE @tStringKey", sql);
        }

        [Fact]
        public void StartsWith()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.StringKey.StartsWith("3"));
            Assert.Equal(" WHERE t.StringKey LIKE @tStringKey", sql);
        }

        [Fact]
        public void EndsWith()
        {
            var sql = LambdaToSql.ConvertWhere<OperationTest>(t => t.StringKey.EndsWith("3"));
            Assert.Equal(" WHERE t.StringKey LIKE @tStringKey", sql);
        }

        [Fact]
        public void OrderBy()
        {
            var sql = LambdaToSql.ConvertOrderBy<OperationTest>(t => t.IntKey);
            Assert.Equal("t.IntKey", sql);
        }

        [Fact]
        public void ConvertColumns_1()
        {
            var colums = LambdaToSql.ConvertColumns<OperationTest>(t => t.IntKey)?.ToArray();
            Assert.Equal(new[] { "IntKey" }, colums);
        }

        [Fact]
        public void ConvertColumns_2()
        {
            var colums = LambdaToSql.ConvertColumns<OperationTest>(t => new { t.IntKey, t.StringKey })?.ToArray();
            Assert.Equal(new[] { "IntKey", "StringKey" }, colums);
        }
    }
}
