using SevenTiny.Bantina.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test.SevenTiny.Bantina.Redis
{
    public class RedisCacheTest
    {
        [Fact]
        public void CacheSet()
        {
            string key = "testKey";
            string value = "testValue";

            IRedisCache instance = RedisCacheManager.Instance;
            instance.Set(key, value);

            var getValue = instance.Get(key);

            Assert.Equal(value, getValue);
        }

        [Fact]
        public void CacheDelete()
        {
            string key = "testKey";

            IRedisCache instance = RedisCacheManager.Instance;
            instance.Delete(key);

            var isExist = instance.Exist(key);

            Assert.False(isExist);
        }

        [Fact]
        public void CacheUpdate()
        {
            string key = "testKey";
            string value = "updateValue";

            IRedisCache instance = RedisCacheManager.Instance;
            instance.Update(key, value);

            var getValue = instance.Get(key);

            Assert.Equal(value, getValue);
        }
    }
}
