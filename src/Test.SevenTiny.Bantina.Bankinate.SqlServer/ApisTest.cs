using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Linq;
using Test.Common;
using Test.Common.Model;
using Xunit;

namespace Test.SqlServer
{
    /// <summary>
    /// 关系型数据库查询测试
    /// </summary>
    public class ApisTest
    {
        [DataBase("SevenTinyTest")]
        private class ApiDb : SqlServerDbContext<ApiDb>
        {
            public ApiDb() : base(ConnectionStringHelper.ConnectionString_Write_SqlServer, ConnectionStringHelper.ConnectionStrings_Read_SqlServer)
            {
                //不真实持久化
                RealExecutionSaveToDb = false;
            }
        }

        [Fact]
        public void Add()
        {
            using (var db = new ApiDb())
            {
                db.Add(new OperationTest
                {
                    Id = 1,
                    Key2 = 1,
                    IntKey = 1,
                    StringKey = "1",
                    IntNullKey = null,
                    FloatNullKey = 1,
                    DoubleNullKey = 1,
                    DateNullKey = DateTime.MinValue,
                    DateTimeNullKey = null
                });

                Assert.Equal("INSERT INTO OperateTest (Key2,StringKey,IntKey,IntNullKey,DateNullKey,DateTimeNullKey,DoubleNullKey) VALUES (@Key2,@StringKey,@IntKey,@IntNullKey,@DateNullKey,@DateTimeNullKey,@DoubleNullKey)", db.SqlStatement);
                Assert.Equal(new[] { "@Key2", "@StringKey", "@IntKey", "@IntNullKey", "@DateNullKey", "@DateTimeNullKey", "@DoubleNullKey" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void AddAsync()
        {
            using (var db = new ApiDb())
            {
                var a = db.AddAsync(new OperationTest
                {
                    Id = 1,
                    Key2 = 1,
                    IntKey = 1,
                    StringKey = "1",
                    IntNullKey = null,
                    FloatNullKey = 1,
                    DoubleNullKey = 1,
                    DateNullKey = DateTime.MinValue,
                    DateTimeNullKey = null
                });

                Assert.Equal("INSERT INTO OperateTest (Key2,StringKey,IntKey,IntNullKey,DateNullKey,DateTimeNullKey,DoubleNullKey) VALUES (@Key2,@StringKey,@IntKey,@IntNullKey,@DateNullKey,@DateTimeNullKey,@DoubleNullKey)", db.SqlStatement);
                Assert.Equal(new[] { "@Key2", "@StringKey", "@IntKey", "@IntNullKey", "@DateNullKey", "@DateTimeNullKey", "@DoubleNullKey" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void Update()
        {
            using (var db = new ApiDb())
            {
                db.Update(new OperationTest
                {
                    Id = 1,
                    Key2 = 1,
                    IntKey = 1,
                    StringKey = "1",
                    IntNullKey = null,
                    FloatNullKey = 1,
                    DoubleNullKey = 1,
                    DateNullKey = DateTime.MinValue,
                    DateTimeNullKey = null
                });

                Assert.Equal("UPDATE t SET Key2=@tKey2,StringKey=@tStringKey,IntKey=@tIntKey,IntNullKey=@tIntNullKey,DateNullKey=@tDateNullKey,DateTimeNullKey=@tDateTimeNullKey,DoubleNullKey=@tDoubleNullKey FROM OperateTest t WHERE t.Id = @tId", db.SqlStatement);
                Assert.Equal(new[] { "@tId", "@tKey2", "@tStringKey", "@tIntKey", "@tIntNullKey", "@tDateNullKey", "@tDateTimeNullKey", "@tDoubleNullKey" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void UpdateAsync()
        {
            using (var db = new ApiDb())
            {
                var a = db.UpdateAsync(new OperationTest
                {
                    Id = 1,
                    Key2 = 1,
                    IntKey = 1,
                    StringKey = "1",
                    IntNullKey = null,
                    FloatNullKey = 1,
                    DoubleNullKey = 1,
                    DateNullKey = DateTime.MinValue,
                    DateTimeNullKey = null
                });

                Assert.Equal("UPDATE t SET Key2=@tKey2,StringKey=@tStringKey,IntKey=@tIntKey,IntNullKey=@tIntNullKey,DateNullKey=@tDateNullKey,DateTimeNullKey=@tDateTimeNullKey,DoubleNullKey=@tDoubleNullKey FROM OperateTest t WHERE t.Id = @tId", db.SqlStatement);
                Assert.Equal(new[] { "@tId", "@tKey2", "@tStringKey", "@tIntKey", "@tIntNullKey", "@tDateNullKey", "@tDateTimeNullKey", "@tDoubleNullKey" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void Delete()
        {
            using (var db = new ApiDb())
            {
                db.Delete(new OperationTest
                {
                    Id = 1
                });

                Assert.Equal("DELETE t FROM OperateTest t WHERE t.Id = @tId", db.SqlStatement);
                Assert.Equal(new[] { "@tId" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void DeleteAsync()
        {
            using (var db = new ApiDb())
            {
                var a = db.DeleteAsync(new OperationTest
                {
                    Id = 1
                });

                Assert.Equal("DELETE t FROM OperateTest t WHERE t.Id = @tId", db.SqlStatement);
                Assert.Equal(new[] { "@tId" }, db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void Query_ToList()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().ToList();
                Assert.Equal("SELECT * FROM OperateTest t  WHERE  1=1", db.SqlStatement);
                Assert.Equal(new string[0], db.Parameters.Keys.ToArray());
            }
        }

        [Fact]
        public void Query_Where()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().Where(t => t.StringKey.EndsWith("3")).ToList();
                Assert.Equal("SELECT * FROM OperateTest t  WHERE ( 1=1 )  AND  (t.StringKey LIKE @tStringKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tStringKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "%3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_MultiWhere()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("3")).Where(t => t.IntKey == 3).ToList();
                Assert.Equal("SELECT * FROM OperateTest t  WHERE (( 1=1 )  AND  (t.StringKey LIKE @tStringKey))  AND  (t.IntKey = @tIntKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tStringKey", "@tIntKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "%3%", "3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_Select()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().Where(t => t.IntKey <= 3).Select(t => new { t.IntKey, t.StringKey }).ToList();
                Assert.Equal("SELECT t.IntKey,t.StringKey FROM OperateTest t  WHERE ( 1=1 )  AND  (t.IntKey <= @tIntKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tIntKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_OrderBy()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().Where(t => t.IntKey <= 3).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).ToList();
                Assert.Equal("SELECT t.IntKey,t.StringKey FROM OperateTest t  WHERE ( 1=1 )  AND  (t.IntKey <= @tIntKey)  ORDER BY t.IntKey DESC", db.SqlStatement);
                Assert.Equal(new[] { "@tIntKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_Limit()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().Where(t => t.IntKey > 3).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).Limit(30).ToList();
                Assert.Equal("SELECT TOP 30 t.IntKey,t.StringKey FROM OperateTest t  WHERE ( 1=1 )  AND  (t.IntKey > @tIntKey)  ORDER BY t.IntKey DESC", db.SqlStatement);
                Assert.Equal(new[] { "@tIntKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_Paging()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderBy(t => t.IntKey).Paging(0, 10).ToList();
                Assert.Equal("SELECT TOP 10 TTTTTT.IntKey,TTTTTT.StringKey FROM (SELECT ROW_NUMBER() OVER ( ORDER BY t.IntKey ASC) AS RowNumber,t.IntKey,t.StringKey FROM OperateTest t  WHERE ( 1=1 )  AND  (t.StringKey LIKE @tStringKey)) AS TTTTTT  WHERE RowNumber > -10", db.SqlStatement);
                Assert.Equal(new[] { "@tStringKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "%1%" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_Any()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().Where(t => t.StringKey.EndsWith("3")).Any();
                Assert.Equal("SELECT TOP 1 1 FROM OperateTest t  WHERE ( 1=1 )  AND  (t.StringKey LIKE @tStringKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tStringKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "%3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_Count()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().Where(t => t.StringKey.EndsWith("3")).Count();
                Assert.Equal("SELECT COUNT(0) FROM OperateTest t  WHERE ( 1=1 )  AND  (t.StringKey LIKE @tStringKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tStringKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "%3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_ToData()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().Where(t => t.StringKey.EndsWith("3")).ToData();
                Assert.Equal("SELECT TOP 1 * FROM OperateTest t  WHERE ( 1=1 )  AND  (t.StringKey LIKE @tStringKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tStringKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "%3" }, db.Parameters.Values.ToArray());
            }
        }

        [Fact]
        public void Query_ToDataSet()
        {
            using (var db = new ApiDb())
            {
                db.Queryable<OperationTest>().Where(t => t.StringKey.EndsWith("3")).ToDataSet();
                Assert.Equal("SELECT * FROM OperateTest t  WHERE ( 1=1 )  AND  (t.StringKey LIKE @tStringKey)", db.SqlStatement);
                Assert.Equal(new[] { "@tStringKey" }, db.Parameters.Keys.ToArray());
                Assert.Equal(new[] { "%3" }, db.Parameters.Values.ToArray());
            }
        }
    }
}