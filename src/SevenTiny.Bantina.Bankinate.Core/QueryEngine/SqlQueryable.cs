/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 1/8/2019, 5:31:04 PM
* Modify: 2019年4月26日16点35分
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 适合于Sql条件下的懒加载查询配置
* Thx , Best Regards ~
*********************************************************/
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using SevenTiny.Bantina.Bankinate.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// SQL强类型复杂查询器
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class SqlQueryable<TEntity> : SqlQueryableBase<TEntity>, ILinqQueryable<TEntity> where TEntity : class
    {
        private string Alias => _where.Parameters[0].Name;

        internal SqlQueryable(SqlDbContext dbContext) : base(dbContext)
        {
        }

        public ILinqQueryable<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            Ensure.ArgumentNotNullOrEmpty(filter, nameof(filter));

            _where = _where.And(filter);

            return this;
        }

        public ILinqQueryable<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            Ensure.ArgumentNotNullOrEmpty(orderBy, nameof(orderBy));

            _orderby = orderBy;
            _isDesc = false;

            return this;
        }

        public ILinqQueryable<TEntity> OrderByDescending(Expression<Func<TEntity, object>> orderBy)
        {
            Ensure.ArgumentNotNullOrEmpty(orderBy, nameof(orderBy));

            _orderby = orderBy;
            _isDesc = true;

            return this;
        }

        public ILinqQueryable<TEntity> Paging(int pageIndex, int pageSize)
        {
            _isPaging = true;

            if (pageIndex <= 0)
                pageIndex = 0;

            if (pageSize <= 0)
                pageSize = 10;

            _pageIndex = pageIndex;
            _pageSize = pageSize;
            return this;
        }

        /// <summary>
        /// 筛选具体的某几列
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public ILinqQueryable<TEntity> Select(Expression<Func<TEntity, object>> columns)
        {
            _columns = columns;
            return this;
        }

        /// <summary>
        /// 取最前面的count行，该方法不能和分页方法连用
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public ILinqQueryable<TEntity> Limit(int count)
        {
            _dbContext.CommandTextGenerator.SetLimit(count);
            return this;
        }

        public List<TEntity> ToList()
        {
            if (_dbContext.IsSqlStatementOrStoredProcedure)
                return _dbContext.QueryExecutor.ExecuteList<TEntity>();

            MustExistCheck();
            ReSetTableName();

            _dbContext.CommandTextGenerator.SetAlias(Alias);
            _dbContext.CommandTextGenerator.SetColumns(_columns);
            _dbContext.CommandTextGenerator.SetWhere(_where);
            _dbContext.CommandTextGenerator.SetOrderBy(_orderby, _isDesc);

            if (_isPaging)
            {
                _dbContext.CommandTextGenerator.SetPage(_pageIndex, _pageSize);
                _dbContext.CommandTextGenerator.QueryablePaging<TEntity>();
            }
            else
            {
                _dbContext.CommandTextGenerator.QueryableQuery<TEntity>();
            }

            return _dbContext.DbCacheManagerSafeExecute((m, r) => m.GetEntities(_where, r), () =>
            {
                return _dbContext.QueryExecutor.ExecuteList<TEntity>();
            });
        }

        public TEntity FirstOrDefault()
        {
            if (_dbContext.IsSqlStatementOrStoredProcedure)
                return _dbContext.QueryExecutor.ExecuteEntity<TEntity>();

            MustExistCheck();
            ReSetTableName();

            _dbContext.CommandTextGenerator.SetAlias(Alias);
            _dbContext.CommandTextGenerator.SetColumns(_columns);
            _dbContext.CommandTextGenerator.SetWhere(_where);
            _dbContext.CommandTextGenerator.SetOrderBy(_orderby, _isDesc);
            Limit(1);
            _dbContext.CommandTextGenerator.QueryableQuery<TEntity>();

            return _dbContext.DbCacheManagerSafeExecute((m, r) => m.GetEntity(_where, r), () =>
            {
                return _dbContext.QueryExecutor.ExecuteEntity<TEntity>();
            });
        }

        public long Count()
        {
            MustExistCheck();
            ReSetTableName();

            _dbContext.CommandTextGenerator.SetAlias(Alias);
            _dbContext.CommandTextGenerator.SetWhere(_where);
            _dbContext.CommandTextGenerator.QueryableCount<TEntity>();

            return _dbContext.DbCacheManagerSafeExecute((m, r) => m.GetCount(_where, r), () =>
            {
                return Convert.ToInt64(_dbContext.QueryExecutor.ExecuteScalar());
            });
        }

        public bool Any()
        {
            MustExistCheck();
            ReSetTableName();

            _dbContext.CommandTextGenerator.SetAlias(Alias);
            _dbContext.CommandTextGenerator.SetWhere(_where);
            _dbContext.CommandTextGenerator.QueryableAny<TEntity>(); //内部 Limit(1)

            return _dbContext.DbCacheManagerSafeExecute((m, r) => m.GetCount(_where, r), () =>
            {
                return Convert.ToInt64(_dbContext.QueryExecutor.ExecuteScalar());
            }) > 0;
        }

        public object ToData()
        {
            if (_dbContext.IsSqlStatementOrStoredProcedure)
                return _dbContext.QueryExecutor.ExecuteScalar();

            MustExistCheck();
            ReSetTableName();

            _dbContext.CommandTextGenerator.SetAlias(Alias);
            _dbContext.CommandTextGenerator.SetColumns(_columns);
            _dbContext.CommandTextGenerator.SetWhere(_where);
            _dbContext.CommandTextGenerator.SetOrderBy(_orderby, _isDesc);
            Limit(1);
            _dbContext.CommandTextGenerator.QueryableQuery<TEntity>();

            return _dbContext.QueryExecutor.ExecuteScalar();
        }

        public DataSet ToDataSet()
        {
            if (_dbContext.IsSqlStatementOrStoredProcedure)
                return _dbContext.QueryExecutor.ExecuteDataSet();

            MustExistCheck();
            ReSetTableName();

            _dbContext.CommandTextGenerator.SetAlias(Alias);
            _dbContext.CommandTextGenerator.SetColumns(_columns);
            _dbContext.CommandTextGenerator.SetWhere(_where);
            _dbContext.CommandTextGenerator.SetOrderBy(_orderby, _isDesc);

            if (_isPaging)
            {
                _dbContext.CommandTextGenerator.SetPage(_pageIndex, _pageSize);
                _dbContext.CommandTextGenerator.QueryablePaging<TEntity>();
            }
            else
            {
                _dbContext.CommandTextGenerator.QueryableQuery<TEntity>();
            }

            return _dbContext.QueryExecutor.ExecuteDataSet();
        }
    }
}
