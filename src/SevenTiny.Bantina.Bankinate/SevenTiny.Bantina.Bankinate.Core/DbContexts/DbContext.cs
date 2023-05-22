using SevenTiny.Bantina.Bankinate.ConnectionManagement;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    /// <summary>
    /// 数据上下文
    /// </summary>
    public abstract class DbContext : IDbContext
    {
        protected DbContext(string connectionString_Write, params string[] connectionStrings_Read)
        {
            if (string.IsNullOrEmpty(connectionString_Write))
                throw new ArgumentNullException(nameof(connectionString_Write), "argument can not be null");

            if (ConnectionManager == null)
                ConnectionManager = new ConnectionManager(connectionString_Write, connectionStrings_Read);

            //初始化DbSet字段值
            DbSet.PropertyInitialization(this);
        }

        #region Database Control 数据库管理
        /// <summary>
        /// 库名（对应SQL数据库的库名）
        /// </summary>
        public string DataBaseName { get; internal set; }
        /// <summary>
        /// 集合名（对应SQL数据库的表，MongoDB的文档名）
        /// </summary>
        public string CollectionName { get; internal set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DataBaseType { get; protected set; }
        /// <summary>
        /// 连接管理器
        /// </summary>
        internal ConnectionManager ConnectionManager { get; }
        /// <summary>
        /// 真实执行持久化操作开关，如果为false，则只执行准备动作，不实际操作数据库（友情提示：测试框架代码执行情况可以将其关闭）
        /// </summary>
        public bool RealExecutionSaveToDb { get; protected set; } = true;
        #endregion

        #region Cache Control 缓存管理
        /// <summary>
        /// 缓存管理器，构造函数赋值，使用提供的执行器访问
        /// </summary>
        private IDbCacheManager DbCacheManager { get; set; }
        /// <summary>
        /// 缓存管理器初始化，用于在扩展组件中提供快捷初始化方法
        /// </summary>
        /// <param name="dbCacheManager"></param>
        protected internal void OpenCache(IDbCacheManager dbCacheManager)
        {
            if (DbCacheManager != null)
                throw new InvalidOperationException("DbCacheManager has been Initialized.");

            DbCacheManager = dbCacheManager;
        }
        /// <summary>
        /// 缓存管理执行器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheExecute"></param>
        /// <param name="realQueryFunc"></param>
        /// <returns></returns>
        internal T DbCacheManagerSafeExecute<T>(Func<IDbCacheManager, Func<T>, T> cacheExecute, Func<T> realQueryFunc)
        {
            if (DbCacheManager != null)
                return cacheExecute(DbCacheManager, realQueryFunc);
            else
                return realQueryFunc();
        }
        /// <summary>
        /// 缓存管理执行器
        /// </summary>
        /// <param name="cacheExecute"></param>
        /// <param name="realQueryFunc"></param>
        internal void DbCacheManagerSafeExecute(Action<IDbCacheManager> cacheExecute)
        {
            if (DbCacheManager != null)
                cacheExecute(DbCacheManager);
        }
        /// <summary>
        /// 标记数据是否从缓存中获取
        /// </summary>
        public bool IsFromCache { get; internal set; } = false;
        /// <summary>
        /// 刷新全部缓存区
        /// </summary>
        public void FlushAllCache()
        {
            if (DbCacheManager == null)
                throw new InvalidOperationException("DbCacheManager is undefined, please define it first.");

            DbCacheManager.FlushAllCache();
        }
        /// <summary>
        /// 刷新当前操作对象缓存区
        /// </summary>
        /// <param name="collectionName"></param>
        public void FlushCurrentCollectionCache(string collectionName = null)
        {
            if (DbCacheManager == null)
                throw new InvalidOperationException("DbCacheManager is undefined, please define it first.");

            DbCacheManager.FlushCurrentCollectionCache(collectionName);
        }
        /// <summary>
        /// 获取一级缓存的缓存键；如SQL中的sql语句和参数，作为一级缓存查询的key，这里根据不同的数据库自定义拼接
        /// </summary>
        /// <returns></returns>
        internal abstract string GetQueryCacheKey();
        /// <summary>
        /// 获取集合全部数据的内置方法，用于二级缓存
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        internal abstract List<TEntity> GetFullCollectionData<TEntity>() where TEntity : class;
        #endregion

        #region Validate Control 数据验证管理
        /// <summary>
        /// 属性值校验器，构造函数赋值，使用提供的执行器访问
        /// </summary>
        private IDataValidator DataValidator { get; set; }
        /// <summary>
        /// 校验属性值校验器初始化，用于在扩展组件中提供快捷初始化方法
        /// </summary>
        /// <param name="dbCacheManager"></param>
        protected internal void OpenDataValidation(IDataValidator dataValidator)
        {
            if (DataValidator != null)
                throw new InvalidOperationException("DataValidator has been Initialized.");

            DataValidator = dataValidator;
        }

        /// <summary>
        /// 校验执行器
        /// </summary>
        /// <param name="action"></param>
        internal void DataValidatorSafeExecute(Action<IDataValidator> action)
        {
            if (DataValidator != null)
                action(DataValidator);
        }
        #endregion

        #region Operate 标准API
        public abstract void Add<TEntity>(TEntity entity) where TEntity : class;
        public abstract Task AddAsync<TEntity>(TEntity entity) where TEntity : class;

        public abstract void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
        public abstract Task UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;

        public abstract void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        public abstract Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

        #endregion

        public void Dispose()
        {
        }
    }
}
