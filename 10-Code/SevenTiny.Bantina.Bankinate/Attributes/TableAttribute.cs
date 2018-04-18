using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string tableName)
        {
            this.Name = tableName;
        }
        public string Name { get; set; }

        public static string GetName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
            return attr != null ? (attr as TableAttribute).Name ?? default(string) : default(string);
        }
    }
}
