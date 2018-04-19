using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute()
        { }
        public ColumnAttribute(string columnName)
        {
            this.Name = columnName;
        }

        public string Name { get; set; }

        public string GetName(string @default)
        {
            return this.Name ?? @default;
        }

        public static string GetName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();
            return attr != null ? (attr as ColumnAttribute).Name ?? type.Name : type.Name;
        }
    }
}
