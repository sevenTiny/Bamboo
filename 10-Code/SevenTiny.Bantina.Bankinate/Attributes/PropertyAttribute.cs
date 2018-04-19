using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PropertyAttribute : Attribute
    {
        public PropertyAttribute(string propertyName)
        {
            this.Name = propertyName;
        }

        public string Name { get; set; }

        public static string GetName(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(PropertyAttribute), true).FirstOrDefault();
            return attr != null ? (attr as PropertyAttribute).Name ?? default(string) : default(string);
        }
    }
}
