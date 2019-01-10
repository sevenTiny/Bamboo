using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Configuration;
using SevenTiny.Bantina.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Bankinate.Model
{
    [DataBase("local")]
    public class MongoDb : MongoDbContext<MongoDb>
    {
        public MongoDb() : base(ConnectionStrings.Get("mongodb39911"))
        {

        }
    }
}
