/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-19 23:58:08
 * Modify: 2018-04-19 23:58:08
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using MongoDB.Bson;
using MongoDB.Driver;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MongoDbContext<TDataBase> : NoSqlDbContext<TDataBase> where TDataBase : class
    {
        protected MongoDbContext(string connectionString) : base(DataBaseType.MongoDB, connectionString) { }
        protected MongoDbContext(MongoClientSettings mongoClientSettings) : base(mongoClientSettings) { }

        /// <summary>
        /// MongoDb Bson集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        private IMongoCollection<BsonDocument> GetCollectionBson<TEntity>() where TEntity : class => DataBase.GetCollection<BsonDocument>(TableAttribute.GetName(typeof(TEntity)));

        public void Add<TEntity>(BsonDocument bsonDocument) where TEntity : class
        {
            GetCollectionBson<TEntity>().InsertOne(bsonDocument);
        }
        public void AddAsync<TEntity>(BsonDocument bsonDocument) where TEntity : class
        {
            GetCollectionBson<TEntity>().InsertOneAsync(bsonDocument);
        }
        public void Add<TEntity>(IEnumerable<BsonDocument> bsonDocuments) where TEntity : class
        {
            GetCollectionBson<TEntity>().InsertMany(bsonDocuments);
        }
        public void AddAsync<TEntity>(IEnumerable<BsonDocument> bsonDocuments) where TEntity : class
        {
            GetCollectionBson<TEntity>().InsertManyAsync(bsonDocuments);
        }

        public void Update<TEntity>(FilterDefinition<BsonDocument> filter, BsonDocument replacement) where TEntity : class
        {
            GetCollectionBson<TEntity>().ReplaceOne(filter, replacement);
        }
        public void UpdateAsync<TEntity>(FilterDefinition<BsonDocument> filter, BsonDocument replacement) where TEntity : class
        {
            GetCollectionBson<TEntity>().ReplaceOneAsync(filter, replacement);
        }

        public void DeleteOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            GetCollectionEntity<TEntity>().DeleteOne(filter);
        }
        public void DeleteOne<TEntity>(FilterDefinition<BsonDocument> filter) where TEntity : class
        {
            GetCollectionBson<TEntity>().DeleteOne(filter);
        }
        public void Delete<TEntity>(FilterDefinition<BsonDocument> filter) where TEntity : class
        {
            GetCollectionBson<TEntity>().DeleteMany(filter);
        }
        public void DeleteAsync<TEntity>(FilterDefinition<BsonDocument> filter) where TEntity : class
        {
            GetCollectionBson<TEntity>().DeleteManyAsync(filter);
        }

        public TEntity QueryOne<TEntity>(string _id) where TEntity : class
        {
            FilterDefinitionBuilder<TEntity> builderFilter = Builders<TEntity>.Filter;
            FilterDefinition<TEntity> filter = builderFilter.Eq("_id", _id);
            return GetCollectionEntity<TEntity>().Find(filter).FirstOrDefault();
        }
        public BsonDocument QueryOneBson<TEntity>(string _id) where TEntity : class
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", _id);
            return GetCollectionBson<TEntity>().Find(filter).FirstOrDefault();
        }
        public BsonDocument QueryOneBson<TEntity>(FilterDefinition<BsonDocument> filter) where TEntity : class
        {
            return GetCollectionBson<TEntity>().Find(filter).FirstOrDefault();
        }

        public List<BsonDocument> QueryListBson<TEntity>() where TEntity : class
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Where(t => true);
            return GetCollectionBson<TEntity>().Find(filter).ToList();
        }
        public List<BsonDocument> QueryListBson<TEntity>(FilterDefinition<BsonDocument> filter) where TEntity : class
        {
            return GetCollectionBson<TEntity>().Find(filter).ToList();
        }

        public int QueryCount<TEntity>(FilterDefinition<BsonDocument> filter) where TEntity : class
        {
            return Convert.ToInt32(GetCollectionBson<TEntity>().Count(filter));
        }
        public bool QueryExist<TEntity>(FilterDefinition<BsonDocument> filter) where TEntity : class
        {
            return QueryCount<TEntity>(filter) > 0;
        }
    }
}
