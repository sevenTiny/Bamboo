using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DataBaseAttribute : Attribute
    {
        public DataBaseAttribute(string databaseName)
        {
            this.Name = databaseName;
        }
        public string Name { get; set; }

        public static string GetName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(DataBaseAttribute), true).FirstOrDefault();
            return attr != null ? (attr as DataBaseAttribute).Name ?? default(string) : default(string);
        }
    }
}
