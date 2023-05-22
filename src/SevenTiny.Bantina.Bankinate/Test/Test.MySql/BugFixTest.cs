using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.ComponentModel;
using System.Linq;
using Test.Common;
using Test.Common.Model;
using Xunit;

namespace Test.MySql
{
    public class BugFixTest
    {
        [DataBase("SevenTinyTest")]
        private class BugDb : MySqlDbContext<BugDb>
        {
            public BugDb() : base(ConnectionStringHelper.ConnectionString_Write_MySql, ConnectionStringHelper.ConnectionStrings_Read_MySql)
            {
                //不真实持久化
                RealExecutionSaveToDb = false;
            }
        }

        [Fact]
        [Description("修复同字段不同值的，sql和参数生成错误; 修复生成sql语句由于没有括号，逻辑顺序有误")]
        public void Query_BugRepaire1()
        {
            using (var db = new BugDb())
            {
                var re = db.Queryable<OperationTest>().Where(t => t.IntKey == 1 && t.Id != 2 && (t.StringKey.Contains("1") || t.StringKey.Contains("2"))).FirstOrDefault();
                Assert.Equal("SELECT * FROM OperateTest t  WHERE ( 1=1 )  AND  (((t.IntKey = @tIntKey)  AND  (t.Id <> @tId))  AND  ((t.StringKey LIKE @tStringKey)  Or  (t.StringKey LIKE @tStringKey0)))  LIMIT 1", db.SqlStatement);
                Assert.Equal(new[] { "@tIntKey", "@tId", "@tStringKey", "@tStringKey0" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "1", "2", "%1%", "%2%" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        [Description("Guid类型字段参数化查询会带一个单引号的问题，Commit a09b5505")]
        public void Query_GuidFieldParameters()
        {
            using (var db = new BugDb())
            {
                var uid = Guid.Parse("27616d9b-5579-48eb-8d84-8dbc4322ce96");
                var re = db.Queryable<OperationTest>().Where(t => t.GuidKey.Equals(uid)).ToList();
                Assert.Equal("SELECT * FROM OperateTest t  WHERE ( 1=1 )  AND  (t.GuidKey = @tGuidKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tGuidKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "27616d9b-5579-48eb-8d84-8dbc4322ce96" }, db.Parameters.Values.ToArray());//原来的guid字符串里面会嵌套一层''，导致参数化查询bug
            }
        }

        [Fact]
        [Description("重置命令生成器并没有进行赋值，导致命令被重复使用")]
        public void QueryOneFirstThenQueryManyAttachLimit1()
        {
            using (var db = new BugDb())
            {
                var uid = Guid.Parse("27616d9b-5579-48eb-8d84-8dbc4322ce96");
                var re = db.Queryable<OperationTest>().Where(t => t.GuidKey.Equals(uid)).FirstOrDefault();
                var sql1 = db.SqlStatement;

                var re2 = db.Queryable<OperationTest>().Where(t => t.GuidKey.Equals(uid)).ToList();
                var sql2 = db.SqlStatement;
                Assert.NotEqual(sql1, sql2);
            }
        }
    }
}
