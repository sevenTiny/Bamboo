using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SevenTiny.Bantina.Net.Http;

namespace Test.SevenTiny.Bantina
{
    public class HttpHelperTest
    {
        [Fact]
        public void Get()
        {
            var result = HttpHelper.Get(new GetRequestArgs { Url = "http://www.baidu.com" });
            Assert.NotNull(result);
        }

        [Fact]
        public void Post()
        {
            var arg = new PostRequestArgs
            {
                Url = "http://www.baidu.com"
            };
            var result = HttpHelper.Post(arg);

            Assert.NotNull(result);
        }

        [Fact]
        public void Put()
        {
            var arg = new PutRequestArgs
            {
                Url = "http://www.baidu.com"
            };
            var result = HttpHelper.Put(arg);

            Assert.NotNull(result);
        }

        [Fact]
        public void Delete()
        {
            var arg = new DeleteRequestArgs
            {
                Url = "http://www.baidu.com"
            };
            var result = HttpHelper.Delete(arg);

            Assert.NotNull(result);
        }
    }
}
