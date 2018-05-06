/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-14 20:00:43
 * Modify: 2018-02-14 20:00:43
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
    [AttributeUsage(AttributeTargets.Class, Inherited = true,AllowMultiple =false)]
    public class ConfigNameAttribute : Attribute
    {
        public string Name { get; set; }

        public ConfigNameAttribute()
        {
        }

        public ConfigNameAttribute(string name)
        {
            this.Name = name;
        }

        public static string GetName(Type type)
        {
            if (type.GetCustomAttribute(typeof(ConfigNameAttribute), true) is ConfigNameAttribute attr)
            {
                return attr?.Name ?? type.Name;
            }
            return type.Name;
        }
    }
}
