using System;
using System.Reflection;

namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    /// <summary>
    /// ServiceAttribute和Aop目前是互斥的，如果使用Aop Factory方式注入的实例，则使用SpringContext获取实例
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ServiceAttribute : Attribute
    {
        public static bool Exist(FieldInfo field)
        {
            return field.GetCustomAttribute(typeof(ServiceAttribute), false) != null;
        }
    }
}
