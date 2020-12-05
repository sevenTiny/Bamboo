/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-09 16:55:16
 * Modify: 2018-04-09 16:55:16
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
    internal sealed class MapperExpressionCommon
    {
        /// <summary>
        /// structure func
        /// </summary>
        /// <param name="outType"></param>
        /// <param name="inTypes"></param>
        /// <param name="memberInitExpression"></param>
        /// <param name="parameterExpressionList"></param>
        public static void GetFunc(Type outType, Type[] inTypes, out MemberInitExpression memberInitExpression, out List<ParameterExpression> parameterExpressionList)
        {
            parameterExpressionList = new List<ParameterExpression>();
            List<MemberBinding> memberBindingList = new List<MemberBinding>();
            PropertyInfo[] propertyInfos = outType.GetProperties();
            Dictionary<string, PropertyInfo> outPropertyDic = propertyInfos.ToDictionary(t => t.Name, t => t);
            foreach (var inType in inTypes)
            {
                ParameterExpression parameterExpression = Expression.Parameter(inType, inType.FullName);
                PropertyInfo[] inTypePpropertyInfos = inType.GetProperties();
                foreach (var inTypeInfo in inTypePpropertyInfos)
                {
                    if (inTypeInfo.GetCustomAttribute(typeof(DoNotMapperAttribute)) == null)
                    {
                        //first
                        string outPropertyDicKey = MapperAttribute.GetTargetName(inTypeInfo);
                        //second
                        if (string.IsNullOrEmpty(outPropertyDicKey) && outPropertyDic.Keys.Contains(inTypeInfo.Name))
                        {
                            outPropertyDicKey = inTypeInfo.Name;
                        }
                        //third
                        if (!string.IsNullOrEmpty(outPropertyDicKey) && outPropertyDic.Keys.Contains(outPropertyDicKey))
                        {
                            MemberExpression property = Expression.Property(parameterExpression, inTypeInfo);
                            MemberBinding memberBinding = Expression.Bind(outPropertyDic[outPropertyDicKey], property);
                            memberBindingList.Add(memberBinding);
                            outPropertyDic.Remove(outPropertyDicKey);//remove property if has be valued
                        }
                    }
                }
                if (!parameterExpressionList.Exists(t => t.Name.Equals(parameterExpression.Name)))
                {
                    parameterExpressionList.Add(parameterExpression);
                }
            }
            memberInitExpression = Expression.MemberInit(Expression.New(outType), memberBindingList.ToArray());
        }
    }

}
