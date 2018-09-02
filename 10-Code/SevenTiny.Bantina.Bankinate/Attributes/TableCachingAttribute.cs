using SevenTiny.Bantina.Bankinate.Configs;
using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableCachingAttribute : Attribute
    {
        public TimeSpan ExpiredTime { get; private set; }
        public TableCachingAttribute() { }
        public TableCachingAttribute(TimeSpan expiredTime)
        {
            ExpiredTime = expiredTime;
        }

        public static bool IsExistTaleCaching(Type type, out TimeSpan timeSpan)
        {
            var attr = type.GetCustomAttributes(typeof(TableCachingAttribute), true)?.FirstOrDefault();
            timeSpan = (attr as TableCachingAttribute)?.ExpiredTime ?? TimeSpan.Zero;//这里默认给Zero，在TableCache里面判断Zero则获取Context的默认值
            if (attr == null)
            {
                return false;
            }
            return true;
        }
    }
}
