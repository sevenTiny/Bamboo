using SevenTiny.Bantina;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class SqlServerTest
    {
        public SqlServerDb Db => new SqlServerDb();

        [Fact]
        [Trait("desc", "初始化测试数据，当跑全部下列用例的时候，删除所有数据并执行预置数据操作！")]
        public void InitTestDatas()
        {
            return;//初始化数据把这行代码放开

            //清空所有数据
            Db.Delete<Student>(t => true);

            //预置测试数据
            List<Student> students = new List<Student>();
            for (int i = 0; i < 1000; i++)
            {
                students.Add(new Student { Name = "7tiny_" + i, Age = i, SchoolTime = DateTime.Now });
            }
            Db.Add<Student>(students);

            Assert.True(true);
        }

        [Fact]
        public void Add()
        {
            Student stu = new Student();
            stu.Age = 9999;
            stu.Name = "AddTest";
            Db.Add<Student>(stu);
        }

        [Fact]
        public void Update()
        {
            Student stu = Db.QueryOne<Student>(t => t.Age == 9999);
            stu.Name = "UpdateTest9999";
            Db.Update<Student>(stu);
        }

        [Fact]
        public void Delete()
        {
            Db.Delete<Student>(t => t.Age == 9999);
        }

        [Fact]
        public void Query_Where()
        {
            var re = Db.Queryable<Student>().Where(t => t.Name.EndsWith("3")).ToList();
            Assert.Equal(100, re.Count);
        }

        [Fact]
        public void Query_Where_Multi()
        {
            var re = Db.Queryable<Student>().Where(t => t.Name.Contains("3")).Where(t => t.Age == 3).ToList();
            Assert.Single(re);
        }

        [Fact]
        public void Query_Select()
        {
            var re = Db.Queryable<Student>().Where(t => t.Age < 3).Select(t => new { t.Age, t.Name }).ToList();
            Assert.Equal(3, re.Count);
        }

        [Fact]
        public void Query_OrderBy()
        {
            var re = Db.Queryable<Student>().Where(t => t.Age < 9).Select(t => new { t.Age, t.Name }).OrderByDescending(t => t.Age).ToList();
            Assert.True(re.Count == 9 && re.First().Age == 8 && re.First().Id == 0);//没有查id，id应该=0
        }

        [Fact]
        public void Query_Top()
        {
            var re = Db.Queryable<Student>().Where(t => t.Age > 3).Select(t => new { t.Age, t.Name }).OrderByDescending(t => t.Age).Top(30).ToList();
            Assert.Equal(30, re.Count);
        }

        [Fact]
        public void Query_Paging()
        {
            var re4 = Db.Queryable<Student>().Where(t => t.Name.Contains("1")).Select(t => new { t.Age, t.Name }).OrderBy(t => t.Age).Paging(0, 10).ToList();
            var re5 = Db.Queryable<Student>().Where(t => t.Name.Contains("1")).Select(t => new { t.Age, t.Name }).OrderByDescending(t => t.Age).Paging(0, 10).ToList();
            var re6 = Db.Queryable<Student>().Where(t => t.Name.Contains("1")).Select(t => new { t.Age, t.Name }).OrderBy(t => t.Age).Paging(1, 10).ToList();
            Assert.True(re4.Count == re5.Count && re5.Count == re6.Count && re6.Count == re4.Count);
        }

        [Theory]
        [InlineData(100)]
        [Trait("desc", "无缓存测试")]
        public void PerformanceTest_QueryListWithNoCacheLevel1(int times)
        {
            return;

            int fromCacheTimes = 0;
            var timeSpan = StopwatchHelper.Caculate(times, () =>
            {
                using (var db = new SqlServerDb())
                {
                    var students = db.Queryable<Student>().Where(t => true).ToList();
                    if (db.IsFromCache)
                    {
                        fromCacheTimes++;
                    }
                }
            });
            Trace.WriteLine($"执行查询{times}次耗时：{timeSpan.TotalMilliseconds}，有{fromCacheTimes}次从缓存中获取，有{times - fromCacheTimes}次从数据库获取");
            //执行查询100次耗时：6576.8009
        }

        [Theory]
        [InlineData(10000)]
        [Trait("desc", "一级缓存测试")]
        [Trait("desc", "测试该用例，请将一级缓存（QueryCache）打开")]
        public void PerformanceTest_QueryListWithCacheLevel1(int times)
        {
            return;

            int fromCacheTimes = 0;
            var timeSpan = StopwatchHelper.Caculate(times, () =>
            {
                using (var db = new SqlServerDb())
                {
                    var students = db.Queryable<Student>().Where(t => true).ToList();
                    if (db.IsFromCache)
                    {
                        fromCacheTimes++;
                    }
                }
            });
            Trace.WriteLine($"执行查询{times}次耗时：{timeSpan.TotalMilliseconds}，有{fromCacheTimes}次从缓存中获取，有{times - fromCacheTimes}次从数据库获取");
            //执行查询10000次耗时：1598.2349
        }

        [Theory]
        [InlineData(10000)]
        [Trait("desc", "二级缓存测试")]
        [Trait("desc", "测试该用例，请将二级缓存（TableCache）打开，并在对应表的实体上添加缓存标签")]
        public void PerformanceTest_QueryListWithCacheLevel2(int times)
        {
            return;

            int fromCacheTimes = 0;
            var timeSpan = StopwatchHelper.Caculate(times, () =>
            {
                using (var db = new SqlServerDb())
                {
                    var students = db.Queryable<Student>().Where(t => true).ToList();
                    if (db.IsFromCache)
                    {
                        fromCacheTimes++;
                    }
                }
            });
            Trace.WriteLine($"执行查询{times}次耗时：{timeSpan.TotalMilliseconds}，有{fromCacheTimes}次从缓存中获取，有{times - fromCacheTimes}次从数据库获取");
            //执行查询10000次耗时：5846.0249，有9999次从缓存中获取，有1次从数据库获取。
            //通过更为详细的打点得知，共有两次从数据库获取值。第一次直接按条件查询存在一级缓存，后台线程扫描表存在了二级缓存。
            //缓存打点结果：二级缓存没有扫描完毕从一级缓存获取数据，二级缓存扫描完毕则都从二级缓存里面获取数据
        }

        [Theory]
        [InlineData(1000)]
        [Trait("desc", "开启二级缓存增删改查测试")]
        [Trait("desc", "测试该用例，请将二级缓存（TableCache）打开，并在对应表的实体上添加缓存标签")]
        public void PerformanceTest_AddUpdateDeleteQueryCacheLevel2(int times)
        {
            return;

            int fromCacheTimes = 0;
            var timeSpan = StopwatchHelper.Caculate(times, () =>
            {
                using (var db = new SqlServerDb())
                {
                    //查询单个
                    var stu = db.QueryOne<Student>(t => t.Id == 2);
                    //修改单个属性
                    stu.Name = "test11-1";
                    db.Update<Student>(t => t.Id == 1, stu);

                    var students = db.Queryable<Student>().Where(t => true).ToList();
                    if (db.IsFromCache)
                    {
                        fromCacheTimes++;
                    }
                }
            });
            Trace.WriteLine($"执行查询{times}次耗时：{timeSpan.TotalMilliseconds}，有{fromCacheTimes}次从缓存中获取，有{times - fromCacheTimes}次从数据库获取");
            //执行查询1000次耗时：19102.6441，有1000次从缓存中获取，有0次从数据库获取
            //事实上，第一次查询单条的时候已经从数据库扫描并放在了缓存中，后续都是对二级缓存的操作以及二级缓存中查询
        }
    }
}
