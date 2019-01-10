using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class MysqlTest
    {
        public MySqlDb Db => new MySqlDb();

        [Fact]
        [Trait("desc", "初始化测试数据，当跑全部下列用例的时候，删除所有数据并执行预置数据操作！")]
        public void InitTestDatas()
        {
            return;//初始化数据把这行代码放开

            //清空所有数据,并重置索引
            Db.ExecuteSql("truncate table " + Db.GetTableName<OperateTestModel>());

            //预置测试数据
            List<OperateTestModel> models = new List<OperateTestModel>();
            for (int i = 1; i < 1001; i++)
            {
                models.Add(new OperateTestModel
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
            Db.Add<OperateTestModel>(models);

            Assert.True(true);
        }

        [Theory]
        [InlineData(9999)]
        public void Add(int value)
        {
            OperateTestModel model = new OperateTestModel();
            model.IntKey = value;
            model.StringKey = "AddTest";
            model.IntKey = value;
            Db.Add<OperateTestModel>(model);
        }

        [Theory]
        [InlineData(9999)]
        public void Update(int value)
        {
            OperateTestModel model = Db.QueryOne<OperateTestModel>(t => t.IntKey == value);
            //model.Id = value;   //自增的主键不应该被修改,如果用这种方式进行修改，给Id赋值就会导致修改不成功，因为条件是用第一个主键作为标识修改的
            model.Key2 = value;
            model.StringKey = $"UpdateTest_{value}";
            model.IntNullKey = value;
            model.DateTimeNullKey = DateTime.Now;
            model.DateNullKey = DateTime.Now.Date;
            model.DoubleNullKey = model.IntNullKey;
            model.FloatNullKey = model.IntNullKey;
            Db.Update<OperateTestModel>(model);
        }

        [Theory]
        [InlineData(9999)]
        public void DeleteWhere(int value)
        {
            Db.Delete<OperateTestModel>(t => t.IntKey == value);
        }

        [Theory]
        [InlineData(9999)]
        public void DeleteEntity(int value)
        {
            OperateTestModel model = Db.QueryOne<OperateTestModel>(t => t.IntKey == value);
            Db.Delete<OperateTestModel>(model);
        }

        [Fact]
        public void Query_Where()
        {
            var re = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.EndsWith("3")).ToList();
            Assert.Equal(100, re.Count);
        }

        [Fact]
        public void Query_Where_Multi()
        {
            var re = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("3")).Where(t => t.IntKey == 3).ToList();
            Assert.Single(re);
        }

        [Fact]
        public void Query_Select()
        {
            var re = Db.Queryable<OperateTestModel>().Where(t => t.IntKey <= 3).Select(t => new { t.IntKey, t.StringKey }).ToList();
            Assert.Equal(3, re.Count);
        }

        [Fact]
        public void Query_OrderBy()
        {
            var re = Db.Queryable<OperateTestModel>().Where(t => t.IntKey <= 9).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).ToList();
            Assert.True(re.Count == 9 && re.First().IntKey == 9 && re.First().Id == 0);//没有查id，id应该=0
        }

        [Fact]
        public void Query_Limit()
        {
            var re = Db.Queryable<OperateTestModel>().Where(t => t.IntKey > 3).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).Limit(30).ToList();
            Assert.Equal(30, re.Count);
        }

        [Fact]
        public void Query_Paging()
        {
            var re4 = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderBy(t => t.IntKey).Paging(0, 10).ToList();
            var re5 = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).Paging(0, 10).ToList();
            var re6 = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderBy(t => t.IntKey).Paging(1, 10).ToList();
            Assert.True(re4.Count == re5.Count && re5.Count == re6.Count && re6.Count == re4.Count);
        }
    }
}
