using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    public interface IDbContext : IDisposable
    {
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void AddAsync<TEntity>(TEntity entity) where TEntity : class;
        void Add<TEntity>(IList<TEntity> entities) where TEntity : class;
        void AddAsync<TEntity>(IList<TEntity> entities) where TEntity : class;
        void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
        void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
        void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        bool QueryExist<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        TEntity QueryOne<TEntity>(string id) where TEntity : class;
        TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        List<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        void ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null);
        object ExecuteQuerySql(string sqlStatement, IDictionary<string, object> parms = null);
    }
}
