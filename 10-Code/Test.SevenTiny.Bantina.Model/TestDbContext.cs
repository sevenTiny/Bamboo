using SevenTiny.Bantina.Bankinate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Model
{
    [DataBase("local")]
    public class TestDbContext : MongoDbContext<TestDbContext>
    {
        public TestDbContext() : base("mongodb://192.168.1.116:39601/local")
        {

        }
    }
}
