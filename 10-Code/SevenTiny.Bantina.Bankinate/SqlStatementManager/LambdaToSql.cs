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
            StringBuilder builder = new StringBuilder();
            builder.Append(" WHERE ");
            if (where.Body is BinaryExpression be)
            {
                return builder.Append(BinarExpressionProvider(be.Left, be.Right, be.NodeType)).ToString();
            }
            return builder.Append(ExpressionRouter(where.Body)).ToString();
        }

        public static string ConvertOrderBy<T>(Expression<Func<T, object>> orderby) where T : class
        {
            if (orderby.Body is UnaryExpression ue)
            {
                return ExpressionRouter(ue.Operand);
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
                var metaFieldName =  (metaFieldAttr as ColumnAttribute)?.Name ?? member.Name;
                return metaFieldName;
            }
            else
            {
                MemberExpression order = (MemberExpression)exp;
                return GetFieldAttribute(order);
            }
        }

        private static string BinarExpressionProvider(Expression left, Expression right, ExpressionType type)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ExpressionRouter(left));
            builder.Append(ExpressionTypeCast(type));
            builder.Append(ExpressionRouter(right));
            return builder.ToString();
        }

        private static string ExpressionRouter(Expression exp)
        {
            if (exp is BinaryExpression be)
            {
                return BinarExpressionProvider(be.Left, be.Right, be.NodeType);
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
                    tmpstr.Append(ExpressionRouter(ex));
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

                if (mce.Method.Name.Equals("Equals"))
                {
                    return $"{mce.Object.ToString()} = '{value}'";
                }
                else if (mce.Method.Name.Equals("Contains"))
                {
                    return $"{mce.Object.ToString()} LIKE '%{value.Replace("'", "")}%'";
                }
                else if (mce.Method.Name.Equals("StartsWith"))
                {
                    return $"{mce.Object.ToString()} LIKE '{value.Replace("'", "")}%'";
                }
                else if (mce.Method.Name.Equals("EndsWith"))
                {
                    return $"{mce.Object.ToString()} LIKE '%{value.Replace("'", "")}'";
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
                    if (ce.Value is bool)
                    {
                        return " 1=1 ";
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
                return ExpressionRouter(ue.Operand);
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

