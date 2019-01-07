using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DataAccessEngine;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SevenTiny.Bantina.Bankinate.SqlStatementManager
{
    internal class SqlGenerator
    {
        //Cache properties by type
        private static Dictionary<Type, PropertyInfo[]> _propertiesDic = new Dictionary<Type, PropertyInfo[]>();
        private static PropertyInfo[] GetPropertiesDicByType(Type type)
        {
            if (!_propertiesDic.ContainsKey(type))
            {
                _propertiesDic.Add(type, type.GetProperties());
            }
            return _propertiesDic[type];
        }

        public static string Add<TEntity>(DbContext dbContext, TEntity entity, out Dictionary<string, object> paramsDic) where TEntity : class
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            paramsDic = new Dictionary<string, object>();

            StringBuilder builder_front = new StringBuilder(), builder_behind = new StringBuilder();
            builder_front.Append("INSERT INTO ");
            builder_front.Append(dbContext.TableName);
            builder_front.Append(" (");
            builder_behind.Append(" VALUES (");

            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //if not column mark exist,jump to next
                if (ColumnIgnoreAttribute.Exist(typeof(TEntity)))
                {
                }
                //AutoIncrease : if property is auto increase attribute skip this column.
                else if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                }
                //Column
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute column)
                {
                    builder_front.Append(column.GetName(propertyInfo.Name));
                    builder_front.Append(",");
                    builder_behind.Append("@");
                    //multitype database support
                    switch (dbContext.DataBaseType)
                    {
                        case DataBaseType.SqlServer:
                            columnName = column.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "");
                            break;
                        case DataBaseType.MySql:
                            columnName = column.GetName(propertyInfo.Name).Replace("`", "");
                            break;
                        default:
                            //default 兼容mysql和sqlserver的系统字段
                            columnName = column.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "").Replace("`", "");
                            break;
                    }
                    builder_behind.Append(columnName);
                    builder_behind.Append(",");
                    if (!paramsDic.ContainsKey(columnName))
                    {
                        paramsDic.Add(columnName, propertyInfo.GetValue(entity));
                    }
                }

                //in the end,remove the redundant symbol of ','
                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                    builder_front.Append(")");

                    builder_behind.Remove(builder_behind.Length - 1, 1);
                    builder_behind.Append(")");
                }
            }
            //Generate SqlStatement
            return dbContext.SqlStatement = builder_front.Append(builder_behind.ToString()).ToString();
        }

        public static string Update<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter, TEntity entity, out Dictionary<string, object> paramsDic) where TEntity : class
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            paramsDic = new Dictionary<string, object>();

            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
            //Mysql和sqlserver的分别处理
            if (dbContext.DataBaseType == DataBaseType.MySql)
            {
                builder_front.Append(dbContext.TableName);
                builder_front.Append(" ");
            }
            builder_front.Append(filter.Parameters[0].Name);
            builder_front.Append(" SET ");

            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                }
                //AutoIncrease : if property is auto increase attribute skip this column.
                else if (propertyInfo.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute keyAttr)
                {
                }
                //Column :
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    builder_front.Append(columnAttr.GetName(propertyInfo.Name));
                    builder_front.Append("=");
                    builder_front.Append("@");
                    //multitype database support
                    switch (dbContext.DataBaseType)
                    {
                        case DataBaseType.SqlServer:
                            columnName = columnAttr.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "");
                            break;
                        case DataBaseType.MySql:
                            columnName = columnAttr.GetName(propertyInfo.Name).Replace("`", "");
                            break;
                        default:
                            //default 兼容mysql和sqlserver的系统字段
                            columnName = columnAttr.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "").Replace("`", "");
                            break;
                    }
                    builder_front.Append(columnName);
                    builder_front.Append(",");
                    if (!paramsDic.ContainsKey(columnName))
                    {
                        paramsDic.Add(columnName, propertyInfo.GetValue(entity));
                    }
                }
                //in the end,remove the redundant symbol of ','
                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                }
            }

            //SqlServer和Mysql的sql语句分别处理
            if (dbContext.DataBaseType == DataBaseType.SqlServer)
            {
                builder_front.Append(" FROM ");
                builder_front.Append(dbContext.TableName);
                builder_front.Append(" ");
                builder_front.Append(filter.Parameters[0].Name);
            }

            //Generate SqlStatement
            return dbContext.SqlStatement = builder_front.Append($"{LambdaToSql.ConvertWhere(filter)}").ToString();
        }

        public static string Update<TEntity>(DbContext dbContext, TEntity entity, out Dictionary<string, object> paramsDic, out Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));

            //查找主键以及主键对应的值
            //get property which is key
            var keyProperty = propertyInfos.Where(t => t.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute)?.FirstOrDefault();
            if (keyProperty == null)
                throw new TableKeyNotFoundException($"table '{dbContext.TableName}' not found key column");

            string colunmName = keyProperty.Name;
            var value = keyProperty.GetValue(entity);

            if (keyProperty.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr1)
                colunmName = columnAttr1.GetName(keyProperty.Name);

            //Generate Expression of update via key
            ParameterExpression param = Expression.Parameter(typeof(TEntity), "t");
            MemberExpression left = Expression.Property(param, keyProperty);
            ConstantExpression right = Expression.Constant(value);
            BinaryExpression where = Expression.Equal(left, right);
            filter = Expression.Lambda<Func<TEntity, bool>>(where, param);

            paramsDic = new Dictionary<string, object>();
            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
            //Mysql和sqlserver的分别处理
            if (dbContext.DataBaseType == DataBaseType.MySql)
            {
                builder_front.Append(dbContext.TableName);
                builder_front.Append(" ");
            }
            builder_front.Append(filter.Parameters[0].Name);
            builder_front.Append(" SET ");

            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                }
                //AutoIncrease : if property is auto increase attribute skip this column.
                else if (propertyInfo.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute keyAttr)
                {
                }
                //Column :
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    builder_front.Append(columnAttr.GetName(propertyInfo.Name));
                    builder_front.Append("=");
                    builder_front.Append("@");
                    //multitype database support
                    switch (dbContext.DataBaseType)
                    {
                        case DataBaseType.SqlServer:
                            columnName = columnAttr.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "");
                            break;
                        case DataBaseType.MySql:
                            columnName = columnAttr.GetName(propertyInfo.Name).Replace("`", "");
                            break;
                        default:
                            //default 兼容mysql和sqlserver的系统字段
                            columnName = columnAttr.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "").Replace("`", "");
                            break;
                    }
                    builder_front.Append(columnName);
                    builder_front.Append(",");
                    if (!paramsDic.ContainsKey(columnName))
                    {
                        paramsDic.Add(columnName, propertyInfo.GetValue(entity));
                    }
                }
                //in the end,remove the redundant symbol of ','
                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                }
            }

            //SqlServer和Mysql的sql语句分别处理
            if (dbContext.DataBaseType == DataBaseType.SqlServer)
            {
                builder_front.Append(" FROM ");
                builder_front.Append(dbContext.TableName);
                builder_front.Append(" ");
                builder_front.Append(filter.Parameters[0].Name);
            }

            //Generate SqlStatement
            return dbContext.SqlStatement = builder_front.Append($"{LambdaToSql.ConvertWhere(filter)}").ToString();
        }

        public static string Delete<TEntity>(DbContext dbContext, TEntity entity) where TEntity : class
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            //get property which is key
            var property = propertyInfos.Where(t => t.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute)?.FirstOrDefault();

            if (property == null)
                throw new TableKeyNotFoundException($"table '{dbContext.TableName}' not found key column");

            string colunmName = property.Name;
            var value = property.GetValue(entity);

            if (property.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                colunmName = columnAttr.GetName(property.Name);

            return dbContext.SqlStatement = $"DELETE t FROM {dbContext.TableName} t WHERE {colunmName} = {value}";
        }

        public static string Delete<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                case DataBaseType.MySql:
                    dbContext.SqlStatement = $"DELETE {filter.Parameters[0].Name} From {dbContext.TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";
                    break;
                case DataBaseType.Oracle:
                    dbContext.SqlStatement = string.Empty;
                    break;
                default:
                    dbContext.SqlStatement = string.Empty;
                    break;
            }
            return dbContext.SqlStatement;
        }

        public static string QueryOne<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                    dbContext.SqlStatement = $"SELECT TOP 1 * FROM {dbContext.TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}"; break;
                case DataBaseType.MySql:
                    dbContext.SqlStatement = $"SELECT * FROM {dbContext.TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)} LIMIT 1"; break;
                case DataBaseType.Oracle:
                    dbContext.SqlStatement = string.Empty; break;
                default:
                    dbContext.SqlStatement = string.Empty; break;
            }
            return dbContext.SqlStatement;
        }

        public static string QueryCount<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                case DataBaseType.MySql:
                    dbContext.SqlStatement = $"SELECT COUNT(0) FROM {dbContext.TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}"; break;
                case DataBaseType.Oracle:
                    dbContext.SqlStatement = string.Empty; break;
                default:
                    dbContext.SqlStatement = string.Empty; break;
            }
            return dbContext.SqlStatement;
        }

        public static string QueryList<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                case DataBaseType.MySql:
                    dbContext.SqlStatement = $"SELECT * FROM {dbContext.TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}"; break;
                case DataBaseType.Oracle:
                    dbContext.SqlStatement = string.Empty; break;
                default:
                    dbContext.SqlStatement = string.Empty; break;
            }
            return dbContext.SqlStatement;
        }

        public static string QueryOrderBy<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> orderBy, bool isDESC) where TEntity : class
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                case DataBaseType.MySql:
                    string desc = isDESC ? "DESC" : "ASC";
                    dbContext.SqlStatement = $"SELECT * FROM {dbContext.TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)} ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}"; break;
                case DataBaseType.Oracle:
                    dbContext.SqlStatement = string.Empty; break;
                default:
                    dbContext.SqlStatement = string.Empty; break;
            }
            return dbContext.SqlStatement;
        }

        public static string QueryPaging<TEntity>(DbContext dbContext, int pageIndex, int pageSize, Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> orderBy, bool isDESC) where TEntity : class
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            string desc = isDESC ? "DESC" : "ASC";
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                    dbContext.SqlStatement = $"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}) AS RowNumber,* FROM {dbContext.TableName} {orderBy.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}) AS TTTTTT  WHERE RowNumber > {pageSize * (pageIndex - 1)}"; break;
                case DataBaseType.MySql:
                    dbContext.SqlStatement = $"SELECT * FROM {dbContext.TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)} ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc} LIMIT {pageIndex * pageSize},{pageSize}"; break;
                case DataBaseType.Oracle:
                    dbContext.SqlStatement = string.Empty; break;
                default:
                    dbContext.SqlStatement = string.Empty; break;
            }
            return dbContext.SqlStatement;
        }
    }
}
