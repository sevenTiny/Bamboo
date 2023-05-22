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
* Description: 适合于NoSql条件下的懒加载查询配置
* Thx , Best Regards ~
*********************************************************/
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// NoSQL强类型复杂查询器
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class NoSqlQueryable<TEntity> : NoSqlQueryableBase<TEntity>, ILinqQueryable<TEntity> where TEntity : class
    {
        public NoSqlQueryable(NoSqlDbContext _dbContext) : base(_dbContext)
        {
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public long Count()
        {
            throw new NotImplementedException();
        }

        public TEntity FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public ILinqQueryable<TEntity> Limit(int count)
        {
            throw new NotImplementedException();
        }

        public ILinqQueryable<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            throw new NotImplementedException();
        }

        public ILinqQueryable<TEntity> OrderByDescending(Expression<Func<TEntity, object>> orderBy)
        {
            throw new NotImplementedException();
        }

        public ILinqQueryable<TEntity> Paging(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public ILinqQueryable<TEntity> Select(Expression<Func<TEntity, object>> columns)
        {
            throw new NotImplementedException();
        }

        public object ToData()
        {
            throw new NotImplementedException();
        }

        public DataSet ToDataSet()
        {
            throw new NotImplementedException();
        }

        public List<TEntity> ToList()
        {
            throw new NotImplementedException();
        }

        public ILinqQueryable<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            if (_where != null)
                _where = _where.And(filter);
            else
                _where = filter;
            return this;
        }

        //public NoSqlQueryable<TEntity> Paging(int pageIndex, int pageSize)
        //{
        //    _isPaging = true;

        //    if (pageIndex <= 0)
        //        pageIndex = 0;

        //    if (pageSize <= 0)
        //        pageSize = 10;

        //    _pageIndex = pageIndex;
        //    _pageSize = pageSize;
        //    return this;
        //}
    }
}
