/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-19 23:58:08
 * Modify: 2018-04-19 23:58:08
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute() { }
        public ColumnAttribute(string columnName)
        {
            this.Name = columnName;
        }

        public string Name { get; private set; }

        public string GetName(string @default) => this.Name ?? @default;

        public static string GetName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(ColumnAttribute), true)?.FirstOrDefault();
            return (attr as ColumnAttribute)?.Name ?? type.Name;
        }
    }
}
