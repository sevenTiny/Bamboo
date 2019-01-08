using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Cache;
using SevenTiny.Bantina.Bankinate.DataAccessEngine;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Helpers;
using SevenTiny.Bantina.Bankinate.SqlStatementManager;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    public class SqlQueryable<TEntity> where TEntity : class
    {
        public SqlQueryable(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private DbContext _dbContext;
        private Expression<Func<TEntity, bool>> _where;

        //orderby
        private Expression<Func<TEntity, object>> _orderby;
        private bool _isDesc = false;

        private List<string> _columns;
        private string _top;
        private string _alias;

        //paging
        private bool _isPaging = false;
        private int _pageIndex = 0;
        private int _pageSize = 0;

        /// <summary>
        /// 必要条件检查
        /// </summary>
        private void MustExistCheck()
        {
            if (_where == null)
            {
                throw new ArgumentNullException("Where condition deficiency");
            }
        }

        /// <summary>
        /// 获取TableName，并将其重新赋值
        /// </summary>
        private void ReSetTableName()
        {
            _dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
        }

        public SqlQueryable<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            if (_where != null)
                _where = _where.And(filter);
            else
                _where = filter;

            _alias = filter.Parameters[0].Name;
            return this;
        }

        public SqlQueryable<TEntity> Select(Expression<Func<TEntity, object>> columns)
        {
            _columns = SqlGenerator.QueryableSelect(_dbContext, columns);
            return this;
        }

        /// <summary>
        /// 取最前面的count行，该方法不能和分页方法连用
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public SqlQueryable<TEntity> Top(int count)
        {
            switch (_dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                    _top = $" TOP {count} "; break;
                case DataBaseType.MySql:
                    _top = $" LIMIT {count} "; break;
                case DataBaseType.Oracle:
                    break;
            }
            return this;
        }

        public SqlQueryable<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            _orderby = orderBy;
            _isDesc = false;
            return this;
        }

        public SqlQueryable<TEntity> OrderByDescending(Expression<Func<TEntity, object>> orderBy)
        {
            _orderby = orderBy;
            _isDesc = true;
            return this;
        }

        public SqlQueryable<TEntity> Paging(int pageIndex, int pageSize)
        {
            _isPaging = true;

            if (pageIndex <= 0)
            {
                pageIndex = 0;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            _pageIndex = pageIndex;
            _pageSize = pageSize;

            return this;
        }

        public List<TEntity> ToList()
        {
            MustExistCheck();
            ReSetTableName();

            if (_isPaging)
            {
                var result = DbCacheManager.GetEntities(_dbContext, _where, () =>
                {
                    _dbContext.SqlStatement = SqlGenerator.QueryablePaging<TEntity>(
                        _dbContext,
                        _columns,
                        _alias,
                        SqlGenerator.QueryableWhere(_dbContext, _where, out IDictionary<string, object> parameters),
                        SqlGenerator.QueryableOrderBy(_dbContext, _orderby, _isDesc),
                        _pageIndex,
                        _pageSize);
                    return DbHelper.ExecuteList<TEntity>(_dbContext.SqlStatement, System.Data.CommandType.Text, parameters);
                });
                return result;
            }
            else
            {
                return DbCacheManager.GetEntities(_dbContext, _where, () =>
                {
                    _dbContext.SqlStatement = SqlGenerator.QueryableQueryList<TEntity>(
                        _dbContext,
                        _columns,
                        _alias,
                        SqlGenerator.QueryableWhere(_dbContext, _where, out IDictionary<string, object> parameters),
                        SqlGenerator.QueryableOrderBy(_dbContext, _orderby, _isDesc),
                        _top);
                    return DbHelper.ExecuteList<TEntity>(_dbContext.SqlStatement, System.Data.CommandType.Text, parameters);
                });
            }
        }

        public TEntity ToEntity()
        {
            MustExistCheck();
            ReSetTableName();

            return default(TEntity);
        }
    }
}
