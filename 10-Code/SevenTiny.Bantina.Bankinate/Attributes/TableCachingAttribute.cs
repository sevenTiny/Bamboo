using SevenTiny.Bantina.Bankinate.Configs;
using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TableCachingAttribute : Attribute
    {
        public TimeSpan ExpiredTime { get; private set; } = DefaultValue.CacheExpiredTime;
        public TableCachingAttribute(TimeSpan expiredTime)
        {
            ExpiredTime = expiredTime;
        }
        public static TimeSpan GetExpiredTime(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();
            return (attr as TableCachingAttribute)?.ExpiredTime ?? DefaultValue.CacheExpiredTime;
        }

    }
}
