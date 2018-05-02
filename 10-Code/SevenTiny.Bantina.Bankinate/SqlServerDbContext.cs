/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-19 23:57:48
 * Modify: 2018-04-19 23:57:48
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class SqlServerDbContext<TDataBase> : IDbContext, IExecuteSqlOperate, IQueryPagingOperate where TDataBase : class
    {
        public SqlServerDbContext(string connectionString)
        {
            DbHelper.ConnString_Default = connectionString;
            DbHelper.DbType = DataBaseType.SqlServer;
            MCache.ExpiredTimeSpan = CacheExpiredTimeSpan;
        }

        public SqlServerDbContext(string connectionString_Read, string connectionString_ReadWrite)
        {
            DbHelper.ConnString_R = connectionString_Read;
            DbHelper.ConnString_RW = connectionString_ReadWrite;
            DbHelper.DbType = DataBaseType.SqlServer;
            MCache.ExpiredTimeSpan = CacheExpiredTimeSpan;
        }

        public string SqlStatement { get; set; }
        public string TableName { get; set; }
        public bool LocalCache { get; set; } = false;
        public TimeSpan CacheExpiredTimeSpan { get; set; } = TimeSpan.FromDays(1);
        public bool IsFromCache { get; set; } = false;

        //Cache properties by type
        private Dictionary<Type, PropertyInfo[]> propertiesDic = new Dictionary<Type, PropertyInfo[]>();
        private PropertyInfo[] GetPropertiesDicByType(Type type)
        {
            if (!propertiesDic.ContainsKey(type))
            {
                propertiesDic.Add(type, type.GetProperties());
            }
            return propertiesDic[type];
        }

        private Dictionary<string, object> GenerateAddSqlAndGetParams<TEntity>(TEntity entity) where TEntity : class
        {
            Dictionary<string, object> paramsDic = new Dictionary<string, object>();

            StringBuilder builder_front = new StringBuilder(), builder_behind = new StringBuilder();
            builder_front.Append("INSERT INTO ");
            builder_front.Append(TableName);
            builder_front.Append(" (");
            builder_behind.Append(" VALUES (");

            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //if not column mark exist,jump to next
                if (NotColumnAttribute.Exist(typeof(TEntity)))
                {
                    continue;
                }
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                    continue;
                }
                //key :
                if (propertyInfo.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute keyAttr)
                {
                    builder_front.Append(keyAttr.GetName(propertyInfo.Name));
                    builder_front.Append(",");
                    builder_behind.Append("@");
                    columnName = keyAttr.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "");
                    builder_behind.Append(columnName);
                    builder_behind.Append(",");

                    if (!paramsDic.ContainsKey(columnName))
                    {
                        paramsDic.Add(columnName, propertyInfo.GetValue(entity));
                    }
                }
                //Column :
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute column)
                {
                    builder_front.Append(column.GetName(propertyInfo.Name));
                    builder_front.Append(",");
                    builder_behind.Append("@");
                    columnName = column.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "");
                    builder_behind.Append(columnName);
                    builder_behind.Append(",");
                    if (!paramsDic.ContainsKey(columnName))
                    {
                        paramsDic.Add(columnName, propertyInfo.GetValue(entity));
                    }
                }
                //the end
                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                    builder_front.Append(")");

                    builder_behind.Remove(builder_behind.Length - 1, 1);
                    builder_behind.Append(")");
                }
            }
            //Generate SqlStatement
            this.SqlStatement = builder_front.Append(builder_behind.ToString()).ToString();

            return paramsDic;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            Dictionary<string, object> paramsDic = GenerateAddSqlAndGetParams(entity);

            MCache.MarkTableModifyAdd(TableName, entity);

            DbHelper.ExecuteNonQuery(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }

        public void AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            Dictionary<string, object> paramsDic = GenerateAddSqlAndGetParams(entity);

            MCache.MarkTableModifyAdd(TableName, entity);

            DbHelper.ExecuteNonQueryAsync(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }

        public void Add<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void AddAsync<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"DELETE {filter.Parameters[0].Name} From {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";

            MCache.MarkTableModifyDelete(TableName, filter);

            DbHelper.ExecuteNonQuery(SqlStatement);
        }

        public void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"DELETE {filter.Parameters[0].Name} From {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";

            MCache.MarkTableModifyDelete(TableName, filter);

            DbHelper.ExecuteNonQueryAsync(SqlStatement);
        }

        private Dictionary<string, object> GenerateUpdateSqlAndGetParams<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            Dictionary<string, object> paramsDic = new Dictionary<string, object>();

            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
            builder_front.Append(TableName);
            builder_front.Append(" ");
            builder_front.Append(filter.Parameters[0].Name);
            builder_front.Append(" SET ");

            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                    continue;
                }
                //key :
                if (propertyInfo.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute keyAttr)
                {
                    builder_front.Append(keyAttr.GetName(propertyInfo.Name));
                    builder_front.Append("=");
                    builder_front.Append("@");
                    columnName = keyAttr.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "");
                    builder_front.Append(columnName);
                    builder_front.Append(",");
                    if (!paramsDic.ContainsKey(columnName))
                    {
                        paramsDic.Add(columnName, propertyInfo.GetValue(entity));
                    }
                }
                //Column :
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    builder_front.Append(columnAttr.GetName(propertyInfo.Name));
                    builder_front.Append("=");
                    builder_front.Append("@");
                    columnName = columnAttr.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "");
                    builder_front.Append(columnName);
                    builder_front.Append(",");
                    if (!paramsDic.ContainsKey(columnName))
                    {
                        paramsDic.Add(columnName, propertyInfo.GetValue(entity));
                    }
                }
                //the end
                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                }
            }
            //Generate SqlStatement
            this.SqlStatement = builder_front.Append($"{LambdaToSql.ConvertWhere(filter)}").ToString();
            return paramsDic;
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            Dictionary<string, object> paramsDic = GenerateUpdateSqlAndGetParams(filter, entity);

            MCache.MarkTableModifyUpdate(TableName, filter, entity);

            DbHelper.ExecuteNonQuery(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }

        public void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            Dictionary<string, object> paramsDic = GenerateUpdateSqlAndGetParams(filter, entity);

            MCache.MarkTableModifyUpdate(TableName, filter, entity);

            DbHelper.ExecuteNonQueryAsync(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }

        public List<TEntity> QueryList<TEntity>() where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"SELECT * FROM {TableName}";

            var result = MCache.GetFromCacheIfNotExistReStoreEntities(LocalCache, TableName, SqlStatement, null, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out bool fromCache);

            IsFromCache = fromCache;

            return result;
        }

        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, object>> orderBy, bool isDESC = false) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            string desc = isDESC ? "DESC" : "ASC";
            this.SqlStatement = $"SELECT * FROM {TableName} {orderBy.Parameters[0].Name} ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}";

            var result = MCache.GetFromCacheIfNotExistReStoreEntities(LocalCache, TableName, SqlStatement, null, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out bool fromCache);

            IsFromCache = fromCache;

            return result;
        }

        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"SELECT * FROM {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";

            var result = MCache.GetFromCacheIfNotExistReStoreEntities(LocalCache, TableName, SqlStatement, filter, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out bool fromCache);

            IsFromCache = fromCache;

            return result;
        }

        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> orderBy, bool isDESC = false) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            string desc = isDESC ? "DESC" : "ASC";
            this.SqlStatement = $"SELECT * FROM {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)} ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}";

            var result = MCache.GetFromCacheIfNotExistReStoreEntities(LocalCache, TableName, SqlStatement, filter, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out bool fromCache);

            IsFromCache = fromCache;

            return result;
        }


        public TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"SELECT TOP 1 * FROM {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";

            var result = MCache.GetFromCacheIfNotExistReStoreEntity(LocalCache, TableName, SqlStatement, filter, () =>
            {
                return DbHelper.ExecuteEntity<TEntity>(SqlStatement);
            }, out bool fromCache);

            IsFromCache = fromCache;

            return result;
        }

        public int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"SELECT COUNT(0) FROM {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";

            var result = Convert.ToInt32(MCache.GetFromCacheIfNotExistReStoreCount(LocalCache, TableName, SqlStatement, filter, () =>
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
            MCache.MarkTableModify(TableName);
            DbHelper.ExecuteNonQuery(sqlStatement, System.Data.CommandType.Text, parms);
        }

        public void ExecuteSqlAsync(string sqlStatement, IDictionary<string, object> parms = null)
        {
            MCache.MarkTableModify(TableName);
            DbHelper.ExecuteNonQueryAsync(sqlStatement, System.Data.CommandType.Text, parms);
        }

        public object ExecuteQuerySql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            return DbHelper.ExecuteDataSet(sqlStatement, System.Data.CommandType.Text, parms);
        }

        public object ExecuteQueryObjectSql(string sqlStatement, IDictionary<string, object> parms = null)
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

        public List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, bool isDESC = false) where TEntity : class
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

            string desc = isDESC ? "DESC" : "ASC";
            this.SqlStatement = $"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}) AS RowNumber,* FROM {TableName} {orderBy.Parameters[0].Name} WHERE 1=1) AS TTTTTT  WHERE RowNumber > {pageSize * (pageIndex - 1)}";

            var result = MCache.GetFromCacheIfNotExistReStoreEntitiesPaging(LocalCache, TableName, SqlStatement, null, pageIndex, pageSize, orderBy, isDESC, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out int count, out bool fromCache);

            IsFromCache = fromCache;

            return result;
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

            string desc = isDESC ? "DESC" : "ASC";
            this.SqlStatement = $"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}) AS RowNumber,* FROM {TableName} {orderBy.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}) AS TTTTTT  WHERE RowNumber > {pageSize * (pageIndex - 1)}";

            var result = MCache.GetFromCacheIfNotExistReStoreEntitiesPaging(LocalCache, TableName, SqlStatement, filter, pageIndex, pageSize, orderBy, isDESC, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out int count, out bool fromCache);

            IsFromCache = fromCache;

            return result;
        }

        public List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, out int count, bool isDESC = false) where TEntity : class
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

            string desc = isDESC ? "DESC" : "ASC";
            this.SqlStatement = $"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}) AS RowNumber,* FROM {TableName} {orderBy.Parameters[0].Name} WHERE 1=1) AS TTTTTT  WHERE RowNumber > {pageSize * (pageIndex - 1)}";

            var result = MCache.GetFromCacheIfNotExistReStoreEntitiesPaging(LocalCache, TableName, SqlStatement, null, pageIndex, pageSize, orderBy, isDESC, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            }, out int cou, out bool fromCache);

            IsFromCache = fromCache;

            count = cou;

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

            string desc = isDESC ? "DESC" : "ASC";
            this.SqlStatement = $"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}) AS RowNumber,* FROM {TableName} {orderBy.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}) AS TTTTTT  WHERE RowNumber > {pageSize * (pageIndex - 1)}";

            var result = MCache.GetFromCacheIfNotExistReStoreEntitiesPaging(LocalCache, TableName, SqlStatement, filter, pageIndex, pageSize, orderBy, isDESC, () =>
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