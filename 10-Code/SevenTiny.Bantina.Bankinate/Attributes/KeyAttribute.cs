using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KeyAttribute : Attribute
    {
        public KeyAttribute(string propertyName)
        {
            this.Name = propertyName;
        }

        public string Name { get; set; }

        public static string GetName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault();
            return attr != null ? (attr as KeyAttribute).Name ?? default(string) : default(string);
        }
    }
}
