using SevenTiny.Bantina.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test.SevenTiny.Bantina.Redis
{
    public class RedisCacheTest
    {
        private IRedisCache Instance { get; set; }
        private void Init()
        {
            Instance = new RedisCacheManager("Default");
        }

        [Fact]
        public void CacheExist()
        {
            Init();
            string key = "testKey";
            var result = Instance.Exist(key);
        }

        [Fact]
        public void CacheSet()
        {
            Init();
            string key = "testKey";
            string value = "testValue";

            Instance.Set(key, value);

            var getValue = Instance.Get(key);

            Assert.Equal(value, getValue);
        }

        [Fact]
        public void CacheDelete()
        {
            Init();
            string key = "testKey";

            Instance.Delete(key);

            var isExist = Instance.Exist(key);

            Assert.False(isExist);
        }

        [Fact]
        public void CacheUpdate()
        {
            Init();
            string key = "testKey";
            string value = "updateValue";

            Instance.Update(key, value);

            var getValue = Instance.Get(key);

            Assert.Equal(value, getValue);
        }
    }
}
