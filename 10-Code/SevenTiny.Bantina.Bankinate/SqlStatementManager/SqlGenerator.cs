using SevenTiny.Bantina.Bankinate.Attributes;
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

        public static string Add<TEntity>(DataBaseType dataBaseType, TEntity entity, out string tableName, out Dictionary<string, object> paramsDic) where TEntity : class
        {
            tableName = TableAttribute.GetName(typeof(TEntity));
            paramsDic = new Dictionary<string, object>();

            StringBuilder builder_front = new StringBuilder(), builder_behind = new StringBuilder();
            builder_front.Append("INSERT INTO ");
            builder_front.Append(tableName);
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
            return builder_front.Append(builder_behind.ToString()).ToString();
        }

        public static string Update<TEntity>(DataBaseType dataBaseType, Expression<Func<TEntity, bool>> filter, TEntity entity, out string tableName, out Dictionary<string, object> paramsDic) where TEntity : class
        {
            tableName = TableAttribute.GetName(typeof(TEntity));
            paramsDic = new Dictionary<string, object>();

            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
            builder_front.Append(tableName);
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
                    columnName = columnAttr.GetName(propertyInfo.Name).Replace("[", "").Replace("]", "");
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
            //Generate SqlStatement
            return builder_front.Append($"{LambdaToSql.ConvertWhere(filter)}").ToString();
        }

        public static string Delete<TEntity>(DataBaseType dataBaseType, Expression<Func<TEntity, bool>> filter, out string tableName) where TEntity : class
        {
            tableName = TableAttribute.GetName(typeof(TEntity));
            switch (dataBaseType)
            {
                case DataBaseType.SqlServer:
                    return $"DELETE {filter.Parameters[0].Name} From {tableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";
                case DataBaseType.MySql:
                    return string.Empty;
                case DataBaseType.Oracle:
                    return string.Empty;
                case DataBaseType.MongoDB:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        public static string QueryOne<TEntity>(DataBaseType dataBaseType, Expression<Func<TEntity, bool>> filter, out string tableName) where TEntity : class
        {
            tableName = TableAttribute.GetName(typeof(TEntity));
            switch (dataBaseType)
            {
                case DataBaseType.SqlServer:
                    return $"SELECT TOP 1 * FROM {tableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";
                case DataBaseType.MySql:
                    return string.Empty;
                case DataBaseType.Oracle:
                    return string.Empty;
                case DataBaseType.MongoDB:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        public static string QueryCount<TEntity>(DataBaseType dataBaseType, Expression<Func<TEntity, bool>> filter, out string tableName) where TEntity : class
        {
            tableName = TableAttribute.GetName(typeof(TEntity));
            switch (dataBaseType)
            {
                case DataBaseType.SqlServer:
                    return $"SELECT COUNT(0) FROM {tableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";
                case DataBaseType.MySql:
                    return string.Empty;
                case DataBaseType.Oracle:
                    return string.Empty;
                case DataBaseType.MongoDB:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        public static string Query<TEntity>(DataBaseType dataBaseType, Expression<Func<TEntity, bool>> filter, out string tableName) where TEntity : class
        {
            tableName = TableAttribute.GetName(typeof(TEntity));
            switch (dataBaseType)
            {
                case DataBaseType.SqlServer:
                    return $"SELECT * FROM {tableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}";
                case DataBaseType.MySql:
                    return string.Empty;
                case DataBaseType.Oracle:
                    return string.Empty;
                case DataBaseType.MongoDB:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        public static string QueryOrderBy<TEntity>(DataBaseType dataBaseType, Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> orderBy, bool isDESC, out string tableName) where TEntity : class
        {
            tableName = TableAttribute.GetName(typeof(TEntity));
            switch (dataBaseType)
            {
                case DataBaseType.SqlServer:
                    string desc = isDESC ? "DESC" : "ASC";
                    return $"SELECT * FROM {tableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)} ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}";
                case DataBaseType.MySql:
                    return string.Empty;
                case DataBaseType.Oracle:
                    return string.Empty;
                case DataBaseType.MongoDB:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        public static string QueryPaging<TEntity>(DataBaseType dataBaseType, int pageIndex, int pageSize, Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, object>> orderBy, bool isDESC, out string tableName) where TEntity : class
        {
            tableName = TableAttribute.GetName(typeof(TEntity));
            switch (dataBaseType)
            {
                case DataBaseType.SqlServer:
                    string desc = isDESC ? "DESC" : "ASC";
                    return $"SELECT TOP {pageSize} * FROM (SELECT ROW_NUMBER() OVER (ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}) AS RowNumber,* FROM {tableName} {orderBy.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter)}) AS TTTTTT  WHERE RowNumber > {pageSize * (pageIndex - 1)}";
                case DataBaseType.MySql:
                    return string.Empty;
                case DataBaseType.Oracle:
                    return string.Empty;
                case DataBaseType.MongoDB:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
