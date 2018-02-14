using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace SevenTiny.Configuration
{
    public static class MySqlDataReaderExtention
    {
        public static IEnumerable<T> ToEnumerable<T>(this MySqlDataReader reader) where T : class
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
                        if (reader[propertyInfo.Name] != System.DBNull.Value)
                        {
                            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                                propertyInfo.SetValue(model, Convert.ChangeType(reader[propertyInfo.Name], new NullableConverter(propertyInfo.PropertyType).UnderlyingType), null);
                            else
                                propertyInfo.SetValue(model, Convert.ChangeType(reader[propertyInfo.Name], propertyInfo.PropertyType), null);
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
    }
}
