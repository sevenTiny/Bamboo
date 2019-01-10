using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Cache;
using SevenTiny.Bantina.Bankinate.DataAccessEngine;
using SevenTiny.Bantina.Bankinate.SqlStatementManager;
using SevenTiny.Bantina.Bankinate.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class SqlDbContext<TDataBase> : DbContext, IDbContext, ISqlQueryOperate, IExecuteSqlOperate where TDataBase : class
    {
        public SqlDbContext(DataBaseType dataBaseType, string connectionString) : this(dataBaseType, connectionString, connectionString) { }

        public SqlDbContext(DataBaseType dataBaseType, string connectionString_ReadWrite, string connectionString_Read) : base(dataBaseType, connectionString_ReadWrite, connectionString_Read)
        {
            DataBaseName = DataBaseAttribute.GetName(typeof(TDataBase));
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Add(this, entity);
            DbHelper.ExecuteNonQuery(this);
            DbCacheManager.Add(this, entity);
        }
        public void AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Add(this, entity);
            DbHelper.ExecuteNonQueryAsync(this);
            DbCacheManager.Add(this, entity);
        }
        public void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            List<BatchExecuteModel> batchExecuteModels = new List<BatchExecuteModel>();
            foreach (var item in entities)
            {
                PropertyDataValidator.Verify(this, item);
                batchExecuteModels.Add(new BatchExecuteModel
                {
                    CommandTextOrSpName = SqlGenerator.Add(this, item),
                    ParamsDic = this.Parameters
                });
            }
            DbHelper.BatchExecuteNonQuery(this, batchExecuteModels);
            DbCacheManager.Add(this, entities);
        }
        public void AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            List<BatchExecuteModel> batchExecuteModels = new List<BatchExecuteModel>();
            foreach (var item in entities)
            {
                PropertyDataValidator.Verify(this, item);
                batchExecuteModels.Add(new BatchExecuteModel
                {
                    CommandTextOrSpName = SqlGenerator.Add(this, item),
                    ParamsDic = this.Parameters
                });
            }
            DbHelper.BatchExecuteNonQueryAsync(this,batchExecuteModels);
            DbCacheManager.Add(this, entities);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            SqlGenerator.Delete(this, entity);
            DbHelper.ExecuteNonQuery(this);
            DbCacheManager.Delete(this, entity);
        }
        public void DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            SqlGenerator.Delete(this, entity);
            DbHelper.ExecuteNonQueryAsync(this);
            DbCacheManager.Delete(this, entity);
        }
        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlGenerator.Delete(this, filter);
            DbHelper.ExecuteNonQuery(this);
            DbCacheManager.Delete(this, filter);
        }
        public void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlGenerator.Delete(this, filter);
            DbHelper.ExecuteNonQueryAsync(this);
            DbCacheManager.Delete(this, filter);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Update(this, entity, out Expression<Func<TEntity, bool>> filter);
            DbHelper.ExecuteNonQuery(this);
            DbCacheManager.Update(this, entity, filter);
        }
        public void UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Update(this, entity, out Expression<Func<TEntity, bool>> filter);
            DbHelper.ExecuteNonQueryAsync(this);
            DbCacheManager.Update(this, entity, filter);
        }
        public void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Update(this, filter, entity );
            DbHelper.ExecuteNonQuery(this);
            DbCacheManager.Update(this, entity, filter);
        }
        public void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Update(this, filter, entity);
            DbHelper.ExecuteNonQueryAsync(this);
            DbCacheManager.Update(this, entity, filter);
        }

        /// <summary>
        /// 支持复杂高效查询的查询入口
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public SqlQueryable<TEntity> Queryable<TEntity>() where TEntity : class
        {
            return new SqlQueryable<TEntity>(this);
        }

        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return Queryable<TEntity>().Where(filter).ToList();
        }
        public TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return Queryable<TEntity>().Where(filter).ToEntity();
        }
        public int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return Queryable<TEntity>().Where(filter).ToCount();
        }
        public bool QueryExist<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return QueryCount(filter) > 0;
        }

        public void ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            SqlStatement = sqlStatement;
            Parameters = parms;
            DbHelper.ExecuteNonQuery(this);
        }
        public void ExecuteSqlAsync(string sqlStatement, IDictionary<string, object> parms = null)
        {
            SqlStatement = sqlStatement;
            Parameters = parms;
            DbHelper.ExecuteNonQueryAsync(this);
        }
        public DataSet ExecuteQueryDataSetSql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            SqlStatement = sqlStatement;
            Parameters = parms;
            return DbHelper.ExecuteDataSet(this);
        }
        public object ExecuteQueryOneDataSql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            SqlStatement = sqlStatement;
            Parameters = parms;
            return DbHelper.ExecuteScalar(this);
        }
        public TEntity ExecuteQueryOneSql<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class
        {
            SqlStatement = sqlStatement;
            Parameters = parms;
            return DbHelper.ExecuteEntity<TEntity>(this);
        }
        public List<TEntity> ExecuteQueryListSql<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class
        {
            SqlStatement = sqlStatement;
            Parameters = parms;
            return DbHelper.ExecuteList<TEntity>(this);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
