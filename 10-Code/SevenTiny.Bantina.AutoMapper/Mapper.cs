/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-03-16 10:11:43
 * Modify: 2018-03-16 10:11:43
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SevenTiny.Bantina.AutoMapper
{
    public sealed class Mapper
    {
        private Mapper() { }

        private static Dictionary<string, object> sourceValueDic;
        /// <summary>
        /// Init Source Value Dic
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        private static void InitSourceValueDic<TSource>(TSource source)
        {
            //init and clear dic
            if (sourceValueDic == null)
                sourceValueDic = new Dictionary<string, object>();
            else
                sourceValueDic.Clear();
            //init dic
            foreach (PropertyInfo property in typeof(TSource).GetProperties())
            {
                if (!sourceValueDic.ContainsKey(property.Name))
                {
                    sourceValueDic.Add(property.Name, property.GetValue(source));
                }
            }
        }

        /// <summary>
        /// AutoMapper
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <typeparam name="TSource">source type</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TValue AutoMapper<TValue, TSource>(TSource source) where TValue : class where TSource : class
        {
            //init dic
            InitSourceValueDic(source);
            //create instance of TValue
            TValue value = Activator.CreateInstance<TValue>();
            foreach (PropertyInfo property in typeof(TValue).GetProperties())
            {
                if (sourceValueDic.ContainsKey(property.Name))
                {
                    property.SetValue(value, sourceValueDic[property.Name]);
                }
            }
            return value;
        }

        public static TValue AutoMapper<TValue, TSource1, TSource2>(TSource1 source1, TSource2 source2) where TValue : class where TSource1 : class where TSource2 : class
        {
            return default(TValue);
        }
    }
}
