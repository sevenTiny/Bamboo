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
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MySqlDbContext<TDataBase> : IDbContext, IExecuteSqlOperate, IQueryPagingOperate where TDataBase : class
    {
        public MySqlDbContext(string connectionString)
        {
            DbHelper.ConnString_Default = connectionString;
            DbHelper.DbType = DataBaseType.MySql;
        }

        public MySqlDbContext(string connectionString_Read, string connectionString_ReadWrite)
        {
            DbHelper.ConnString_R = connectionString_Read;
            DbHelper.ConnString_RW = connectionString_ReadWrite;
            DbHelper.DbType = DataBaseType.MySql;
        }

        public string SqlStatement { get; set; }
        public string TableName { get; set; }
        public bool LocalCache { get; set; } = true;

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
                    builder_behind.Append(keyAttr.GetName(propertyInfo.Name));
                    builder_behind.Append(",");

                    if (!paramsDic.ContainsKey(keyAttr.GetName(propertyInfo.Name)))
                    {
                        paramsDic.Add(keyAttr.GetName(propertyInfo.Name), propertyInfo.GetValue(entity));
                    }
                }
                //Column :
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute column)
                {
                    builder_front.Append(column.GetName(propertyInfo.Name));
                    builder_front.Append(",");

                    builder_behind.Append("@");
                    builder_behind.Append(column.GetName(propertyInfo.Name));
                    builder_behind.Append(",");
                    if (!paramsDic.ContainsKey(column.GetName(propertyInfo.Name)))
                    {
                        paramsDic.Add(column.GetName(propertyInfo.Name), propertyInfo.GetValue(entity));
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

            MCache.MarkTableModify(TableName);

            DbHelper.ExecuteNonQuery(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }

        public void AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            Dictionary<string, object> paramsDic = GenerateAddSqlAndGetParams(entity);

            MCache.MarkTableModify(TableName);

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

            MCache.MarkTableModify(TableName);

            DbHelper.ExecuteNonQuery(SqlStatement);
        }

        public void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"DELETE {filter.Parameters[0].Name} From {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";

            MCache.MarkTableModify(TableName);

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
                    builder_front.Append(keyAttr.GetName(propertyInfo.Name));
                    builder_front.Append(",");

                    if (!paramsDic.ContainsKey(keyAttr.GetName(propertyInfo.Name)))
                    {
                        paramsDic.Add(keyAttr.GetName(propertyInfo.Name), propertyInfo.GetValue(entity));
                    }
                }
                //Column :
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    builder_front.Append(columnAttr.GetName(propertyInfo.Name));
                    builder_front.Append("=");
                    builder_front.Append("@");
                    builder_front.Append(columnAttr.GetName(propertyInfo.Name));
                    builder_front.Append(",");

                    if (!paramsDic.ContainsKey(columnAttr.GetName(propertyInfo.Name)))
                    {
                        paramsDic.Add(columnAttr.GetName(propertyInfo.Name), propertyInfo.GetValue(entity));
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

            MCache.MarkTableModify(TableName);

            DbHelper.ExecuteNonQuery(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }

        public void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            Dictionary<string, object> paramsDic = GenerateUpdateSqlAndGetParams(filter, entity);

            MCache.MarkTableModify(TableName);

            DbHelper.ExecuteNonQueryAsync(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }

        public List<TEntity> QueryList<TEntity>() where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"SELECT * FROM {TableName}";

            return MCache.GetInCacheIfNotExistReStoreEntities(LocalCache, TableName, SqlStatement, null, () =>
              {
                  return DbHelper.ExecuteList<TEntity>(SqlStatement);
              });
        }

        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"SELECT * FROM {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";

            return MCache.GetInCacheIfNotExistReStoreEntities(LocalCache, TableName, SqlStatement, filter, () =>
            {
                return DbHelper.ExecuteList<TEntity>(SqlStatement);
            });
        }

        public TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"SELECT * FROM {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)} LIMIT 1";

            return DbHelper.ExecuteEntity<TEntity>(SqlStatement);
        }

        public int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            this.SqlStatement = $"SELECT COUNT(0) FROM {TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";

            return Convert.ToInt32(DbHelper.ExecuteScalar(SqlStatement));
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

        public List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, bool isDESC = false) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            string desc = isDESC ? "DESC" : "ASC";
            this.SqlStatement = $"SELECT * FROM {TableName} ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc} LIMIT {pageIndex * pageSize},{pageSize}";

            return DbHelper.ExecuteList<TEntity>(SqlStatement);
        }

        public List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, Expression<Func<TEntity, bool>> filter, bool isDESC = false) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            string desc = isDESC ? "DESC" : "ASC";
            this.SqlStatement = $"SELECT * FROM {TableName} {LambdaToSql.ConvertWhere(filter)} ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc} LIMIT {pageIndex * pageSize},{pageSize}";

            return DbHelper.ExecuteList<TEntity>(SqlStatement);
        }

        public List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, out int count, bool isDESC = false) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            string desc = isDESC ? "DESC" : "ASC";
            this.SqlStatement = $"SELECT * FROM {TableName} ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc} LIMIT {pageIndex * pageSize},{pageSize}";

            List<TEntity> result = DbHelper.ExecuteList<TEntity>(SqlStatement);

            count = result.Count;

            return result;
        }

        public List<TEntity> QueryListPaging<TEntity>(int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, Expression<Func<TEntity, bool>> filter, out int count, bool isDESC = false) where TEntity : class
        {
            TableName = TableAttribute.GetName(typeof(TEntity));

            string desc = isDESC ? "DESC" : "ASC";
            this.SqlStatement = $"SELECT * FROM {TableName} {LambdaToSql.ConvertWhere(filter)} ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc} LIMIT {pageIndex * pageSize},{pageSize}";

            List<TEntity> result = DbHelper.ExecuteList<TEntity>(SqlStatement);

            count = result.Count;

            return result;
        }

        public void Dispose()
        {

        }
    }
}