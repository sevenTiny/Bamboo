using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace SevenTiny.Bantina.Bankinate.Caching.Helpers
{
    internal class TypeConvertHelper
    {
        /// <summary>
        /// 弱类型转泛型类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToGenericType<T>(object value)
        {
            if (value == null)
                return default(T);

            Type type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                NullableConverter nullableConverter = new NullableConverter(type);
                //将convertsionType转换为nullable对的基础基元类型
                return (T)Convert.ChangeType(value, nullableConverter.UnderlyingType);
            }
            else if (type.IsClass)
            {
                //复杂类型Json反序列化不彻底，需要二次处理
                if (value is JObject)
                {
                    return (value as JObject).ToObject<T>();
                }
                else if (value is JArray)
                {
                    return (value as JArray).ToObject<T>();
                }
                return (T)value;
            }
            else
            {
                return (T)Convert.ChangeType(value, type);
            }
        }
    }
}
