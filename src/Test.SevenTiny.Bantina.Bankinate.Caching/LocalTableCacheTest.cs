using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Caching;
using System;
using System.ComponentModel;
using System.Threading;
using Test.Common;
using Test.Common.Model;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Test.Caching
{
    public class LocalTableCacheTest
    {
        /// <summary>
        /// 缓存过期时间
        /// </summary>
        private const int _CacheMillisecondsTimeout = 200;
        /// <summary>
        /// 等待执行扫描表时间
        /// </summary>
        private const int _waitScanTableMillisecondsTimeout = 100;

        [DataBase("SevenTinyTest")]
        private class LocalTableCache : MySqlDbContext<LocalTableCache>
        {
            public LocalTableCache() : base(ConnectionStringHelper.ConnectionString_Write_MySql, ConnectionStringHelper.ConnectionStrings_Read_MySql)
            {
                this.OpenLocalCache(openTableCache: true, tableCacheExpiredTimeSpan: TimeSpan.FromMilliseconds(_CacheMillisecondsTimeout));
                RealExecutionSaveToDb = false;
            }
        }

        [Fact]
        public void LocalTableCacheTestQueue()
        {
            QueryAll();
            QueryCount();
            QueryWhereWithUnSameCondition();
            QueryCacheExpired();
        }

        private void QueryAll()
        {
            using (var db = new LocalTableCache())
            {
                db.FlushAllCache();

                db.Queryable<OperationTest>().ToList();

                Assert.False(db.IsFromCache);

                Thread.CurrentThread.Join(_waitScanTableMillisecondsTimeout);

                db.Queryable<OperationTest>().ToList();

                Assert.True(db.IsFromCache);
            }
        }

        private void QueryCount()
        {
            using (var db = new LocalTableCache())
            {
                db.FlushAllCache();

                db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("test")).Count();

                Assert.False(db.IsFromCache);

                Thread.CurrentThread.Join(_waitScanTableMillisecondsTimeout);

                var result = db.Queryable<OperationTest>().Where(t => t.StringKey.Contains("test")).Count();

                Assert.True(db.IsFromCache);

                Assert.Equal(0, result);
            }
        }

        private void QueryWhereWithUnSameCondition()
        {
            using (var db = new LocalTableCache())
            {
                db.FlushAllCache();

                db.Queryable<OperationTest>().Where(t => t.Id == 1).FirstOrDefault();

                Assert.False(db.IsFromCache);

                Thread.CurrentThread.Join(_waitScanTableMillisecondsTimeout);

                db.Queryable<OperationTest>().Where(t => t.Id == 2).FirstOrDefault();

                Assert.True(db.IsFromCache);
            }
        }

        [Description("设置缓存过期时间进行测试")]
        private void QueryCacheExpired()
        {
            using (var db = new LocalTableCache())
            {
                db.FlushAllCache();

                //先查询肯定是没有的
                db.Queryable<OperationTest>().Where(t => t.Id == 1).ToList();

                Assert.False(db.IsFromCache);

                Thread.CurrentThread.Join(_waitScanTableMillisecondsTimeout);

                //第二次在缓存中可以查到
                db.Queryable<OperationTest>().Where(t => t.Id == 1).ToList();

                Assert.True(db.IsFromCache);

                Thread.Sleep(_CacheMillisecondsTimeout);

                //这时候查询应该从缓存获取不到
                db.Queryable<OperationTest>().Where(t => t.Id == 1).ToList();

                Assert.False(db.IsFromCache);
            }
        }
    }
}