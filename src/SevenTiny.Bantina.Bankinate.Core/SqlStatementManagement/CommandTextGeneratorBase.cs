using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SevenTiny.Bantina.Bankinate.SqlStatementManagement
{
    /// <summary>
    /// 命令生成规则的基类
    /// </summary>
    internal abstract class CommandTextGeneratorBase
    {
        public CommandTextGeneratorBase(SqlDbContext _dbContext)
        {
            DbContext = _dbContext;
        }

        //context
        protected SqlDbContext DbContext;
        protected string _where;
        protected string _orderBy;
        protected int _pageIndex;
        protected int _pageSize;
        protected string _alias;
        protected List<string> _columns;
        protected string _limit;

        #region 设置关键字
        public abstract void SetWhere<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        public abstract void SetOrderBy<TEntity>(Expression<Func<TEntity, object>> orderBy, bool isDesc) where TEntity : class;
        public abstract void SetPage(int pageIndex, int pageSize);
        public abstract void SetLimit(int count);
        public abstract void SetAlias(string alias);
        public abstract void SetColumns<TEntity>(Expression<Func<TEntity, object>> columns) where TEntity : class;
        #endregion

        //Cache properties by type
        private static ConcurrentDictionary<Type, PropertyInfo[]> _propertiesDic = new ConcurrentDictionary<Type, PropertyInfo[]>();
        protected static PropertyInfo[] GetPropertiesDicByType(Type type)
        {
            _propertiesDic.AddOrUpdate(type, type.GetProperties());
            return _propertiesDic[type];
        }

        public abstract string Add<TEntity>(TEntity entity) where TEntity : class;
        public abstract string Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
        public abstract string Update<TEntity>(TEntity entity, out Expression<Func<TEntity, bool>> filter) where TEntity : class;
        public abstract string Delete<TEntity>(TEntity entity) where TEntity : class;
        public abstract string Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

        #region Queryable Methods,返回的都是最终结果
        public abstract string QueryableCount<TEntity>() where TEntity : class;
        public abstract string QueryableAny<TEntity>() where TEntity : class;
        public abstract string QueryableQuery<TEntity>() where TEntity : class;
        public abstract string QueryablePaging<TEntity>() where TEntity : class;
        #endregion

    }
}
