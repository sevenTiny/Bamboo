using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    public interface IDbCacheManager
    {
        void FlushAllCache();
        void FlushCurrentCollectionCache(string collectionName = null);
        void Add<TEntity>(TEntity entity);
        void Add<TEntity>(IEnumerable<TEntity> entities);
        void Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> filter);
        void Delete<TEntity>(TEntity entity);
        TEntity GetEntity<TEntity>(Expression<Func<TEntity, bool>> filter, Func<TEntity> func) where TEntity : class;
        List<TEntity> GetEntities<TEntity>(Expression<Func<TEntity, bool>> filter, Func<List<TEntity>> func) where TEntity : class;
        long GetCount<TEntity>(Expression<Func<TEntity, bool>> filter, Func<long> func) where TEntity : class;
        T GetObject<T>(Func<T> func) where T : class;
    }
}
