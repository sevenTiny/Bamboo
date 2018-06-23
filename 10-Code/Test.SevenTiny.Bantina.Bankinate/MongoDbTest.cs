using MongoDB.Bson;
using MongoDB.Driver;
using System;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class MongoDbTest
    {
        [Fact]
        public void Test1()
        {
            var filter = Builders<BsonDocument>.Filter.Gt("counter", 100);
            //var resut = collection.DeleteManyAsync(filter).Result;
        }
    }
}
