using System;
using System.Collections.Generic;
using System.Text;
using Test.SevenTiny.Bantina.Model;
using Xunit;

namespace Test.SevenTiny.Bankinate
{
    public class MongoDbTest
    {
        [Fact]
        public void GetConnectionString()
        {
            var conn = ConnectionStrings.Get("mongodb39911");
        }
    }
}
