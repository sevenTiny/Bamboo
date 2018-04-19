using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MySqlDbContext<TDataBase> : IDbContext where TDataBase : class
    {
        public MySqlDbContext(string connectionString)
        {

        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Add<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void AddAsync<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public List<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            throw new NotImplementedException();
        }
        public TEntity QueryOne<TEntity>(string id) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            throw new NotImplementedException();
        }

        public object ExecuteQuerySql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            throw new NotImplementedException();
        }

        public TEntity QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}
