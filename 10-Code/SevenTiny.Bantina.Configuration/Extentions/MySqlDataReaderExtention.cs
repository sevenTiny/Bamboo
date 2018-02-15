/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-14 23:32:33
 * Modify: 2018-02-14 23:32:33
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace SevenTiny.Bantina.Configuration.Extentions
{
    public static class MySqlDataReaderExtention
    {
        public static IList<T> ToList<T>(this MySqlDataReader reader) where T : class
        {
            Type typeT = typeof(T);
            IList<T> list = new List<T>();
            PropertyInfo[] propertyInfos = typeT.GetProperties();
            using (reader)
            {
                while (reader.Read())
                {
                    T model = System.Activator.CreateInstance<T>();
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        object readerValue = reader[GetPropertyInfoStorageName(propertyInfo)];
                        if (readerValue != System.DBNull.Value)
                        {
                            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                                propertyInfo.SetValue(model, Convert.ChangeType(readerValue, new NullableConverter(propertyInfo.PropertyType).UnderlyingType), null);
                            else
                                propertyInfo.SetValue(model, Convert.ChangeType(readerValue, propertyInfo.PropertyType), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(model, null, null);
                        }
                    }
                    list.Add(model);
                }
            }
            return list;
        }

        /// <summary>
        /// regard as cache
        /// </summary>
        private static Dictionary<string, string> cacheDic = new Dictionary<string, string>();
        /// <summary>
        /// get property name in db,it is declara with config attribute or property info name
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static string GetPropertyInfoStorageName(PropertyInfo propertyInfo)
        {
            //get storage name from static dic
            if (cacheDic.ContainsKey(propertyInfo.Name))
            {
                return cacheDic[propertyInfo.Name];
            }
            //get storage name from config attribute
            var attrName = ConfigPropertyAttribute.GetName(propertyInfo);
            if (!string.IsNullOrEmpty(attrName))
            {
                return cacheDic[propertyInfo.Name] = attrName;
            }
            //get storage name from property info
            return cacheDic[propertyInfo.Name] = propertyInfo.Name;
        }
    }
}
