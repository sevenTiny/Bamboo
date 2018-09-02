using SevenTiny.Bantina;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Test.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class SqlServerDbContextTest
    {
        [Fact]
        public void QueryList()
        {
            using (var db = new SqlServerTestDbContext())
            {
                var students = db.QueryList<Student>(t => true);
            }
        }

        [Fact]
        public void Add()
        {
            using (var db = new SqlServerTestDbContext())
            {
                Student stu = new Student();
                stu.Name = "AddTest";
                db.Add<Student>(stu);
            }
        }

        [Fact]
        public void Update()
        {
            using (var db = new SqlServerTestDbContext())
            {
                Student stu = db.QueryOne<Student>(t => t.Name.Contains("AddTest"));
                stu.Age = 3;
                db.Update<Student>(t => t.Name.Contains("AddTest"), stu);
            }
        }

        [Fact]
        public void Delete()
        {
            using (var db = new SqlServerTestDbContext())
            {
                db.Delete<Student>(t => t.Id == 1);
            }
        }

        [Theory]
        [InlineData(100)]
        [Trait("desc", "无缓存测试")]
        public void QueryListWithNoCacheLevel1(int times)
        {
            int fromCacheTimes = 0;
            var timeSpan = StopwatchHelper.Caculate(times, () =>
            {
                using (var db = new SqlServerTestDbContext())
                {
                    var students = db.QueryList<Student>(t => true);
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
        public void QueryListWithCacheLevel1(int times)
        {
            int fromCacheTimes = 0;
            var timeSpan = StopwatchHelper.Caculate(times, () =>
            {
                using (var db = new SqlServerTestDbContext())
                {
                    var students = db.QueryList<Student>(t => true);
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
        public void QueryListWithCacheLevel2(int times)
        {
            int fromCacheTimes = 0;
            var timeSpan = StopwatchHelper.Caculate(times, () =>
            {
                using (var db = new SqlServerTestDbContext())
                {
                    var students = db.QueryList<Student>(t => true);
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
        public void AddUpdateDeleteQueryCacheLevel2(int times)
        {
            int fromCacheTimes = 0;
            var timeSpan = StopwatchHelper.Caculate(times, () =>
            {
                using (var db = new SqlServerTestDbContext())
                {
                    //查询单个
                    var stu = db.QueryOne<Student>(t => t.Id == 1);
                    //修改单个属性
                    stu.Name = "test11-1";
                    db.Update<Student>(t => t.Id == 1, stu);

                    var students = db.QueryList<Student>(t => true);
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
