using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotColumnAttribute : Attribute
    {
        public static bool Exist(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(NotColumnAttribute), true).FirstOrDefault();
            return attr != null ? true : false;
        }
    }
}
