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
        public SqlDbContext(string connectionString, DataBaseType dataBaseType) : this(connectionString, connectionString, dataBaseType) { }

        public SqlDbContext(string connectionString_Read, string connectionString_ReadWrite, DataBaseType dataBaseType) : base(dataBaseType)
        {
            DbHelper.ConnString_R = connectionString_Read;
            DbHelper.ConnString_RW = connectionString_ReadWrite;
            DbHelper.DbType = dataBaseType;
            DataBaseName = DataBaseAttribute.GetName(typeof(TDataBase));
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            SqlGenerator.Add(this, entity, out Dictionary<string, object> paramsDic);
            DbHelper.ExecuteNonQuery(SqlStatement, System.Data.CommandType.Text, paramsDic);
            DbCacheManager.Add(this, entity);
        }
        public void AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            SqlGenerator.Add(this, entity, out Dictionary<string, object> paramsDic);
            DbHelper.ExecuteNonQueryAsync(SqlStatement, System.Data.CommandType.Text, paramsDic);
            DbCacheManager.Add(this, entity);
        }
        public void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            List<BatchExecuteModel> batchExecuteModels = new List<BatchExecuteModel>();
            foreach (var item in entities)
            {
                SqlGenerator.Add(this, item, out Dictionary<string, object> paramsDic);

                batchExecuteModels.Add(new BatchExecuteModel
                {
                    CommandTextOrSpName = SqlStatement,
                    ParamsDic = paramsDic
                });
            }
            DbHelper.BatchExecuteNonQuery(batchExecuteModels);
            DbCacheManager.Add(this, entities);
        }
        public void AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            List<BatchExecuteModel> batchExecuteModels = new List<BatchExecuteModel>();
            foreach (var item in entities)
            {
                SqlGenerator.Add(this, item, out Dictionary<string, object> paramsDic);

                batchExecuteModels.Add(new BatchExecuteModel
                {
                    CommandTextOrSpName = SqlStatement,
                    ParamsDic = paramsDic
                });
            }
            DbHelper.BatchExecuteNonQueryAsync(batchExecuteModels);
            DbCacheManager.Add(this, entities);
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlGenerator.Delete(this, filter);
            DbHelper.ExecuteNonQuery(SqlStatement);
            DbCacheManager.Delete(this, filter);
        }
        public void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlGenerator.Delete(this, filter);
            DbHelper.ExecuteNonQueryAsync(SqlStatement);
            DbCacheManager.Delete(this, filter);
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            SqlGenerator.Update(this, filter, entity, out Dictionary<string, object> paramsDic);
            DbHelper.ExecuteNonQuery(SqlStatement, System.Data.CommandType.Text, paramsDic);
            DbCacheManager.Update(this, entity, filter);
        }
        public void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            SqlGenerator.Update(this, filter, entity, out Dictionary<string, object> paramsDic);
            DbHelper.ExecuteNonQueryAsync(SqlStatement, System.Data.CommandType.Text, paramsDic);
            DbCacheManager.Update(this, entity, filter);
        }

        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlGenerator.QueryList(this, filter);
            return DbCacheManager.GetEntities(this, filter, () =>
              {
                  return DbHelper.ExecuteList<TEntity>(SqlStatement);
              });
        }
        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> orderBy, bool isDESC = false) where TEntity : class
        {
            SqlGenerator.QueryOrderBy(this, filter, orderBy, isDESC);
            return DbCacheManager.GetEntities(this, filter, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            });
        }
        public List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, Expression<Func<TEntity, bool>> filter, bool isDESC = false) where TEntity : class
        {
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            SqlGenerator.QueryPaging(this, pageIndex, pageSize, filter, orderBy, isDESC);
            return DbCacheManager.GetEntities(this, filter, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            });
        }
        public List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, Expression<Func<TEntity, bool>> filter, out int count, bool isDESC = false) where TEntity : class
        {
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            SqlGenerator.QueryPaging(this, pageIndex, pageSize, filter, orderBy, isDESC);
            var result = DbCacheManager.GetEntities(this, filter, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            });
            count = result?.Count ?? default(int);
            return result;
        }

        public TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlGenerator.QueryOne(this, filter);
            return DbCacheManager.GetEntity(this, filter, () =>
             {
                 return DbHelper.ExecuteEntity<TEntity>(SqlStatement);
             });
        }
        public int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlGenerator.QueryCount(this, filter);
            return DbCacheManager.GetCount(this, filter, () =>
            {
                return int.TryParse(Convert.ToString(DbHelper.ExecuteScalar(SqlStatement)), out int result) ? result : default(int);
            });
        }
        public bool QueryExist<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return QueryCount(filter) > 0;
        }

        public void ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            DbHelper.ExecuteNonQuery(sqlStatement, System.Data.CommandType.Text, parms);
        }
        public void ExecuteSqlAsync(string sqlStatement, IDictionary<string, object> parms = null)
        {
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
