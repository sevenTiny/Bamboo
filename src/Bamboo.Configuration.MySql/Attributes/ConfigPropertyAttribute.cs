/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-14 20:00:37
 * Modify: 2018-02-14 20:00:37
 * E-mail: dong@7tiny.com | Bamboo@foxmail.com 
 * GitHub: https://github.com/Bamboo 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Reflection;

namespace Bamboo.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ConfigPropertyAttribute : ColumnAttribute
    {
        public ConfigPropertyAttribute() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="persistenceName">持久化对象对应的字段名</param>
        public ConfigPropertyAttribute(string persistenceName) : base(persistenceName) { }

        internal static string GetName(PropertyInfo property)
        {
            if (property.GetCustomAttribute(typeof(ConfigPropertyAttribute)) is ConfigPropertyAttribute configPropertyAttribute)
                return configPropertyAttribute.Name;
            return property.Name;
        }
    }
}
