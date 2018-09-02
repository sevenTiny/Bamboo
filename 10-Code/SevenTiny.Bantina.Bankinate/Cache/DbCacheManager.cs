using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate.Cache
{
    /// <summary>
    /// 数据库缓存管理器
    /// Bankinate的缓存是分为两级的，每级都有对应的开关
    /// 一级缓存（QueryCache查询缓存），缓存简短查询中的缓存数据
    /// 二级缓存（TableCache表缓存），缓存整张表，需要标签配合使用
    /// </summary>
    internal abstract class DbCacheManager
    {
        /// <summary>
        /// 清空所有缓存
        /// </summary>
        internal static void FlushAllCache(DbContext dbContext)
        {
            QueryCacheManager.FlushAllCache(dbContext);
            TableCacheManager.FlushAllCache(dbContext);
        }
        /// <summary>
        /// 清空单个表相关的所有缓存
        /// </summary>
        internal static void FlushTableCache(DbContext dbContext)
        {
            QueryCacheManager.FlushTableCache(dbContext);
            TableCacheManager.FlushTableCache(dbContext);
        }

        internal static void Add<TEntity>(DbContext dbContext, TEntity entity)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            QueryCacheManager.FlushTableCache(dbContext);
            //2.更新Table缓存中的该表记录
            TableCacheManager.AddCache(dbContext, entity);
        }
        internal static void Add<TEntity>(DbContext dbContext, IEnumerable<TEntity> entities)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            QueryCacheManager.FlushTableCache(dbContext);
            //2.更新Table缓存中的该表记录
            TableCacheManager.AddCache(dbContext, entities);
        }
        internal static void Update<TEntity>(DbContext dbContext, TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            QueryCacheManager.FlushTableCache(dbContext);
            //2.更新Table缓存中的该表记录
            TableCacheManager.UpdateCache(dbContext, entity, filter);
        }
        internal static void Delete<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            QueryCacheManager.FlushTableCache(dbContext);
            //2.更新Table缓存中的该表记录
            TableCacheManager.DeleteCache(dbContext, filter);
        }

        internal static List<TEntity> GetEntities<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter, Func<List<TEntity>> func) where TEntity : class
        {
            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            var entities = TableCacheManager.GetEntitiesFromCache(dbContext, filter);

            //2.判断是否在一级QueryCahe中
            if (entities == null || !entities.Any())
            {
                entities = QueryCacheManager.GetEntitiesFromCache<List<TEntity>>(dbContext);
            }

            //3.如果都没有，则直接从逻辑中获取
            if (entities == null || !entities.Any())
            {
                entities = func();
                dbContext.IsFromCache = false;
                //4.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(dbContext, entities);
            }

            return entities;
        }
        internal static TEntity GetEntity<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter, Func<TEntity> func) where TEntity : class
        {
            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            var result = TableCacheManager.GetEntitiesFromCache(dbContext, filter)?.FirstOrDefault();

            //2.判断是否在一级QueryCahe中
            if (result == null)
            {
                result = QueryCacheManager.GetEntitiesFromCache<TEntity>(dbContext);
            }

            //3.如果都没有，则直接从逻辑中获取
            if (result == null)
            {
                result = func();
                dbContext.IsFromCache = false;
                //4.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(dbContext, result);
            }

            return result;
        }
        internal static int GetCount<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter, Func<int> func) where TEntity : class
        {
            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            var result = TableCacheManager.GetEntitiesFromCache(dbContext, filter)?.Count;

            //2.判断是否在一级QueryCahe中
            if (result == null)
            {
                result = QueryCacheManager.GetEntitiesFromCache<int>(dbContext);
            }

            //3.如果都没有，则直接从逻辑中获取
            if (result == null || result == default(int))
            {
                result = func();
                dbContext.IsFromCache = false;
                //4.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(dbContext, result);
            }

            return result ?? default(int);
        }
        internal static T GetObject<T>(DbContext dbContext, Func<T> func) where T : class
        {
            //1.判断是否在一级QueryCahe中
            var result = QueryCacheManager.GetEntitiesFromCache<T>(dbContext);

            //2.如果都没有，则直接从逻辑中获取
            if (result == null)
            {
                result = func();
                dbContext.IsFromCache = false;
                //3.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(dbContext, result);
            }

            return result;
        }
    }
}
