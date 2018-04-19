using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MySqlDbContext<TDataBase> : IDbContext where TDataBase : class
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

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            Type entityType = typeof(TEntity);

            string tableName = TableAttribute.GetName(entityType);

            Dictionary<string, object> paramsDic = new Dictionary<string, object>();

            StringBuilder builder_front = new StringBuilder(), builder_behind = new StringBuilder();
            builder_front.Append("INSERT INTO ");
            builder_front.Append(tableName);
            builder_front.Append(" (");
            builder_behind.Append(" VALUES (");

            PropertyInfo[] propertyInfos = GetPropertiesDicByType(entityType);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //if not column mark exist,jump to next
                if (NotColumnAttribute.Exist(entityType))
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

            int result = DbHelper.ExecuteNonQuery(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }

        public void Add<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void AddAsync<TEntity>(IList<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            string tableName = TableAttribute.GetName(typeof(TEntity));

            StringBuilder builder_front = new StringBuilder();

            builder_front.Append("DELETE ");
            builder_front.Append(filter.Parameters[0].Name);
            builder_front.Append(" From ");
            builder_front.Append(tableName);
            builder_front.Append(" ");
            builder_front.Append(LambdaToSql.ConvertWhere(filter));

            //Generate SqlStatement
            this.SqlStatement = builder_front.ToString();

            //Execute Task That Execute SqlStatement
            DbHelper.ExecuteNonQuery(SqlStatement);
        }

        public void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            string tableName = TableAttribute.GetName(typeof(TEntity));

            Dictionary<string, object> paramsDic = new Dictionary<string, object>();

            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
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
                    builder_front.Append(" From ");
                    builder_front.Append(tableName);
                    builder_front.Append(" ");
                }
            }
            //Generate SqlStatement
            this.SqlStatement = builder_front.Append(LambdaToSql.ConvertWhere(filter)).ToString();
            //Execute Task That Execute SqlStatement
            DbHelper.ExecuteNonQuery(SqlStatement, System.Data.CommandType.Text, paramsDic);
        }

        public void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public List<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            throw new NotImplementedException();
        }
        public TEntity QueryOne<TEntity>(string id) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            DbHelper.ExecuteNonQuery(sqlStatement, System.Data.CommandType.Text, parms);
        }

        public object ExecuteQuerySql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            return DbHelper.ExecuteDataSet(sqlStatement, System.Data.CommandType.Text, parms);
        }

        public int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public bool QueryExist<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return QueryCount(filter) > 0;
        }
    }
}
