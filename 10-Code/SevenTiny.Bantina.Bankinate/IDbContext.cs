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
using System.Data;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 通用的Api接口，具备基础的操作，缓存
    /// </summary>
    public interface IDbContext : IDisposable, IBaseOerate, ICacheable
    {
    }

    /// <summary>
    /// 基础操作Api
    /// </summary>
    public interface IBaseOerate
    {
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void AddAsync<TEntity>(TEntity entity) where TEntity : class;
        void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
        void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;

        void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

        QueryableBase<TEntity> Queryable<TEntity>() where TEntity:class;

        List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        bool QueryExist<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
    }

    /// <summary>
    /// 执行sql语句扩展Api
    /// </summary>
    public interface IExecuteSqlOperate
    {
        void ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null);
        void ExecuteSqlAsync(string sqlStatement, IDictionary<string, object> parms = null);
        DataSet ExecuteQueryDataSetSql(string sqlStatement, IDictionary<string, object> parms = null);
        object ExecuteQueryOneDataSql(string sqlStatement, IDictionary<string, object> parms = null);
        TEntity ExecuteQueryOneSql<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class;
        List<TEntity> ExecuteQueryListSql<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class;
    }

    /// <summary>
    /// 缓存接口，实现该接口的类必须具备ORM缓存
    /// </summary>
    public interface ICacheable
    {
    }
}
