using MongoDB.Bson;
using MongoDB.Driver;
using Test.SevenTiny.Bantina.Model;
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

        [Fact]
        public void GetConnectionString()
        {
            var conn = ConnectionStrings.Get("mongodb39911");
        }

        [Fact]
        public void Add()
        {

        }
    }
}
