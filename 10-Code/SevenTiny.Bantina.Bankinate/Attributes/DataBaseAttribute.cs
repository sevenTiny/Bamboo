using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DataBaseAttribute : Attribute
    {
        public DataBaseAttribute()
        { }
        public DataBaseAttribute(string databaseName)
        {
            this.Name = databaseName;
        }
        public string Name { get; set; }

        public string GetName(string @default)
        {
            return this.Name ?? @default;
        }

        public static string GetName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(DataBaseAttribute), true).FirstOrDefault();
            return attr != null ? (attr as DataBaseAttribute).Name ?? type.Name : type.Name;
        }
    }
}
