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
using MongoDB.Driver.Linq;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MongoDbContext<TDataBase> : NoSqlDbContext where TDataBase : class
    {
        protected MongoDbContext(string connectionString) : base(connectionString)
        {
            SetContext();
            Client = new MongoClient(connectionString);
        }
        protected MongoDbContext(string host, int port) : base(string.Concat(host, ":", port))
        {
            SetContext();
            Client = new MongoClient(new MongoClientSettings { Server = new MongoServerAddress(host, port) });
        }
        protected MongoDbContext(IDictionary<string, int> host_port_dic) : base("123")    //mongodb 不用连接管理器托管字符串管理，所以这里随便能传递了一个连接字符串
        {
            SetContext();

            if (host_port_dic == null || !host_port_dic.Any())
                throw new ArgumentException("host_port_dic must be provide.", nameof(host_port_dic));

            Client = new MongoClient(new MongoClientSettings
            {
                Servers = host_port_dic.Select(t => new MongoServerAddress(t.Key, t.Value)).ToList()
            });
        }
        protected MongoDbContext(MongoClientSettings mongoClientSettings) : base("123")    //mongodb 不用连接管理器托管字符串管理，所以这里随便能传递了一个连接字符串
        {
            SetContext();
            Client = new MongoClient(mongoClientSettings);
        }
        /// <summary>
        /// 上下文赋值
        /// </summary>
        private void SetContext()
        {
            DataBaseType = DataBaseType.MongoDB;
            DataBaseName = DataBaseAttribute.GetName(typeof(TDataBase));
        }

        #region MongoDb Server
        protected MongoClient Client { get; private set; }
        protected IMongoDatabase DataBase => Client.GetDatabase(DataBaseName);
        public IMongoCollection<TEntity> GetCollectionEntity<TEntity>() where TEntity : class
        {
            CollectionName = TableAttribute.GetName(typeof(TEntity));
            return DataBase.GetCollection<TEntity>(CollectionName);
        }
        /// <summary>
        /// 对外支持弱类型的接口
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public IMongoCollection<BsonDocument> GetCollectionBson(string collectionName)
        {
            CollectionName = collectionName;
            return DataBase.GetCollection<BsonDocument>(collectionName);
        }
        #endregion

        #region 强类型 API
        public override void Add<TEntity>(TEntity entity)
        {
            DataValidatorSafeExecute(v => v.Verify(entity));
            GetCollectionEntity<TEntity>().InsertOne(entity);
            this.DbCacheManagerSafeExecute(m => m.Add(entity));
        }
        public override async Task AddAsync<TEntity>(TEntity entity)
        {
            DataValidatorSafeExecute(v => v.Verify(entity));
            await GetCollectionEntity<TEntity>().InsertOneAsync(entity);
            this.DbCacheManagerSafeExecute(m => m.Add(entity));
        }
        public void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            DataValidatorSafeExecute(v => v.Verify(entities));
            GetCollectionEntity<TEntity>().InsertMany(entities);
            this.DbCacheManagerSafeExecute(m => m.Add(entities));
        }
        public async Task AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            DataValidatorSafeExecute(v => v.Verify(entities));
            await GetCollectionEntity<TEntity>().InsertManyAsync(entities);
            this.DbCacheManagerSafeExecute(m => m.Add(entities));
        }

        public override void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity)
        {
            DataValidatorSafeExecute(v => v.Verify(entity));
            GetCollectionEntity<TEntity>().ReplaceOne(filter, entity);
            this.DbCacheManagerSafeExecute(m => m.Update(entity, filter));
        }
        public override async Task UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity)
        {
            DataValidatorSafeExecute(v => v.Verify(entity));
            await GetCollectionEntity<TEntity>().ReplaceOneAsync(filter, entity);
            this.DbCacheManagerSafeExecute(m => m.Update(entity, filter));
        }

        public void DeleteOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            GetCollectionEntity<TEntity>().DeleteOne(filter);
            this.DbCacheManagerSafeExecute(m => m.Delete(filter));
        }
        public async Task DeleteOneAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            await GetCollectionEntity<TEntity>().DeleteOneAsync(filter);
            this.DbCacheManagerSafeExecute(m => m.Delete(filter));
        }
        public override void Delete<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            GetCollectionEntity<TEntity>().DeleteMany(filter);
            this.DbCacheManagerSafeExecute(m => m.Delete(filter));
        }
        public override async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            await GetCollectionEntity<TEntity>().DeleteManyAsync(filter);
            this.DbCacheManagerSafeExecute(m => m.Delete(filter));
        }

        public IMongoQueryable<TEntity> MongoQueryable<TEntity>() where TEntity : class
        {
            return GetCollectionEntity<TEntity>().AsQueryable();
        }
        #endregion

        /// <summary>
        /// 获取全集合数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        internal override List<TEntity> GetFullCollectionData<TEntity>()
        {
            //获取MongoDb全文档记录
            return GetCollectionEntity<TEntity>().Find(t => true).ToList();
        }
    }
}
