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

namespace SevenTiny.Bantina.AutoMapper
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = true)]
    public class MapperClassAttribute : Attribute
    {
        public string Name { get; set; }
        public static string GetName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(MapperClassAttribute), true).FirstOrDefault();
            return attr != null ? (attr as MapperClassAttribute).Name ?? default(string) : default(string);
        }
    }
}
