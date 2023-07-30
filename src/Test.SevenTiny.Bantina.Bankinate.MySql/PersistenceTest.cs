using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Test.Common;
using Test.Common.Model;
using Xunit;

namespace Test.MySql
{
    /// <summary>
    /// 数据预置类
    /// </summary>
    public class PersistenceTest
    {
        [DataBase("SevenTinyTest")]
        private class DataPreseterDb : MySqlDbContext<DataPreseterDb>
        {
            public DataPreseterDb() : base(ConnectionStringHelper.ConnectionString_Write_MySql, ConnectionStringHelper.ConnectionStrings_Read_MySql)
            {

            }
        }

        //[Fact]
        public void InitData_OperationTest()
        {
            using (var db = new DataPreseterDb())
            {
                //清空所有数据,并重置索引
                db.ExecuteSql("truncate table " + db.GetTableName<OperationTest>());

                //预置测试数据
                List<OperationTest> models = new List<OperationTest>();
                for (int i = 1; i < 1001; i++)
                {
                    db.Add<OperationTest>(new OperationTest
                    {
                        Key2 = i,
                        StringKey = $"test_{i}",
                        IntKey = i,
                        IntNullKey = null,
                        DateNullKey = DateTime.Now.Date,
                        DateTimeNullKey = DateTime.Now,
                        DoubleNullKey = i,
                        FloatNullKey = i
                    });
                }
            }
        }

        //[Fact]
        public void InitData_OperationTest2()
        {
            using (var db = new DataPreseterDb())
            {
                //清空所有数据,并重置索引
                db.ExecuteSql("truncate table " + db.GetTableName<OperationTest2>());

                //预置测试数据
                List<OperationTest2> models = new List<OperationTest2>();
                for (int i = 1; i < 10000; i++)
                {
                    models.Add(new OperationTest2
                    {
                        Uid = Guid.NewGuid(),
                        StringKey = string.Concat("str_", i)
                    });
                }

                db.Add<OperationTest2>(models);
            }
        }

        //[Fact]
        //[Description("持久化测试")]
        //public void Persistence()
        //{
        //    using (var db = new DataPreseterDb())
        //    {
        //        int value = 999999;

        //        //初次查询没有数据
        //        db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();

        //        //add一条数据
        //        OperationTest model = new OperationTest
        //        {
        //            IntKey = value,
        //            StringKey = "AddTest"
        //        };
        //        model.IntKey = value;
        //        db.Add<OperationTest>(model);

        //        //插入后查询有一条记录
        //        var re1 = db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
        //        Assert.Single(re1);
        //        Assert.Equal(value, re1.First().IntKey);

        //        //查询一条
        //        var entity = db.Queryable<OperationTest>().Where(t => t.IntKey == value).FirstOrDefault();
        //        Assert.NotNull(entity);
        //        Assert.Equal(value, entity.IntKey);

        //        //更新数据
        //        //entity.Id = value;   //自增的主键不应该被修改,如果用这种方式进行修改，给Id赋值就会导致修改不成功，因为条件是用第一个主键作为标识修改的
        //        entity.Key2 = value;
        //        entity.StringKey = $"UpdateTest_{value}";
        //        entity.IntNullKey = value;
        //        entity.DateTimeNullKey = DateTime.Now;
        //        entity.DateNullKey = DateTime.Now.Date;
        //        entity.DoubleNullKey = entity.IntNullKey;
        //        entity.FloatNullKey = entity.IntNullKey;
        //        db.Update<OperationTest>(entity);

        //        var entity2 = db.Queryable<OperationTest>().Where(t => t.IntKey == value).FirstOrDefault();
        //        Assert.NotNull(entity2);
        //        Assert.Equal(value, entity2.IntNullKey);
        //        Assert.Equal($"UpdateTest_{value}", entity2.StringKey);

        //        //删除数据
        //        db.Delete<OperationTest>(t => t.IntKey == value);

        //        //删除后查询没有
        //        var re4 = db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
        //        Assert.Null(re4);
        //    }
        //}

        //[Fact]
        //[Description("持久化测试_默认使用实体主键删除数据")]
        //public void Persistence_DeleteEntity()
        //{
        //    using (var db = new DataPreseterDb())
        //    {
        //        int value = 999999;

        //        //初次查询没有数据
        //        db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();

        //        //add一条数据
        //        OperationTest model = new OperationTest
        //        {
        //            IntKey = value,
        //            StringKey = "AddTest"
        //        };
        //        model.IntKey = value;
        //        db.Add<OperationTest>(model);

        //        //插入后查询有一条记录
        //        var re1 = db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
        //        Assert.Single(re1);
        //        Assert.Equal(value, re1.First().IntKey);

        //        var entity = db.Queryable<OperationTest>().Where(t => t.IntKey == value).FirstOrDefault();
        //        Assert.NotNull(entity);
        //        Assert.Equal(value, entity.IntKey);

        //        //删除数据
        //        db.Delete<OperationTest>(entity);

        //        //删除后查询没有
        //        var re4 = db.Queryable<OperationTest>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
        //        Assert.Null(re4);
        //    }
        //}

        //[Fact]
        public void GuidQueryTest()
        {
            using (var db = new DataPreseterDb())
            {
                var aa = db.Queryable<OperationTest2>().ToList();
                Assert.NotEqual(Guid.Empty, aa?.FirstOrDefault().Uid);
            }
        }
    }
}
