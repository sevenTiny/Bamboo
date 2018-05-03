/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-03-16 10:11:43
 * Modify: 2018-4-3 11:35:53
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

namespace SevenTiny.Bantina.AutoMapper
{
    public sealed class Mapper<TIn, TOut> where TOut : class where TIn : class
    {
        private Mapper() { }
        private static readonly Func<TIn, TOut> funcCache = GetFunc();
        public static TOut AutoMapper(TIn tIn)
        {
            return funcCache(tIn);
        }
        public static TOut AutoMapper(TIn tIn, Action<TOut> action)
        {
            TOut outValue = funcCache(tIn);
            action(outValue);
            return outValue;
        }
        private static Func<TIn, TOut> GetFunc()
        {
            Type[] types = new Type[] { typeof(TIn) };
            MemberInitExpression memberInitExpression;
            List<ParameterExpression> parameterExpressionList;
            MapperExpressionCommon.GetFunc(typeof(TOut), types, out memberInitExpression, out parameterExpressionList);
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, parameterExpressionList);
            return lambda.Compile();
        }
    }
    public sealed class Mapper<TIn1, TIn2, TOut> where TOut : class where TIn1 : class where TIn2 : class
    {
        private Mapper() { }
        public static TOut AutoMapper(TIn1 tIn1, TIn2 tIn2)
        {
            return funcCache(tIn1, tIn2);
        }
        public static TOut AutoMapper(TIn1 tIn1, TIn2 tIn2, Action<TOut> action)
        {
            TOut outValue = funcCache(tIn1, tIn2);
            action(outValue);
            return outValue;
        }
        private static readonly Func<TIn1, TIn2, TOut> funcCache = GetFunc();
        private static Func<TIn1, TIn2, TOut> GetFunc()
        {
            Type[] types = new Type[] { typeof(TIn1), typeof(TIn2) };
            MemberInitExpression memberInitExpression;
            List<ParameterExpression> parameterExpressionList;
            MapperExpressionCommon.GetFunc(typeof(TOut), types, out memberInitExpression, out parameterExpressionList);
            Expression<Func<TIn1, TIn2, TOut>> lambda = Expression.Lambda<Func<TIn1, TIn2, TOut>>(memberInitExpression, parameterExpressionList);
            return lambda.Compile();
        }
    }
    public sealed class Mapper<TIn1, TIn2, TIn3, TOut> where TOut : class where TIn1 : class where TIn2 : class where TIn3 : class
    {
        private Mapper() { }
        public static TOut AutoMapper(TIn1 tIn1, TIn2 tIn2, TIn3 tIn3)
        {
            return funcCache(tIn1, tIn2, tIn3);
        }
        public static TOut AutoMapper(TIn1 tIn1, TIn2 tIn2, TIn3 tIn3, Action<TOut> action)
        {
            TOut outValue = funcCache(tIn1, tIn2, tIn3);
            action(outValue);
            return outValue;
        }
        private static readonly Func<TIn1, TIn2, TIn3, TOut> funcCache = GetFunc();
        private static Func<TIn1, TIn2, TIn3, TOut> GetFunc()
        {
            Type[] types = new Type[] { typeof(TIn1), typeof(TIn2), typeof(TIn3) };
            MemberInitExpression memberInitExpression;
            List<ParameterExpression> parameterExpressionList;
            MapperExpressionCommon.GetFunc(typeof(TOut), types, out memberInitExpression, out parameterExpressionList);
            Expression<Func<TIn1, TIn2, TIn3, TOut>> lambda = Expression.Lambda<Func<TIn1, TIn2, TIn3, TOut>>(memberInitExpression, parameterExpressionList);
            return lambda.Compile();
        }
    }
    public sealed class Mapper<TIn1, TIn2, TIn3, TIn4, TOut> where TOut : class where TIn1 : class where TIn2 : class where TIn3 : class where TIn4 : class
    {
        private Mapper() { }
        public static TOut AutoMapper(TIn1 tIn1, TIn2 tIn2, TIn3 tIn3, TIn4 tIn4)
        {
            return funcCache(tIn1, tIn2, tIn3, tIn4);
        }
        public static TOut AutoMapper(TIn1 tIn1, TIn2 tIn2, TIn3 tIn3, TIn4 tIn4, Action<TOut> action)
        {
            TOut outValue = funcCache(tIn1, tIn2, tIn3, tIn4);
            action(outValue);
            return outValue;
        }
        private static readonly Func<TIn1, TIn2, TIn3, TIn4, TOut> funcCache = GetFunc();
        private static Func<TIn1, TIn2, TIn3, TIn4, TOut> GetFunc()
        {
            Type[] types = new Type[] { typeof(TIn1), typeof(TIn2), typeof(TIn3), typeof(TIn4) };
            MemberInitExpression memberInitExpression;
            List<ParameterExpression> parameterExpressionList;
            MapperExpressionCommon.GetFunc(typeof(TOut), types, out memberInitExpression, out parameterExpressionList);
            Expression<Func<TIn1, TIn2, TIn3, TIn4, TOut>> lambda = Expression.Lambda<Func<TIn1, TIn2, TIn3, TIn4, TOut>>(memberInitExpression, parameterExpressionList);
            return lambda.Compile();
        }
    }
    public sealed class Mapper<TIn1, TIn2, TIn3, TIn4, TIn5, TOut> where TOut : class where TIn1 : class where TIn2 : class where TIn3 : class where TIn4 : class where TIn5 : class
    {
        private Mapper() { }
        public static TOut AutoMapper(TIn1 tIn1, TIn2 tIn2, TIn3 tIn3, TIn4 tIn4, TIn5 tIn5)
        {
            return funcCache(tIn1, tIn2, tIn3, tIn4, tIn5);
        }
        public static TOut AutoMapper(TIn1 tIn1, TIn2 tIn2, TIn3 tIn3, TIn4 tIn4, TIn5 tIn5, Action<TOut> action)
        {
            TOut outValue = funcCache(tIn1, tIn2, tIn3, tIn4, tIn5);
            action(outValue);
            return outValue;
        }
        private static readonly Func<TIn1, TIn2, TIn3, TIn4, TIn5, TOut> funcCache = GetFunc();
        private static Func<TIn1, TIn2, TIn3, TIn4, TIn5, TOut> GetFunc()
        {
            Type[] types = new Type[] { typeof(TIn1), typeof(TIn2), typeof(TIn3), typeof(TIn4), typeof(TIn5) };
            MemberInitExpression memberInitExpression;
            List<ParameterExpression> parameterExpressionList;
            MapperExpressionCommon.GetFunc(typeof(TOut), types, out memberInitExpression, out parameterExpressionList);
            Expression<Func<TIn1, TIn2, TIn3, TIn4, TIn5, TOut>> lambda = Expression.Lambda<Func<TIn1, TIn2, TIn3, TIn4, TIn5, TOut>>(memberInitExpression, parameterExpressionList);
            return lambda.Compile();
        }
    }
}
