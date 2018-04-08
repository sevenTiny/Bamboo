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
using System.Reflection;

namespace SevenTiny.Bantina.AutoMapper
{
    public sealed class Mapper
    {
        private Mapper() { }
        /// <summary>
        /// Init Source Value Dic
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Dictionary<string, object> Initdic<TSource>(TSource source)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (PropertyInfo property in typeof(TSource).GetProperties())
            {
                string targetPropertyName = MapperAttribute.GetTargetName(property);
                if (!string.IsNullOrEmpty(targetPropertyName))
                {
                    if (!dic.ContainsKey(targetPropertyName))
                    {
                        dic.Add(targetPropertyName, property.GetValue(source));
                    }
                }
                else if (!dic.ContainsKey(property.Name))
                {
                    dic.Add(property.Name, property.GetValue(source));
                }
            }
            return dic;
        }
        /// <summary>
        /// SetValue from propertyinfo and sourceDictionary
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <param name="propertyInfos"></param>
        /// <param name="sourceDic"></param>
        /// <returns></returns>
        private static TValue SetValue<TValue>(TValue value, PropertyInfo[] propertyInfos, Dictionary<string, object> sourceDic, Dictionary<string, string> keys) where TValue : class
        {
            foreach (PropertyInfo property in propertyInfos)
            {
                if (!keys.ContainsKey(property.Name))
                {
                    if (sourceDic.ContainsKey(property.Name))
                    {
                        try
                        {
                            property.SetValue(value, sourceDic[property.Name]);
                            keys.Add(property.Name, string.Empty);
                        }
                        catch (Exception)
                        {
                            property.SetValue(value, null);
                        }
                    }
                }
            }
            return value;
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
            TValue value = Activator.CreateInstance<TValue>();
            PropertyInfo[] propertyInfos = typeof(TValue).GetProperties();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            value = SetValue(value, propertyInfos, Initdic(source), keys);
            return value;
        }
        /// <summary>
        /// AutoMapper,Support for Use Action to custom special fields.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TValue AutoMapper<TValue, TSource>(TSource source, Action<TValue> action) where TValue : class where TSource : class
        {
            TValue value = Activator.CreateInstance<TValue>();
            PropertyInfo[] propertyInfos = typeof(TValue).GetProperties();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            value = SetValue(value, propertyInfos, Initdic(source), keys);
            action(value);
            return value;
        }
        /// <summary>
        /// AutoMapper with multitype properties.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TSource1"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <returns></returns>
        public static TValue AutoMapper<TValue, TSource1, TSource2>(TSource1 source1, TSource2 source2) where TValue : class where TSource1 : class where TSource2 : class
        {
            TValue value = Activator.CreateInstance<TValue>();
            PropertyInfo[] propertyInfos = typeof(TValue).GetProperties();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            value = SetValue(value, propertyInfos, Initdic(source1), keys);
            value = SetValue(value, propertyInfos, Initdic(source2), keys);
            return value;
        }
        /// <summary>
        /// AutoMapper with multitype properties.Support for Use Action to custom special fields.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TSource1"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TValue AutoMapper<TValue, TSource1, TSource2>(TSource1 source1, TSource2 source2, Action<TValue> action) where TValue : class where TSource1 : class where TSource2 : class
        {
            TValue value = Activator.CreateInstance<TValue>();
            PropertyInfo[] propertyInfos = typeof(TValue).GetProperties();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            value = SetValue(value, propertyInfos, Initdic(source1), keys);
            value = SetValue(value, propertyInfos, Initdic(source2), keys);
            action(value);
            return value;
        }
        /// <summary>
        /// AutoMapper with multitype properties.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TSource1"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <typeparam name="TSource3"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <param name="source3"></param>
        /// <returns></returns>
        public static TValue AutoMapper<TValue, TSource1, TSource2, TSource3>(TSource1 source1, TSource2 source2, TSource3 source3) where TValue : class where TSource1 : class where TSource2 : class where TSource3 : class
        {
            TValue value = Activator.CreateInstance<TValue>();
            PropertyInfo[] propertyInfos = typeof(TValue).GetProperties();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            value = SetValue(value, propertyInfos, Initdic(source1), keys);
            value = SetValue(value, propertyInfos, Initdic(source2), keys);
            value = SetValue(value, propertyInfos, Initdic(source3), keys);
            return value;
        }
        /// <summary>
        /// AutoMapper with multitype properties.Support for Use Action to custom special fields.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TSource1"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <typeparam name="TSource3"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <param name="source3"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TValue AutoMapper<TValue, TSource1, TSource2, TSource3>(TSource1 source1, TSource2 source2, TSource3 source3, Action<TValue> action) where TValue : class where TSource1 : class where TSource2 : class where TSource3 : class
        {
            TValue value = Activator.CreateInstance<TValue>();
            PropertyInfo[] propertyInfos = typeof(TValue).GetProperties();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            value = SetValue(value, propertyInfos, Initdic(source1), keys);
            value = SetValue(value, propertyInfos, Initdic(source2), keys);
            value = SetValue(value, propertyInfos, Initdic(source3), keys);
            action(value);
            return value;
        }
        /// <summary>
        /// AutoMapper with multitype properties.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TSource1"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <typeparam name="TSource3"></typeparam>
        /// <typeparam name="TSource4"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <param name="source3"></param>
        /// <param name="source4"></param>
        /// <returns></returns>
        public static TValue AutoMapper<TValue, TSource1, TSource2, TSource3, TSource4>(TSource1 source1, TSource2 source2, TSource3 source3, TSource4 source4) where TValue : class where TSource1 : class where TSource2 : class where TSource3 : class where TSource4 : class
        {
            TValue value = Activator.CreateInstance<TValue>();
            PropertyInfo[] propertyInfos = typeof(TValue).GetProperties();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            value = SetValue(value, propertyInfos, Initdic(source1), keys);
            value = SetValue(value, propertyInfos, Initdic(source2), keys);
            value = SetValue(value, propertyInfos, Initdic(source3), keys);
            value = SetValue(value, propertyInfos, Initdic(source4), keys);
            return value;
        }
        /// <summary>
        /// AutoMapper with multitype properties.Support for Use Action to custom special fields.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TSource1"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <typeparam name="TSource3"></typeparam>
        /// <typeparam name="TSource4"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <param name="source3"></param>
        /// <param name="source4"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TValue AutoMapper<TValue, TSource1, TSource2, TSource3, TSource4>(TSource1 source1, TSource2 source2, TSource3 source3, TSource4 source4, Action<TValue> action) where TValue : class where TSource1 : class where TSource2 : class where TSource3 : class where TSource4 : class
        {
            TValue value = Activator.CreateInstance<TValue>();
            PropertyInfo[] propertyInfos = typeof(TValue).GetProperties();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            value = SetValue(value, propertyInfos, Initdic(source1), keys);
            value = SetValue(value, propertyInfos, Initdic(source2), keys);
            value = SetValue(value, propertyInfos, Initdic(source3), keys);
            value = SetValue(value, propertyInfos, Initdic(source4), keys);
            action(value);
            return value;
        }
        /// <summary>
        /// AutoMapper with multitype properties.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TSource1"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <typeparam name="TSource3"></typeparam>
        /// <typeparam name="TSource4"></typeparam>
        /// <typeparam name="TSource5"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <param name="source3"></param>
        /// <param name="source4"></param>
        /// <param name="source5"></param>
        /// <returns></returns>
        public static TValue AutoMapper<TValue, TSource1, TSource2, TSource3, TSource4, TSource5>(TSource1 source1, TSource2 source2, TSource3 source3, TSource4 source4, TSource5 source5) where TValue : class where TSource1 : class where TSource2 : class where TSource3 : class where TSource4 : class where TSource5 : class
        {
            TValue value = Activator.CreateInstance<TValue>();
            PropertyInfo[] propertyInfos = typeof(TValue).GetProperties();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            value = SetValue(value, propertyInfos, Initdic(source1), keys);
            value = SetValue(value, propertyInfos, Initdic(source2), keys);
            value = SetValue(value, propertyInfos, Initdic(source3), keys);
            value = SetValue(value, propertyInfos, Initdic(source4), keys);
            value = SetValue(value, propertyInfos, Initdic(source5), keys);
            return value;
        }
        /// <summary>
        /// AutoMapper with multitype properties.Support for Use Action to custom special fields.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TSource1"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <typeparam name="TSource3"></typeparam>
        /// <typeparam name="TSource4"></typeparam>
        /// <typeparam name="TSource5"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <param name="source3"></param>
        /// <param name="source4"></param>
        /// <param name="source5"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TValue AutoMapper<TValue, TSource1, TSource2, TSource3, TSource4, TSource5>(TSource1 source1, TSource2 source2, TSource3 source3, TSource4 source4, TSource5 source5, Action<TValue> action) where TValue : class where TSource1 : class where TSource2 : class where TSource3 : class where TSource4 : class where TSource5 : class
        {
            TValue value = Activator.CreateInstance<TValue>();
            PropertyInfo[] propertyInfos = typeof(TValue).GetProperties();
            Dictionary<string, string> keys = new Dictionary<string, string>();
            value = SetValue(value, propertyInfos, Initdic(source1), keys);
            value = SetValue(value, propertyInfos, Initdic(source2), keys);
            value = SetValue(value, propertyInfos, Initdic(source3), keys);
            value = SetValue(value, propertyInfos, Initdic(source4), keys);
            value = SetValue(value, propertyInfos, Initdic(source5), keys);
            action(value);
            return value;
        }
    }
}
