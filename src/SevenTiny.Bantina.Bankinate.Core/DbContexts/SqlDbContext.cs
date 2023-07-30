using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Configs;
using SevenTiny.Bantina.Bankinate.Helpers;
using SevenTiny.Bantina.Bankinate.SqlDataAccess;
using SevenTiny.Bantina.Bankinate.SqlStatementManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class SqlDbContext : DbContext, IExecuteSql, ICacheable
    {
        protected SqlDbContext(string connectionString_Write, params string[] connectionStrings_Read) : base(connectionString_Write, connectionStrings_Read)
        {
            ConnectionManager.SetConnectionString(OperationType.Write);                     //初始化连接字符串
            DbConnection = CreateDbConnection(ConnectionManager.CurrentConnectionString);   //初始化连接器
            DbCommand = CreateDbCommand();                                                  //初始化命令执行器
            DbDataAdapter = CreateDbDataAdapter();                                          //初始化集合访问器
            AccessorInitializes();                                                          //初始化访问器
            CommandTextGenerator = CreateCommandTextGenerator();                            //初始化SQL生成器
            QueryExecutor = new QueryExecutor(this);                                        //初始化SQL执行器
        }

        #region 数据库管理
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get => CollectionName; internal set => CollectionName = value; }
        /// <summary>
        /// Sql语句，获取或赋值命令行对象的CommandText参数
        /// </summary>
        public string SqlStatement
        {
            get => this.DbCommand?.CommandText;
            internal set
            {
                if (this.DbCommand == null)
                    throw new NullReferenceException("DbCommand is null,please initialize connection first!");
                this.DbCommand.CommandText = value;
            }
        }
        /// <summary>
        /// 参数化查询参数
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// 数据库连接管理器
        /// </summary>
        internal DbConnection DbConnection { get; private set; }
        /// <summary>
        /// 命令管理器
        /// </summary>
        internal DbCommand DbCommand { get; private set; }
        /// <summary>
        /// 结果集访问器
        /// </summary>
        internal DbDataAdapter DbDataAdapter { get; private set; }
        /// <summary>
        /// 命令生成器
        /// </summary>
        internal CommandTextGeneratorBase CommandTextGenerator { get; private set; }
        /// <summary>
        /// 创建连接管理器
        /// </summary>
        /// <param name="connectionString"></param>
        internal abstract DbConnection CreateDbConnection(string connectionString);
        /// <summary>
        /// 创建命令管理器
        /// </summary>
        internal abstract DbCommand CreateDbCommand();
        /// <summary>
        /// 创建结果集访问器
        /// </summary>
        internal abstract DbDataAdapter CreateDbDataAdapter();
        /// <summary>
        /// 创建SQL生成器
        /// </summary>
        internal abstract CommandTextGeneratorBase CreateCommandTextGenerator();
        /// <summary>
        /// 连接状态检查，如果关闭，则打开连接
        /// </summary>
        internal void ConnectionStatusCheck()
        {
            //打开连接
            if (DbConnection.State != ConnectionState.Open)
                DbConnection.Open();
        }
        /// <summary>
        /// 初始化访问器
        /// </summary>
        internal void AccessorInitializes()
        {
            //设置SqlCommand对象的属性值
            DbCommand.CommandTimeout = BankinateConst.CommandTimeout;
        }
        /// <summary>
        /// 初始化查询参数
        /// </summary>
        internal abstract void ParameterInitializes();
        /// <summary>
        /// 查询执行器
        /// </summary>
        internal QueryExecutor QueryExecutor { get; private set; }
        /// <summary>
        /// 根据实体获取表名
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public string GetTableName<TEntity>() where TEntity : class
            => TableAttribute.GetName(typeof(TEntity));
        /// <summary>
        /// 获取一级缓存的缓存键
        /// </summary>
        /// <returns></returns>
        internal override string GetQueryCacheKey()
        {
            //如果有条件，则sql的key要拼接对应的参数值
            if (Parameters != null && Parameters.Any())
            {
                return MD5Helper.GetMd5Hash($"{SqlStatement}_{string.Join("|", Parameters.Values)}");
            }
            return MD5Helper.GetMd5Hash(SqlStatement);
        }
        /// <summary>
        /// 切换数据库连接
        /// </summary>
        /// <param name="operationType"></param>
        private void SwitchConnection(OperationType operationType)
        {
            //获取下次执行的链接字符串
            var connectionString = this.ConnectionManager.SetConnectionString(operationType);
            //如果下次设置的链接字符串和当前的一致，则无需切换
            if (connectionString.Equals(this.DbConnection.ConnectionString))
                return;

            if (this.DbConnection.State != ConnectionState.Closed)
                this.DbConnection.Close();

            this.DbConnection.ConnectionString = connectionString;
        }
        /// <summary>
        /// 是否Sql语句或者存储过程,如果是SQL语句或者存储过程，则查询时不走缓存，且跳过LINQ解析的代码
        /// </summary>
        internal bool IsSqlStatementOrStoredProcedure { get; set; } = false;
        #endregion

        #region 事务控制
        /// <summary>
        /// 快捷事务处理，异常出现事务回滚
        /// </summary>
        /// <param name="action"></param>
        public void Transaction(Action action)
        {
            try
            {
                this.DbCommand.Transaction = this.DbConnection.BeginTransaction();
                action();
                this.DbCommand.Transaction.Commit();
            }
            catch (Exception ex)
            {
                this.DbCommand.Transaction.Rollback();
                throw ex;
            }
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        public void TransactionBegin()
        {
            this.DbConnection.BeginTransaction();
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void TransactionCommit()
        {
            this.DbCommand.Transaction.Commit();
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void TransactionRollback()
        {
            this.DbCommand.Transaction.Rollback();
        }
        #endregion

        #region 强类型的执行操作API
        public override void Add<TEntity>(TEntity entity)
        {
            DataValidatorSafeExecute(v => v.Verify(entity));
            DbCommand.CommandType = CommandType.Text;
            this.CommandTextGenerator.Add(entity);
            this.SwitchConnection(OperationType.Write);
            this.QueryExecutor.ExecuteNonQuery();
            this.DbCacheManagerSafeExecute(m => m.Add(entity));
        }
        public override async Task AddAsync<TEntity>(TEntity entity)
        {
            DataValidatorSafeExecute(v => v.Verify(entity));
            DbCommand.CommandType = CommandType.Text;
            this.CommandTextGenerator.Add(entity);
            this.SwitchConnection(OperationType.Write);
            await QueryExecutor.ExecuteNonQueryAsync();
            this.DbCacheManagerSafeExecute(m => m.Add(entity));
        }
        public void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            this.SwitchConnection(OperationType.Write);
            DbCommand.CommandType = CommandType.Text;
            foreach (var entity in entities)
            {
                DataValidatorSafeExecute(v => v.Verify(entity));
                this.CommandTextGenerator.Add(entity);
                this.QueryExecutor.ExecuteNonQuery();
            }
            this.DbCacheManagerSafeExecute(m => m.Add(entities));
        }
        public async Task AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            this.SwitchConnection(OperationType.Write);
            DbCommand.CommandType = CommandType.Text;
            foreach (var entity in entities)
            {
                DataValidatorSafeExecute(v => v.Verify(entity));
                this.CommandTextGenerator.Add(entity);
                await this.QueryExecutor.ExecuteNonQueryAsync();
            }
            this.DbCacheManagerSafeExecute(m => m.Add(entities));
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            DbCommand.CommandType = CommandType.Text;
            this.CommandTextGenerator.Delete(entity);
            this.SwitchConnection(OperationType.Write);
            QueryExecutor.ExecuteNonQuery();
            this.DbCacheManagerSafeExecute(m => m.Delete(entity));
        }
        public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            DbCommand.CommandType = CommandType.Text;
            this.CommandTextGenerator.Delete(entity);
            this.SwitchConnection(OperationType.Write);
            await QueryExecutor.ExecuteNonQueryAsync();
            this.DbCacheManagerSafeExecute(m => m.Delete(entity));
        }
        public override void Delete<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            DbCommand.CommandType = CommandType.Text;
            this.CommandTextGenerator.Delete(filter);
            this.SwitchConnection(OperationType.Write);
            this.QueryExecutor.ExecuteNonQuery();
            this.DbCacheManagerSafeExecute(m => m.Delete(filter));
        }
        public override async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            DbCommand.CommandType = CommandType.Text;
            this.CommandTextGenerator.Delete(filter);
            this.SwitchConnection(OperationType.Write);
            await QueryExecutor.ExecuteNonQueryAsync();
            this.DbCacheManagerSafeExecute(m => m.Delete(filter));
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            DataValidatorSafeExecute(v => v.Verify(entity));
            DbCommand.CommandType = CommandType.Text;
            this.CommandTextGenerator.Update(entity, out Expression<Func<TEntity, bool>> filter);
            this.SwitchConnection(OperationType.Write);
            QueryExecutor.ExecuteNonQuery();
            this.DbCacheManagerSafeExecute(m => m.Update(entity, filter));
        }
        public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            DbCommand.CommandType = CommandType.Text;
            DataValidatorSafeExecute(v => v.Verify(entity));
            this.CommandTextGenerator.Update(entity, out Expression<Func<TEntity, bool>> filter);
            this.SwitchConnection(OperationType.Write);
            await QueryExecutor.ExecuteNonQueryAsync();
            this.DbCacheManagerSafeExecute(m => m.Update(entity, filter));
        }
        public override void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity)
        {
            DbCommand.CommandType = CommandType.Text;
            DataValidatorSafeExecute(v => v.Verify(entity));
            this.CommandTextGenerator.Update(filter, entity);
            this.SwitchConnection(OperationType.Write);
            QueryExecutor.ExecuteNonQuery();
            this.DbCacheManagerSafeExecute(m => m.Update(entity, filter));
        }
        public override async Task UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity)
        {
            DbCommand.CommandType = CommandType.Text;
            DataValidatorSafeExecute(v => v.Verify(entity));
            this.CommandTextGenerator.Update(filter, entity);
            this.SwitchConnection(OperationType.Write);
            await QueryExecutor.ExecuteNonQueryAsync();
            this.DbCacheManagerSafeExecute(m => m.Update(entity, filter));
        }
        #endregion

        #region SQL执行操作Api
        public int ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            DbCommand.CommandType = CommandType.Text;
            this.SqlStatement = sqlStatement;
            this.Parameters = parms;
            this.SwitchConnection(OperationType.Write);
            this.IsSqlStatementOrStoredProcedure = true;
            return QueryExecutor.ExecuteNonQuery();
        }
        public async Task<int> ExecuteSqlAsync(string sqlStatement, IDictionary<string, object> parms = null)
        {
            DbCommand.CommandType = CommandType.Text;
            this.SqlStatement = sqlStatement;
            this.Parameters = parms;
            this.SwitchConnection(OperationType.Write);
            this.IsSqlStatementOrStoredProcedure = true;
            return await QueryExecutor.ExecuteNonQueryAsync();
        }
        public int ExecuteStoredProcedure(string storedProcedureName, IDictionary<string, object> parms = null)
        {
            this.SqlStatement = storedProcedureName;
            this.Parameters = parms;
            DbCommand.CommandType = CommandType.StoredProcedure;
            this.SwitchConnection(OperationType.Write);
            this.IsSqlStatementOrStoredProcedure = true;
            return QueryExecutor.ExecuteNonQuery();
        }
        public async Task<int> ExecuteStoredProcedureAsync(string storedProcedureName, IDictionary<string, object> parms = null)
        {
            this.SqlStatement = storedProcedureName;
            this.Parameters = parms;
            DbCommand.CommandType = CommandType.StoredProcedure;
            this.SwitchConnection(OperationType.Write);
            this.IsSqlStatementOrStoredProcedure = true;
            return await QueryExecutor.ExecuteNonQueryAsync();
        }
        #endregion

        #region 强类型查询API
        /// <summary>
        /// 强类型Linq查询器
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public ILinqQueryable<TEntity> Queryable<TEntity>() where TEntity : class
        {
            this.DbCommand.CommandType = CommandType.Text;
            //重置命令生成器，防止上次查询参数被重用
            this.CommandTextGenerator = this.CreateCommandTextGenerator();
            this.SwitchConnection(OperationType.Read);
            return new SqlQueryable<TEntity>(this);
        }
        /// <summary>
        /// 强类型Sql查询器
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> Queryable<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class
        {
            this.DbCommand.CommandType = CommandType.Text;
            this.SqlStatement = sqlStatement;
            this.Parameters = parms;
            this.SwitchConnection(OperationType.Read);
            this.IsSqlStatementOrStoredProcedure = true;
            return new SqlQueryable<TEntity>(this);
        }
        /// <summary>
        /// 强类型存储过程查询器
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> StoredProcedureQueryable<TEntity>(string storedProcedureName, IDictionary<string, object> parms = null) where TEntity : class
        {
            this.DbCommand.CommandType = CommandType.StoredProcedure;
            this.SqlStatement = storedProcedureName;
            this.Parameters = parms;
            this.SwitchConnection(OperationType.Read);
            this.IsSqlStatementOrStoredProcedure = true;
            return new SqlQueryable<TEntity>(this);
        }
        #endregion

        public new void Dispose()
        {
            //释放资源
            if (this.DbDataAdapter != null)
                this.DbDataAdapter.Dispose();

            if (this.DbCommand != null)
                this.DbCommand.Dispose();

            if (this.DbConnection.State == ConnectionState.Open)
                this.DbConnection.Close();
            if (this.DbConnection != null)
                this.DbConnection.Dispose();

            base.Dispose();
        }
    }
}
