using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate.Caching
{
    /// <summary>
    /// 数据库缓存管理器
    /// Bankinate的缓存是分为两级的，每级都有对应的开关
    /// 一级缓存（QueryCache查询缓存），缓存简短查询中的缓存数据
    /// 二级缓存（TableCache表缓存），缓存整张表，需要标签配合使用
    /// </summary>
    public class DbCacheManager : CacheManagerBase, IDbCacheManager
    {
        public DbCacheManager(DbContext context, CacheOptions cacheOptions) : base(context, cacheOptions)
        {
            Ensure.ArgumentNotNullOrEmpty(context, nameof(context));
            Ensure.ArgumentNotNullOrEmpty(cacheOptions, nameof(cacheOptions));

            if (cacheOptions.OpenQueryCache)
                QueryCacheManager = new QueryCacheManager(context, cacheOptions);
            if (cacheOptions.OpenTableCache)
                TableCacheManager = new TableCacheManager(context, cacheOptions);
        }

        internal QueryCacheManager QueryCacheManager { get; private set; }
        internal TableCacheManager TableCacheManager { get; private set; }

        /// 清空所有缓存
        /// </summary>
        public void FlushAllCache()
        {
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushAllCache();
            if (CacheOptions.OpenTableCache)
                TableCacheManager.FlushAllCache();
        }
        /// <summary>
        /// 清空单个表相关的所有缓存
        /// </summary>
        public void FlushCurrentCollectionCache(string collectionName = null)
        {
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache(collectionName);
            if (CacheOptions.OpenTableCache)
                TableCacheManager.FlushCollectionCache(collectionName);
        }

        public void Add<TEntity>(TEntity entity)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache();
            //2.更新Table缓存中的该表记录
            if (CacheOptions.OpenTableCache)
                TableCacheManager.AddCache(entity);
        }
        public void Add<TEntity>(IEnumerable<TEntity> entities)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache();
            //2.更新Table缓存中的该表记录
            if (CacheOptions.OpenTableCache)
                TableCacheManager.AddCache(entities);
        }
        public void Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache();
            //2.更新Table缓存中的该表记录
            if (CacheOptions.OpenTableCache)
                TableCacheManager.UpdateCache(entity, filter);
        }
        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache();
            //2.更新Table缓存中的该表记录
            if (CacheOptions.OpenTableCache)
                TableCacheManager.DeleteCache(filter);
        }
        public void Delete<TEntity>(TEntity entity)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.FlushCollectionCache();
            //2.更新Table缓存中的该表记录
            if (CacheOptions.OpenTableCache)
                TableCacheManager.DeleteCache(entity);
        }

        public List<TEntity> GetEntities<TEntity>(Expression<Func<TEntity, bool>> filter, Func<List<TEntity>> func) where TEntity : class
        {
            DbContext.IsFromCache = false;

            List<TEntity> result = null;

            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            if (CacheOptions.OpenTableCache)
                result = TableCacheManager.GetEntitiesFromCache(filter);

            if (DbContext.IsFromCache)
                return result;

            //2.判断是否在一级QueryCahe中
            if (CacheOptions.OpenQueryCache)
                result = QueryCacheManager.GetEntitiesFromCache<List<TEntity>>();

            if (DbContext.IsFromCache)
                return result;

            //3.如果都没有，则直接从逻辑中获取

            result = func();

            //4.Query缓存存储逻辑（内涵缓存开启校验）
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.CacheData(result);

            return result;
        }
        public TEntity GetEntity<TEntity>(Expression<Func<TEntity, bool>> filter, Func<TEntity> func) where TEntity : class
        {
            DbContext.IsFromCache = false;

            TEntity result = null;

            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            if (CacheOptions.OpenTableCache)
                result = TableCacheManager.GetEntitiesFromCache(filter)?.FirstOrDefault();

            if (DbContext.IsFromCache)
                return result;

            //2.判断是否在一级QueryCahe中
            if (CacheOptions.OpenQueryCache)
                result = QueryCacheManager.GetEntitiesFromCache<TEntity>();

            if (DbContext.IsFromCache)
                return result;

            //3.如果都没有，则直接从逻辑中获取
            result = func();

            //4.Query缓存存储逻辑（内含缓存开启校验）
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.CacheData(result);

            return result;
        }
        public long GetCount<TEntity>(Expression<Func<TEntity, bool>> filter, Func<long> func) where TEntity : class
        {
            DbContext.IsFromCache = false;

            long? result = null;

            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            if (CacheOptions.OpenTableCache)
                result = TableCacheManager.GetEntitiesFromCache(filter)?.Count;

            if (DbContext.IsFromCache)
                return result ?? default(long);

            //2.判断是否在一级QueryCahe中
            if (CacheOptions.OpenQueryCache)
                result = QueryCacheManager.GetEntitiesFromCache<long?>();

            if (DbContext.IsFromCache)
                return result ?? default(long);

            //3.如果都没有，则直接从逻辑中获取
            result = func();

            //4.Query缓存存储逻辑（内涵缓存开启校验）
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.CacheData(result);

            return result ?? default(long);
        }
        public T GetObject<T>(Func<T> func) where T : class
        {
            DbContext.IsFromCache = false;

            T result = null;

            //1.判断是否在一级QueryCache中
            if (CacheOptions.OpenQueryCache)
                result = QueryCacheManager.GetEntitiesFromCache<T>();

            if (DbContext.IsFromCache)
                return result;

            //2.如果都没有，则直接从逻辑中获取
            result = func();

            //3.Query缓存存储逻辑（内涵缓存开启校验）
            if (CacheOptions.OpenQueryCache)
                QueryCacheManager.CacheData(result);

            return result;
        }
    }
}
