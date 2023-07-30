using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate
{
    internal class DbSet
    {
        /// <summary>
        /// DbSet属性初始化
        /// </summary>
        /// <param name="dbContext">数据库操作上下文</param>
        internal static void PropertyInitialization(DbContext dbContext)
        {
            foreach (var item in dbContext.GetType().GetProperties())
            {
                if (item.PropertyType.Name.Equals("DbSet`1"))
                    item.SetValue(dbContext, Activator.CreateInstance(item.PropertyType, new[] { dbContext }));
            }
        }
    }

    /// <summary>
    /// 对象操作实体集合,专用于强类型实体操作
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DbSet<TEntity> : ILinqQueryable<TEntity> where TEntity : class
    {
        public DbSet(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// 操作上下文
        /// </summary>
        private DbContext DbContext { get; set; }

        #region 查询操作
        /// <summary>
        /// 获取查询提供器
        /// </summary>
        private ILinqQueryable<TEntity> Queryable => QueryEngineSelector.Select<TEntity>(DbContext.DataBaseType, DbContext);

        public bool Any() => Queryable.Any();

        public long Count() => Queryable.Count();

        public TEntity FirstOrDefault() => Queryable.FirstOrDefault();

        public ILinqQueryable<TEntity> Limit(int count) => Queryable.Limit(count);

        public ILinqQueryable<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy) => Queryable.OrderBy(orderBy);

        public ILinqQueryable<TEntity> OrderByDescending(Expression<Func<TEntity, object>> orderBy) => Queryable.OrderByDescending(orderBy);

        public ILinqQueryable<TEntity> Paging(int pageIndex, int pageSize) => Queryable.Paging(pageIndex, pageSize);

        public ILinqQueryable<TEntity> Select(Expression<Func<TEntity, object>> columns) => Queryable.Select(columns);

        public object ToData() => Queryable.ToData();

        public DataSet ToDataSet() => Queryable.ToDataSet();

        public List<TEntity> ToList() => Queryable.ToList();

        public ILinqQueryable<TEntity> Where(Expression<Func<TEntity, bool>> filter) => Queryable.Where(filter);
        #endregion
    }
}
