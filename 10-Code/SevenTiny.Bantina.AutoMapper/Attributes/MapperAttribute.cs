/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-03 13:28:38
 * Modify: 2018-04-03 13:28:38
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

namespace SevenTiny.Bantina.AutoMapper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    public class MapperAttribute : Attribute
    {
        public string Name { get; set; }
        public static string GetName(PropertyInfo property)
        {
            var attr = property.GetCustomAttributes(typeof(MapperAttribute), true).FirstOrDefault();
            return attr != null ? (attr as MapperAttribute).Name ?? default(string) : default(string);
        }
    }
}
