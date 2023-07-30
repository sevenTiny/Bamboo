//using SevenTiny.Bantina.Bankinate;
//using SevenTiny.Bantina.Bankinate.Attributes;
//using SevenTiny.Bantina.Bankinate.Caching;
//using Test.Common;
//using Test.Common.Model;
//using Xunit;

//namespace Test.Caching
//{
//    public class RedisQueryCacheTest
//    {
//        [DataBase("SevenTinyTest")]
//        private class RedisQueryCache : MySqlDbContext<RedisQueryCache>
//        {
//            public RedisQueryCache() : base(ConnectionStringHelper.ConnectionString_Write, ConnectionStringHelper.ConnectionStrings_Read)
//            {
//                this.OpenRedisCache(true, false, "192.168.1.110:39912");
//                //OpenQueryCache = true;//一级缓存开关
//                //CacheMediaType = CacheMediaType.Redis;
//                //CacheMediaServer = "192.168.1.110:39912";//redis服务器地址以及端口号
//            }
//        }

//        [Fact]
//        public void QueryAdd()
//        {
//            using (var db = new RedisQueryCache())
//            {
//                //1.先查询肯定是没有的
//                var re0 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
//                Assert.Null(re0);

//                db.Add(new OperateTestModel
//                {
//                    IntKey = 123,
//                    StringKey = "CacheAddTest123"
//                });

//                //2.这时候查询应该有一条，这次查询才加入缓存
//                var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
//                Assert.Single(re);

//                //3.重复查询，这次是从缓存查的，还是一条
//                var re2 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
//                Assert.Single(re2);

//                //再次新增，清楚一级缓存
//                db.Add(new OperateTestModel
//                {
//                    IntKey = 123,
//                    StringKey = "CacheAddTest123"
//                });

//                //4.这次查应该从数据库查询，加入缓存，2条
//                var re4 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
//                Assert.Equal(2, re4.Count);

//                db.Delete<OperateTestModel>(t => t.StringKey.StartsWith("CacheAddTest"));

//                //4.删除完毕以后，查询是没有的
//                var re3 = db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
//                Assert.Null(re3);
//            }
//        }

//        [Theory]
//        [InlineData(100)]
//        public void QueryAll(int count)
//        {
//            using (var db = new RedisQueryCache())
//            {
//                for (int i = 0; i < count; i++)
//                {
//                    var re = db.Queryable<OperateTestModel>().ToList();
//                    Assert.Equal(1000, re.Count);
//                }
//            }
//        }

//        [Theory]
//        [InlineData(100)]
//        public void QueryOne(int count)
//        {
//            using (var db = new RedisQueryCache())
//            {
//                for (int i = 0; i < count; i++)
//                {
//                    var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("test")).FirstOrDefault();
//                    Assert.NotNull(re);
//                }
//            }
//        }

//        [Theory]
//        [InlineData(100)]
//        public void QueryCount(int count)
//        {
//            using (var db = new RedisQueryCache())
//            {
//                for (int i = 0; i < count; i++)
//                {
//                    var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("test")).Count();
//                    Assert.Equal(1000, re);
//                }
//            }
//        }

//        [Theory]
//        [InlineData(100)]
//        public void QueryWhereWithUnSameCondition(int count)
//        {
//            using (var db = new RedisQueryCache())
//            {
//                for (int i = 0; i < count; i++)
//                {
//                    var re = db.Queryable<OperateTestModel>().Where(t => t.Id == 1).FirstOrDefault();
//                    var re1 = db.Queryable<OperateTestModel>().Where(t => t.Id == 2).FirstOrDefault();
//                    Assert.NotEqual(re.StringKey, re1.StringKey);
//                }
//            }
//        }

//        [Theory]
//        [InlineData(100)]
//        [Trait("desc", "设置缓存过期时间进行测试")]
//        public void QueryCacheExpired(int count)
//        {
//            using (var db = new RedisQueryCache())
//            {
//                for (int i = 0; i < count; i++)
//                {
//                    var re = db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("test")).FirstOrDefault();
//                    Assert.NotNull(re);
//                }
//            }
//        }

//        [Fact]
//        [Trait("bug", "两次查出来的结果不正确【由于内存做的缓存，改内存数据时缓存会一起变动...作为缓存时，慎改内存数据】")]
//        [Trait("bug", "参数传递值没有使用参数化查询")]
//        public void QueryBugRepaire2()
//        {
//            int metaObjectId = 1;
//            using (var db = new RedisQueryCache())
//            {
//                for (int i = 0; i < 3; i++)
//                {
//                    var re = db.Queryable<OperateTestModel>().Where(t => t.IntNullKey == 1 && t.IntKey == metaObjectId).ToList();
//                    Assert.NotNull(re);
//                }
//            }
//        }
//    }
//}
