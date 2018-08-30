using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Cache;
using SevenTiny.Bantina.Bankinate.SqlStatementManager;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class SqlDbContext<TDataBase> : DbContext, IDbContext, IExecuteSqlOperate, IQueryPagingOperate where TDataBase : class
    {
        public SqlDbContext(string connectionString, DataBaseType dataBaseType) : this(connectionString, connectionString, dataBaseType)
        {
        }

        public SqlDbContext(string connectionString_Read, string connectionString_ReadWrite, DataBaseType dataBaseType) : base(dataBaseType)
        {
            DbHelper.ConnString_R = connectionString_Read;
            DbHelper.ConnString_RW = connectionString_ReadWrite;
            DbHelper.DbType = dataBaseType;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            SqlStatement = SqlGenerator.Add(DataBaseType, entity, out string tableName, out Dictionary<string, object> paramsDic);
            TableName = tableName;

            MCache.Instance.MarkTableModifyAdd(TableName, entity);

            DbHelper.ExecuteNonQuery(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }
        public void AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            SqlStatement = SqlGenerator.Add(DataBaseType, entity, out string tableName, out Dictionary<string, object> paramsDic);
            TableName = tableName;

            MCache.Instance.MarkTableModifyAdd(TableName, entity);

            DbHelper.ExecuteNonQueryAsync(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }
        public void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            List<BatchExecuteModel> batchExecuteModels = new List<BatchExecuteModel>();
            foreach (var item in entities)
            {
                SqlStatement = SqlGenerator.Add(DataBaseType, item, out string tableName, out Dictionary<string, object> paramsDic);
                TableName = tableName;
                batchExecuteModels.Add(new BatchExecuteModel
                {
                    CommandTextOrSpName = SqlStatement,
                    ParamsDic = paramsDic
                });
            }
            DbHelper.BatchExecuteNonQuery(batchExecuteModels);
        }
        public void AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            List<BatchExecuteModel> batchExecuteModels = new List<BatchExecuteModel>();
            foreach (var item in entities)
            {
                SqlStatement = SqlGenerator.Add(DataBaseType, item, out string tableName, out Dictionary<string, object> paramsDic);
                TableName = tableName;
                batchExecuteModels.Add(new BatchExecuteModel
                {
                    CommandTextOrSpName = SqlStatement,
                    ParamsDic = paramsDic
                });
            }
            DbHelper.BatchExecuteNonQueryAsync(batchExecuteModels);
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"DELETE {filter.Parameters[0].Name} From {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";

            MCache.Instance.MarkTableModifyDelete(TableName, filter);

            DbHelper.ExecuteNonQuery(SqlStatement);
        }
        public void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"DELETE {filter.Parameters[0].Name} From {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";

            MCache.Instance.MarkTableModifyDelete(TableName, filter);

            DbHelper.ExecuteNonQueryAsync(SqlStatement);
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            SqlStatement = SqlGenerator.Update(DataBaseType, filter, entity, out string tableName, out Dictionary<string, object> paramsDic);
            TableName = tableName;

            MCache.Instance.MarkTableModifyUpdate(TableName, filter, entity);

            DbHelper.ExecuteNonQuery(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }
        public void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            SqlStatement = SqlGenerator.Update(DataBaseType, filter, entity, out string tableName, out Dictionary<string, object> paramsDic);
            TableName = tableName;

            MCache.Instance.MarkTableModifyUpdate(TableName, filter, entity);

            DbHelper.ExecuteNonQueryAsync(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }

        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlStatement = SqlGenerator.Query(DataBaseType, filter, out string tableName);
            TableName = tableName;

            var result = MCache.Instance.GetFromCacheIfNotExistReStore_Entities(LocalCache, TableName, SqlStatement, filter, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out bool fromCache);

            IsFromCache = fromCache;

            return result;
        }
        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> orderBy, bool isDESC = false) where TEntity : class
        {
            SqlStatement = SqlGenerator.QueryOrderBy(DataBaseType, filter, orderBy, isDESC, out string tableName);
            TableName = tableName;

            var result = MCache.Instance.GetFromCacheIfNotExistReStore_Entities(LocalCache, TableName, SqlStatement, filter, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out bool fromCache);

            IsFromCache = fromCache;

            return result;
        }

        public TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlStatement = SqlGenerator.QueryOne(DataBaseType, filter, out string tableName);
            TableName = tableName;

            var result = MCache.Instance.GetFromCacheIfNotExistReStore_Entity(LocalCache, TableName, SqlStatement, filter, () =>
            {
                return DbHelper.ExecuteEntity<TEntity>(SqlStatement);
            }, out bool fromCache);

            IsFromCache = fromCache;

            return result;
        }
        public int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlStatement = SqlGenerator.QueryCount(DataBaseType, filter, out string tableName);
            TableName = tableName;

            var result = Convert.ToInt32(MCache.Instance.GetFromCacheIfNotExistReStore_Count(LocalCache, TableName, SqlStatement, filter, () =>
            {
                return DbHelper.ExecuteScalar(SqlStatement);
            }, out bool fromCache));

            IsFromCache = fromCache;

            return result;
        }
        public bool QueryExist<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return QueryCount(filter) > 0;
        }

        public void ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            MCache.Instance.MarkTableModify(TableName);
            DbHelper.ExecuteNonQuery(sqlStatement, System.Data.CommandType.Text, parms);
        }
        public void ExecuteSqlAsync(string sqlStatement, IDictionary<string, object> parms = null)
        {
            MCache.Instance.MarkTableModify(TableName);
            DbHelper.ExecuteNonQueryAsync(sqlStatement, System.Data.CommandType.Text, parms);
        }
        public object ExecuteQuerySql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            return DbHelper.ExecuteDataSet(sqlStatement, System.Data.CommandType.Text, parms);
        }
        public object ExecuteQueryOneDataSql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            return DbHelper.ExecuteScalar(sqlStatement, System.Data.CommandType.Text, parms);
        }
        public TEntity ExecuteQueryOneSql<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class
        {
            return DbHelper.ExecuteEntity<TEntity>(sqlStatement, System.Data.CommandType.Text, parms);
        }
        public List<TEntity> ExecuteQueryListSql<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class
        {
            return DbHelper.ExecuteList<TEntity>(sqlStatement, System.Data.CommandType.Text, parms);
        }

        public List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, Expression<Func<TEntity, bool>> filter, bool isDESC = false) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            SqlStatement = SqlGenerator.QueryPaging(DataBaseType, pageIndex, pageSize, filter, orderBy, isDESC, out string tableName);
            TableName = tableName;

            var result = MCache.Instance.GetFromCacheIfNotExistReStoreEntitiesPaging(LocalCache, TableName, SqlStatement, filter, pageIndex, pageSize, orderBy, isDESC, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out int count, out bool fromCache);

            IsFromCache = fromCache;

            return result;
        }
        public List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, Expression<Func<TEntity, bool>> filter, out int count, bool isDESC = false) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            SqlStatement = SqlGenerator.QueryPaging(DataBaseType, pageIndex, pageSize, filter, orderBy, isDESC, out string tableName);
            TableName = tableName;

            var result = MCache.Instance.GetFromCacheIfNotExistReStoreEntitiesPaging(LocalCache, TableName, SqlStatement, filter, pageIndex, pageSize, orderBy, isDESC, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out int cou, out bool fromCache);

            IsFromCache = fromCache;

            count = cou;

            return result;
        }

        public void Dispose()
        {
        }
    }
}
