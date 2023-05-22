/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 1/7/2019, 3:42:48 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 
* Thx , Best Regards ~
*********************************************************/
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace SevenTiny.Bantina.Bankinate.SqlDataAccess
{
    /// <summary>
    /// Auto Fill Adapter
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    internal class FillAdapter<Entity>
    {
        private static readonly Func<DataRow, Entity> funcCache = GetFactory();
        public static Entity AutoFill(DataRow row)
        {
            return funcCache(row);
        }
        private static Func<DataRow, Entity> GetFactory()
        {
            var type = typeof(Entity);
            var rowType = typeof(DataRow);
            var rowDeclare = Expression.Parameter(rowType, "row");
            var instanceDeclare = Expression.Parameter(type, "t");
            //new Student()
            var newExpression = Expression.New(type);
            //(t = new Student())
            var instanceExpression = Expression.Assign(instanceDeclare, newExpression);
            //row == null
            var nullEqualExpression = Expression.NotEqual(rowDeclare, Expression.Constant(null));
            var containsMethod = typeof(DataColumnCollection).GetMethod("Contains");
            var indexerMethod = rowType.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, new[] { new ParameterModifier(1) });
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var setExpressions = new List<Expression>();
            //row.Table.Columns
            var columns = Expression.Property(Expression.Property(rowDeclare, "Table"), "Columns");
            foreach (var propertyInfo in properties)
            {
                // no column , no translation
                if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) == null)
                    continue;

                if (propertyInfo.CanWrite)
                {
                    //Id,Id is a property of Entity
                    var propertyName = Expression.Constant(propertyInfo.Name, typeof(string));
                    //row.Table.Columns.Contains("Id")
                    var checkIfContainsColumn = Expression.Call(columns, containsMethod, propertyName);
                    //t.Id
                    var propertyExpression = Expression.Property(instanceDeclare, propertyInfo);
                    //row.get_Item("Id")
                    var value = Expression.Call(rowDeclare, indexerMethod, propertyName);
                    //t.Id = Convert(row.get_Item("Id"), Int32)
                    var propertyAssign = Expression.Assign(propertyExpression, Expression.Convert(value, propertyInfo.PropertyType));
                    //t.Id = default(Int32)
                    var propertyAssignDefault = Expression.Assign(propertyExpression, Expression.Default(propertyInfo.PropertyType));
                    //if(row.Table.Columns.Contains("Id")&&!value.Equals(DBNull.Value<>)) {t.Id = Convert(row.get_Item("Id"), Int32)}else{t.Id = default(Int32)}
                    var checkRowNull = Expression.IfThenElse(Expression.AndAlso(checkIfContainsColumn, Expression.NotEqual(value, Expression.Constant(System.DBNull.Value))), propertyAssign, propertyAssignDefault);
                    //var checkContains = Expression.IfThen(checkIfContainsColumn, propertyAssign);
                    setExpressions.Add(checkRowNull);
                }
            }
            var checkIfRowIsNull = Expression.IfThen(nullEqualExpression, Expression.Block(setExpressions));
            var body = Expression.Block(new[] { instanceDeclare }, instanceExpression, checkIfRowIsNull, instanceDeclare);
            return Expression.Lambda<Func<DataRow, Entity>>(body, rowDeclare).Compile();
        }
    }
}
