using SevenTiny.Bantina.Bankinate.Cache;
using System;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class DbContext
    {
        public DbContext(DataBaseType dataBaseType)
        {
            MCache.Instance.ExpiredTimeSpan = CacheExpiredTimeSpan;
        }

        public DataBaseType DataBaseType { get; private set; }
        public string SqlStatement { get; internal set; }
        public string TableName { get; internal set; }

        //待细化
        public bool LocalCache { get; set; } = false;

        public TimeSpan CacheExpiredTimeSpan { get; protected set; } = TimeSpan.FromDays(1);
        public bool IsFromCache { get;internal set; } = false;
    }
}
