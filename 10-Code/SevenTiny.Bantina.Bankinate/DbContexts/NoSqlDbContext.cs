using MongoDB.Driver;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Cache;
using SevenTiny.Bantina.Bankinate.DataAccessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class NoSqlDbContext<TDataBase> : DbContext, IDbContext where TDataBase : class
    {
        protected NoSqlDbContext(DataBaseType dataBaseType, string connectionString) : base(dataBaseType)
        {
            switch (dataBaseType)
            {
                case DataBaseType.MongoDB:
                    Client = new MongoClient(connectionString);
                    break;
                default:
                    break;
            }
            DataBaseName = DataBaseAttribute.GetName(typeof(TDataBase));
        }
        protected NoSqlDbContext(MongoClientSettings mongoClientSettings) : base(DataBaseType.MongoDB)
        {
            Client = new MongoClient(mongoClientSettings);
            DataBaseName = DataBaseAttribute.GetName(typeof(TDataBase));
        }

        #region MongoDb Server
        private MongoClient Client { get; set; }
        protected IMongoDatabase DataBase => Client.GetDatabase(DataBaseAttribute.GetName(typeof(TDataBase)));
        protected IMongoCollection<TEntity> GetCollectionEntity<TEntity>() where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));
            return DataBase.GetCollection<TEntity>(TableName);
        }
        #endregion

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            switch (DataBaseType)
            {
                case DataBaseType.MongoDB:
                    GetCollectionEntity<TEntity>().InsertOne(entity);
                    break;
                default:
                    break;
            }
            DbCacheManager.Add(this, entity);
        }
        public void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            switch (DataBaseType)
            {
                case DataBaseType.MongoDB:
                    GetCollectionEntity<TEntity>().InsertMany(entities);
                    break;
                default:
                    break;
            }
            DbCacheManager.Add(this, entities);
        }
        public void AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            switch (DataBaseType)
            {
                case DataBaseType.MongoDB:
                    GetCollectionEntity<TEntity>().InsertOneAsync(entity);
                    break;
                default:
                    break;
            }
            DbCacheManager.Add(this, entity);
        }
        public void AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            switch (DataBaseType)
            {
                case DataBaseType.MongoDB:
                    GetCollectionEntity<TEntity>().InsertManyAsync(entities);
                    break;
                default:
                    break;
            }
            DbCacheManager.Add(this, entities);
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            SqlStatement = filter.ToString();
            switch (DataBaseType)
            {
                case DataBaseType.MongoDB:
                    GetCollectionEntity<TEntity>().ReplaceOne(filter, entity);
                    break;
                default:
                    break;
            }
            DbCacheManager.Update(this, entity, filter);
        }
        public void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            SqlStatement = filter.ToString();
            switch (DataBaseType)
            {
                case DataBaseType.MongoDB:
                    GetCollectionEntity<TEntity>().ReplaceOneAsync(filter, entity);
                    break;
                default:
                    break;
            }
            DbCacheManager.Update(this, entity, filter);
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlStatement = filter.ToString();
            switch (DataBaseType)
            {
                case DataBaseType.MongoDB:
                    GetCollectionEntity<TEntity>().DeleteMany(filter);
                    break;
                default:
                    break;
            }
            DbCacheManager.Delete(this, filter);
        }
        public void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlStatement = filter.ToString();
            switch (DataBaseType)
            {
                case DataBaseType.MongoDB:
                    GetCollectionEntity<TEntity>().DeleteManyAsync(filter);
                    break;
                default:
                    break;
            }
            DbCacheManager.Delete(this, filter);
        }

        public int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlStatement = filter.ToString();
            return DbCacheManager.GetCount(this, filter, () =>
            {
                switch (DataBaseType)
                {
                    case DataBaseType.MongoDB:
                        return Convert.ToInt32(GetCollectionEntity<TEntity>().Count(filter));
                    default:
                        break;
                }
                return default(int);
            });
        }
        public bool QueryExist<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlStatement = filter.ToString();
            switch (DataBaseType)
            {
                case DataBaseType.MongoDB:
                    return QueryCount<TEntity>(filter) > 0;
                default:
                    break;
            }
            return default(bool);
        }
        public TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlStatement = filter.ToString();
            return DbCacheManager.GetEntity(this, filter, () =>
            {
                switch (DataBaseType)
                {
                    case DataBaseType.MongoDB:
                        return QueryList(filter).FirstOrDefault();
                    default:
                        break;
                }
                return default(TEntity);
            });
        }
        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlStatement = filter.ToString();
            return DbCacheManager.GetEntities(this, filter, () =>
            {
                switch (DataBaseType)
                {
                    case DataBaseType.MongoDB:
                        return GetCollectionEntity<TEntity>().Find(filter).ToList();
                    default:
                        break;
                }
                return default(List<TEntity>);
            });
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
