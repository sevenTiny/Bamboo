/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-14 20:00:37
 * Modify: 2018-02-14 20:00:37
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;
using System.Linq;
using System.Reflection;

namespace SevenTiny.Bantina.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    public class ConfigPropertyAttribute : Attribute
    {
        public string Name { get; set; }

        public static string GetName(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes(typeof(ConfigPropertyAttribute), true).FirstOrDefault();
            if (attr != null)
            {
                string name = (attr as ConfigPropertyAttribute).Name;
                if (!string.IsNullOrEmpty(name))
                {
                    return name;
                }
            }
            return default(string);
        }
    }
}
