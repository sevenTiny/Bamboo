using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Exceptions;
using SevenTiny.Bantina.Bankinate.Extensions;
using SevenTiny.Bantina.Bankinate.SqlStatementManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SevenTiny.Bantina.Bankinate.MySql.SqlStatementManagement
{
    internal class MySqlCommandTextGenerator : CommandTextGeneratorBase
    {
        public MySqlCommandTextGenerator(SqlDbContext _dbContext) : base(_dbContext) { }

        public override void SetWhere<TEntity>(Expression<Func<TEntity, bool>> where)
        {
            this._where = LambdaToSql.ConvertWhere(where, out IDictionary<string, object> parameters);
            DbContext.Parameters = parameters;
        }
        public override void SetOrderBy<TEntity>(Expression<Func<TEntity, object>> orderBy, bool isDesc)
        {
            if (orderBy == null)
                return;
            string desc = isDesc ? "DESC" : "ASC";
            this._orderBy = $" ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}".TrimEnd();
        }
        public override void SetPage(int pageIndex, int pageSize)
        {
            this._pageIndex = pageIndex;
            this._pageSize = pageSize;
        }
        public override void SetLimit(int count)
        {
            _limit = $"LIMIT {count}";
        }
        public override void SetAlias(string alias)
        {
            this._alias = alias;
        }
        public override void SetColumns<TEntity>(Expression<Func<TEntity, object>> columns)
        {
            _columns = LambdaToSql.ConvertColumns<TEntity>(columns);
        }

        public override string Add<TEntity>(TEntity entity)
        {
            DbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            DbContext.Parameters = new Dictionary<string, object>();

            StringBuilder builder_front = new StringBuilder(), builder_behind = new StringBuilder();
            builder_front.Append("INSERT INTO ");
            builder_front.Append(DbContext.TableName);
            builder_front.Append(" (");
            builder_behind.Append(" VALUES (");

            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                }
                //Column
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute column)
                {
                    builder_front.Append(column.GetName(propertyInfo.Name));
                    builder_front.Append(",");
                    builder_behind.Append("@");
                    columnName = column.GetName(propertyInfo.Name).Replace("`", "");
                    builder_behind.Append(columnName);
                    builder_behind.Append(",");

                    DbContext.Parameters.AddOrUpdate($"@{columnName}", propertyInfo.GetValue(entity));
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
            return DbContext.SqlStatement = builder_front.Append(builder_behind.ToString()).ToString().TrimEnd();
        }
        public override string Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity)
        {
            DbContext.Parameters = new Dictionary<string, object>();
            DbContext.TableName = TableAttribute.GetName(typeof(TEntity));

            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
            builder_front.Append(DbContext.TableName);
            builder_front.Append(" ");

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
                    columnName = columnAttr.GetName(propertyInfo.Name).Replace("`", "");
                    builder_front.Append(columnName);
                    builder_front.Append(",");

                    DbContext.Parameters.AddOrUpdate($"@{alias}{columnName}", propertyInfo.GetValue(entity));
                }
                //in the end,remove the redundant symbol of ','
                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                }
            }

            //Generate SqlStatement
            return DbContext.SqlStatement = builder_front.Append($"{LambdaToSql.ConvertWhere(filter)}").ToString().TrimEnd();
        }
        public override string Update<TEntity>(TEntity entity, out Expression<Func<TEntity, bool>> filter)
        {
            DbContext.Parameters = new Dictionary<string, object>();
            DbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));

            //查找主键以及主键对应的值，如果用该方法更新数据，主键是必须存在的
            //get property which is key
            var keyProperty = propertyInfos.Where(t => t.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute)?.FirstOrDefault();
            if (keyProperty == null)
                throw new TableKeyNotFoundException($"table '{DbContext.TableName}' not found key column");

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
            DbContext.Parameters.AddOrUpdate($"@t{keyName}", keyValue);

            //开始构造赋值的sql语句
            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
            //Mysql和sqlserver的分别处理
            if (DbContext.DataBaseType == DataBaseType.MySql)
            {
                builder_front.Append(DbContext.TableName);
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
                    columnName = columnAttr.GetName(propertyInfo.Name).Replace("`", "");
                    builder_front.Append(columnName);
                    builder_front.Append(",");

                    DbContext.Parameters.AddOrUpdate($"@{alias}{columnName}", propertyInfo.GetValue(entity));
                }
                //in the end,remove the redundant symbol of ','
                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                }
            }

            //SqlServer和Mysql的sql语句分别处理
            if (DbContext.DataBaseType == DataBaseType.SqlServer)
            {
                builder_front.Append(" FROM ");
                builder_front.Append(DbContext.TableName);
                builder_front.Append(" ");
                builder_front.Append(alias);
            }

            //Generate SqlStatement
            return DbContext.SqlStatement = builder_front.Append($"{LambdaToSql.ConvertWhere(filter)}").ToString().TrimEnd();
        }
        public override string Delete<TEntity>(TEntity entity)
        {
            DbContext.Parameters = new Dictionary<string, object>();
            DbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            //get property which is key
            var property = propertyInfos.Where(t => t.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute)?.FirstOrDefault();
            if (property == null)
                throw new TableKeyNotFoundException($"table '{DbContext.TableName}' not found key column");

            string colunmName = property.Name;
            var value = property.GetValue(entity);

            if (property.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                colunmName = columnAttr.GetName(property.Name);

            DbContext.Parameters.AddOrUpdate($"@t{colunmName}", value);
            return DbContext.SqlStatement = $"DELETE t FROM {DbContext.TableName} t WHERE t.{colunmName} = @t{colunmName}".TrimEnd();
        }
        public override string Delete<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            DbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            DbContext.SqlStatement = $"DELETE {filter.Parameters[0].Name} From {DbContext.TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter, out parameters)}".TrimEnd();
            DbContext.Parameters = parameters;
            return DbContext.SqlStatement;
        }

        public override string QueryableCount<TEntity>()
        {
            return DbContext.SqlStatement = $"SELECT COUNT(0) FROM {DbContext.TableName} {_alias} {_where}".TrimEnd();
        }
        public override string QueryableQuery<TEntity>()
        {
            string queryColumns = (_columns == null || !_columns.Any()) ? "*" : string.Join(",", _columns.Select(t => $"{_alias}.{t}"));
            return DbContext.SqlStatement = $"SELECT {queryColumns} FROM {DbContext.TableName} {_alias} {_where} {_orderBy} {_limit}".TrimEnd();
        }
        public override string QueryablePaging<TEntity>()
        {
            string queryColumns = (_columns == null || !_columns.Any()) ? "*" : string.Join(",", _columns.Select(t => $"TTTTTT.{t}").ToArray());
            return DbContext.SqlStatement = $"SELECT {string.Join(",", _columns.Select(t => $"{_alias}.{t}"))} FROM {DbContext.TableName} {_alias} {_where} {_orderBy} LIMIT {_pageIndex * _pageSize},{_pageSize}".TrimEnd();
        }
        public override string QueryableAny<TEntity>()
        {
            SetLimit(1);
            return DbContext.SqlStatement = $"SELECT 1 FROM {DbContext.TableName} {_alias} {_where} {_limit}".TrimEnd();
        }
    }
}