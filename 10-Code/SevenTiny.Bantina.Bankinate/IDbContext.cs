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
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    public interface IDbContext : IDisposable, IAddOperate, IUpdateOperate, IDeleteOperate, IQueryOperate
    {

    }

    public interface IAddOperate
    {
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void AddAsync<TEntity>(TEntity entity) where TEntity : class;
        void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    }

    public interface IUpdateOperate
    {
        void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
        void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
    }

    public interface IDeleteOperate
    {
        void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
    }

    public interface IExecuteSqlOperate
    {
        void ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null);
        void ExecuteSqlAsync(string sqlStatement, IDictionary<string, object> parms = null);
        object ExecuteQueryObjectSql(string sqlStatement, IDictionary<string, object> parms = null);
        TEntity ExecuteQueryOneSql<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class;
        List<TEntity> ExecuteQueryListSql<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class;
    }

    public interface IQueryOperate
    {
        bool QueryExist<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        List<TEntity> QueryList<TEntity>() where TEntity : class;
        List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
    }

    public interface IQueryPagingOperate
    {
        List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, bool isDESC = false) where TEntity : class;
        List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, Expression<Func<TEntity, bool>> filter, bool isDESC = false) where TEntity : class;
        List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, out int count, bool isDESC = false) where TEntity : class;
        List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, Expression<Func<TEntity, bool>> filter, out int count, bool isDESC = false) where TEntity : class;
    }
}
