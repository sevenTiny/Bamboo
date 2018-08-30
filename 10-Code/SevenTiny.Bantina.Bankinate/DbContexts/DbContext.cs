using SevenTiny.Bantina.Bankinate.Cache;
using SevenTiny.Bantina.Bankinate.Configs;
using System;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class DbContext
    {
        public DbContext(DataBaseType dataBaseType)
        {
            MCache.Instance.ExpiredTimeSpan = DefalutCacheExpiredTimeSpan;
        }

        public DataBaseType DataBaseType { get; private set; }
        public string SqlStatement { get; internal set; }
        public string TableName { get; internal set; }

        //Cache Control
        /// <summary>
        /// 一级缓存
        /// 查询条件级别的缓存（filter），可以暂时缓存根据查询条件查询到的数据
        /// 如果开启二级缓存，且当前操作对应的表已经在二级缓存里，则不进行条件缓存
        /// </summary>
        public bool QueryCache { get; protected set; } = false;
        /// <summary>
        /// 二级缓存
        /// 配置表缓存标签对整张数据库表进行缓存
        /// </summary>
        public bool TableCache { get; protected set; } = false;
        public TimeSpan DefalutCacheExpiredTimeSpan { get; protected set; } = DefaultValue.CacheExpiredTime;
        public bool IsFromCache { get; internal set; } = false;
        /// <summary>
        /// Cache 存储媒介,默认本地缓存
        /// </summary>
        public CacheMediaType CacheMediaType { get; protected set; } = DefaultValue.CacheMediaType;
        /// <summary>
        /// Cache 第三方存储媒介服务地址
        /// </summary>
        public string CacheMediaServer { get; protected set; }
    }
}
