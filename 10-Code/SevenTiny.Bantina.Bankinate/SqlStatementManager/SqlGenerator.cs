using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DataAccessEngine;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Exceptions;
using SevenTiny.Bantina.Bankinate.Helpers;
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
            _propertiesDic.AddOrUpdate(type, type.GetProperties());
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

                    paramsDic.AddOrUpdate($"@{columnName}", propertyInfo.GetValue(entity));
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

        public static string Update<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter, TEntity entity, out IDictionary<string, object> paramsDic) where TEntity : class
        {
            paramsDic = new Dictionary<string, object>();
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));

            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
            //Mysql和sqlserver的分别处理
            if (dbContext.DataBaseType == DataBaseType.MySql)
            {
                builder_front.Append(dbContext.TableName);
                builder_front.Append(" ");
            }

            //查询语句中表的别名，例如“t”
            string alias = filter.Parameters[0].Name;
            builder_front.Append(alias);
            builder_front.Append(" SET ");

            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                }
                //Column :
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    builder_front.Append(columnAttr.GetName(propertyInfo.Name));
                    builder_front.Append("=");
                    builder_front.Append($"@{alias}");
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

                    paramsDic.AddOrUpdate($"@{alias}{columnName}", propertyInfo.GetValue(entity));
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
                builder_front.Append(alias);
            }

            //Generate SqlStatement
            return dbContext.SqlStatement = builder_front.Append($"{LambdaToSql.ConvertWhere(filter)}").ToString();
        }

        public static string Update<TEntity>(DbContext dbContext, TEntity entity, out IDictionary<string, object> paramsDic, out Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            paramsDic = new Dictionary<string, object>();
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));

            //查找主键以及主键对应的值，如果用该方法更新数据，主键是必须存在的
            //get property which is key
            var keyProperty = propertyInfos.Where(t => t.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute)?.FirstOrDefault();
            if (keyProperty == null)
                throw new TableKeyNotFoundException($"table '{dbContext.TableName}' not found key column");

            //主键的key
            string keyName = keyProperty.Name;
            //主键的value 
            var keyValue = keyProperty.GetValue(entity);

            if (keyProperty.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr1)
                keyName = columnAttr1.GetName(keyProperty.Name);

            //Generate Expression of update via key : t=>t.'Key'== value
            ParameterExpression param = Expression.Parameter(typeof(TEntity), "t");
            MemberExpression left = Expression.Property(param, keyProperty);
            ConstantExpression right = Expression.Constant(keyValue);
            BinaryExpression where = Expression.Equal(left, right);
            filter = Expression.Lambda<Func<TEntity, bool>>(where, param);

            //将主键的查询参数加到字典中
            paramsDic.AddOrUpdate($"@t{keyName}", keyValue);

            //开始构造赋值的sql语句
            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
            //Mysql和sqlserver的分别处理
            if (dbContext.DataBaseType == DataBaseType.MySql)
            {
                builder_front.Append(dbContext.TableName);
                builder_front.Append(" ");
            }

            //查询语句中表的别名，例如“t”
            string alias = filter.Parameters[0].Name;

            builder_front.Append(alias);
            builder_front.Append(" SET ");

            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                }
                //Column :
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    builder_front.Append(columnAttr.GetName(propertyInfo.Name));
                    builder_front.Append("=");
                    builder_front.Append($"@{alias}");
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

                    paramsDic.AddOrUpdate($"@{alias}{columnName}", propertyInfo.GetValue(entity));
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
                builder_front.Append(alias);
            }

            //Generate SqlStatement
            return dbContext.SqlStatement = builder_front.Append($"{LambdaToSql.ConvertWhere(filter)}").ToString();
        }

        public static string Delete<TEntity>(DbContext dbContext, TEntity entity, out IDictionary<string, object> parameters) where TEntity : class
        {
            parameters = new Dictionary<string, object>();
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

            parameters.AddOrUpdate($"@t{colunmName}", value);
            return dbContext.SqlStatement = $"DELETE t FROM {dbContext.TableName} t WHERE t.{colunmName} = @t{colunmName}";
        }

        public static string Delete<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter, out IDictionary<string, object> parameters) where TEntity : class
        {
            parameters = new Dictionary<string, object>();
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                case DataBaseType.MySql:
                    dbContext.SqlStatement = $"DELETE {filter.Parameters[0].Name} From {dbContext.TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter, out parameters)}";
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

        #region Queryable Methods

        public static string QueryableWhere<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter, out IDictionary<string, object> parameters) where TEntity : class
        {
            parameters = new Dictionary<string, object>();
            string result = string.Empty;
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                case DataBaseType.MySql:
                    result = LambdaToSql.ConvertWhere(filter, out parameters); break;
                case DataBaseType.Oracle:
                    result = string.Empty; break;
                default:
                    result = string.Empty; break;
            }
            return result;
        }

        public static string QueryableOrderBy<TEntity>(DbContext dbContext, Expression<Func<TEntity, object>> orderBy, bool isDESC) where TEntity : class
        {
            if (orderBy == null)
            {
                return string.Empty;
            }

            string result = string.Empty;
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                case DataBaseType.MySql:
                    string desc = isDESC ? "DESC" : "ASC";
                    result = $" ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}"; break;
                case DataBaseType.Oracle:
                    result = string.Empty; break;
                default:
                    result = string.Empty; break;
            }
            return result;
        }

        public static List<string> QueryableSelect<TEntity>(DbContext dbContext, Expression<Func<TEntity, object>> columns) where TEntity : class
        {
            return LambdaToSql.ConvertColumns<TEntity>(columns);
        }

        public static string QueryableQueryCount<TEntity>(DbContext dbContext, string alias, string where) where TEntity : class
        {
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                case DataBaseType.MySql:
                    dbContext.SqlStatement = $"SELECT COUNT(0) FROM {dbContext.TableName} {alias} {where}"; break;
                case DataBaseType.Oracle:
                    dbContext.SqlStatement = string.Empty; break;
                default:
                    dbContext.SqlStatement = string.Empty; break;
            }
            return dbContext.SqlStatement;
        }

        public static string QueryableQuery<TEntity>(DbContext dbContext, List<string> columns, string alias, string where, string orderBy, string top) where TEntity : class
        {
            string queryColumns = (columns == null || !columns.Any()) ? "*" : string.Join(",", columns.Select(t => $"{alias}.{t}"));
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                    dbContext.SqlStatement = $"SELECT {top} {queryColumns} FROM {dbContext.TableName} {alias} {where} {orderBy}"; break;
                case DataBaseType.MySql:
                    dbContext.SqlStatement = $"SELECT {queryColumns} FROM {dbContext.TableName} {alias} {where} {orderBy} {top}"; break;
                case DataBaseType.Oracle:
                    dbContext.SqlStatement = string.Empty; break;
                default:
                    dbContext.SqlStatement = string.Empty; break;
            }
            return dbContext.SqlStatement;
        }

        //目前queryablePaging是最终的结果了
        public static string QueryablePaging<TEntity>(DbContext dbContext, List<string> columns, string alias, string where, string orderBy, int pageIndex, int pageSize) where TEntity : class
        {
            string queryColumns = (columns == null || !columns.Any()) ? "*" : string.Join(",", columns.Select(t => $"TTTTTT.{t}"));
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                    string queryColumnsChild = (columns == null || !columns.Any()) ? "*" : string.Join(",", columns.Select(t => $"{alias}.{t}"));
                    dbContext.SqlStatement = $"SELECT TOP {pageSize} {queryColumns} FROM (SELECT ROW_NUMBER() OVER ({orderBy}) AS RowNumber,{queryColumnsChild} FROM {dbContext.TableName} {alias} {where}) AS TTTTTT  WHERE RowNumber > {pageSize * (pageIndex - 1)}"; break;
                case DataBaseType.MySql:
                    dbContext.SqlStatement = $"SELECT {string.Join(",", columns.Select(t => $"{alias}.{t}"))} FROM {dbContext.TableName} {alias} {where} {orderBy} LIMIT {pageIndex * pageSize},{pageSize}"; break;
                case DataBaseType.Oracle:
                    dbContext.SqlStatement = string.Empty; break;
                default:
                    dbContext.SqlStatement = string.Empty; break;
            }
            return dbContext.SqlStatement;
        }

        #endregion
    }
}
