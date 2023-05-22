using MongoDB.Bson;
using MongoDB.Driver;
using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using Test.Common;
using Test.Common.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate.NoSqlDbTest
{
    [DataBase("local")]
    public class MongoDb : MongoDbContext<MongoDb>
    {
        public MongoDb() : base(ConnectionStringHelper.ConnectionString_Write_MongoDb)
        {

        }
    }

    public class MongoDbTest
    {

    }
}
