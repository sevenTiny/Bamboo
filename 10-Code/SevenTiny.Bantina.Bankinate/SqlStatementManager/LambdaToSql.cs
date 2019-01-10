/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-19 23:58:08
 * Modify: 2018-04-19 23:58:08
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SevenTiny.Bantina.Bankinate.SqlStatementManager
{
    internal class LambdaToSql
    {
        public static string ConvertWhere<T>(Expression<Func<T, bool>> where) where T : class
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();

            StringBuilder builder = new StringBuilder();
            builder.Append(" WHERE ");
            if (where.Body is BinaryExpression be)
            {
                return builder.Append(BinarExpressionProvider(be.Left, be.Right, be.NodeType, ref parameters)).ToString();
            }
            return builder.Append(ExpressionRouter(where.Body, ref parameters)).ToString();
        }

        public static string ConvertWhere<T>(Expression<Func<T, bool>> where, out IDictionary<string, object> parameters) where T : class
        {
            parameters = new Dictionary<string, object>();

            StringBuilder builder = new StringBuilder();
            builder.Append(" WHERE ");
            if (where.Body is BinaryExpression be)
            {
                return builder.Append(BinarExpressionProvider(be.Left, be.Right, be.NodeType, ref parameters)).ToString();
            }
            return builder.Append(ExpressionRouter(where.Body, ref parameters)).ToString();
        }

        public static string ConvertOrderBy<T>(Expression<Func<T, object>> orderby) where T : class
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();

            if (orderby.Body is UnaryExpression ue)
            {
                return ExpressionRouter(ue.Operand, ref parameters);
            }
            else
            {
                MemberExpression order = (MemberExpression)orderby.Body;
                return order.Member.Name;
            }
        }

        //转换查询列
        public static List<string> ConvertColumns<TEntity>(Expression<Func<TEntity, object>> columns) where TEntity : class
        {
            if (columns == null)
            {
                return null;
            }
            List<string> strList = new List<string>();
            if (columns.Body is NewExpression)
            {
                var newExp = columns.Body as NewExpression;
                foreach (var nExp in newExp.Arguments)
                {
                    strList.Add(GetFieldAttribute(nExp));
                }
            }
            else
            {
                strList.Add(GetFieldAttribute(columns.Body));
            }
            return strList;
        }
        //通过Attribute获取需要查找的字段列表
        private static string GetFieldAttribute(Expression exp)
        {
            if (exp is UnaryExpression)
            {
                var ue = exp as UnaryExpression;
                return GetFieldAttribute(ue.Operand);
            }
            else if (exp is MemberExpression)
            {
                var mem = exp as MemberExpression;
                var member = mem.Member;
                var metaFieldAttr = member.GetCustomAttributes(typeof(ColumnAttribute), true)?.FirstOrDefault();
                var metaFieldName = (metaFieldAttr as ColumnAttribute)?.Name ?? member.Name;
                return metaFieldName;
            }
            else
            {
                MemberExpression order = (MemberExpression)exp;
                return GetFieldAttribute(order);
            }
        }

        private static string BinarExpressionProvider(Expression left, Expression right, ExpressionType type, ref IDictionary<string, object> parameters)
        {
            var leftValue = ExpressionRouter(left, ref parameters);
            var typeCast = ExpressionTypeCast(type);
            var rightValue = ExpressionRouter(right, ref parameters);

            if (left is MemberExpression && right is ConstantExpression)
            {
                var keyNameNoPoint = leftValue.Replace(".", "");

                parameters.AddOrUpdate($"@{keyNameNoPoint}", $"{rightValue}");
                return $"{leftValue} {typeCast} @{keyNameNoPoint}";
            }
            else
            {
                return $"{leftValue} {typeCast} {rightValue}";
            }
        }

        private static string ExpressionRouter(Expression exp, ref IDictionary<string, object> parameters)
        {
            if (exp is BinaryExpression be)
            {
                return BinarExpressionProvider(be.Left, be.Right, be.NodeType, ref parameters);
            }
            else if (exp is MemberExpression me)
            {
                if (!exp.ToString().StartsWith("value"))
                {
                    return me.ToString();
                }
                else
                {
                    var result = Expression.Lambda(exp).Compile().DynamicInvoke();
                    if (result == null)
                    {
                        return "NULL";
                    }
                    else if (result is ValueType)
                    {
                        if (result is Guid)
                        {
                            return $"'{result}'";
                        }
                        return result.ToString();
                    }
                    else if (result is string || result is DateTime || result is char)
                    {
                        return $"'{result}'";
                    }
                }
            }
            else if (exp is NewArrayExpression ae)
            {
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(ExpressionRouter(ex, ref parameters));
                    tmpstr.Append(",");
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression mce)
            {
                //get value
                string value = null;
                if (mce.Object == null)
                {
                    value = Expression.Lambda(mce).Compile().DynamicInvoke().ToString();
                }
                else
                {
                    value = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke().ToString();
                }

                //参数名
                var keyName = mce.Object.ToString();
                var keyNameNoPoint = keyName.Replace(".","");

                if (mce.Method.Name.Equals("Equals"))
                {
                    parameters.AddOrUpdate($"@{keyNameNoPoint}", $"{value}");
                    return $"{keyName} = @{keyNameNoPoint}";
                }
                else if (mce.Method.Name.Equals("Contains"))
                {
                    parameters.AddOrUpdate($"@{keyNameNoPoint}", $"%{value.Replace("'", "")}%");
                    return $"{keyName} LIKE @{keyNameNoPoint}";
                }
                else if (mce.Method.Name.Equals("StartsWith"))
                {
                    parameters.AddOrUpdate($"@{keyNameNoPoint}", $"{value.Replace("'", "")}%");
                    return $"{keyName} LIKE @{keyNameNoPoint}";
                }
                else if (mce.Method.Name.Equals("EndsWith"))
                {
                    parameters.AddOrUpdate($"@{keyNameNoPoint}", $"%{value.Replace("'", "")}");
                    return $"{keyName} LIKE @{keyNameNoPoint}";
                }
                return value;
            }
            else if (exp is ConstantExpression ce)
            {
                if (ce.Value == null)
                {
                    return "NULL";
                }
                else if (ce.Value is ValueType)
                {
                    if (ce.Value is bool b)
                    {
                        if (b)
                            return " 1=1 ";
                        else
                            return " 1=2 ";
                    }
                    return ce.Value.ToString();
                }
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                {
                    return $"'{ce.Value.ToString()}'";
                }
                return " ";
            }
            else if (exp is UnaryExpression ue)
            {
                return ExpressionRouter(ue.Operand, ref parameters);
            }
            return null;
        }

        private static string ExpressionTypeCast(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                    return " AND ";
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                    return " Or ";
                case ExpressionType.OrElse:
                    return " Or ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.MultiplyChecked:
                    return "*";
                default:
                    return null;
            }
        }
    }
}

