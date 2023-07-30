using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// Linq语法查询支持
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ILinqQueryable<TEntity> : IQueryable<TEntity>
    {
        /// <summary>
        /// 查询出符合当前条件的数据条数
        /// </summary>
        /// <returns></returns>
        long Count();
        /// <summary>
        /// 查询是否有符合条件的记录数
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool Any();

        ILinqQueryable<TEntity> Where(Expression<Func<TEntity, bool>> filter);
        ILinqQueryable<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy);
        ILinqQueryable<TEntity> OrderByDescending(Expression<Func<TEntity, object>> orderBy);
        ILinqQueryable<TEntity> Paging(int pageIndex, int pageSize);
        ILinqQueryable<TEntity> Select(Expression<Func<TEntity, object>> columns);
        ILinqQueryable<TEntity> Limit(int count);
    }
}
