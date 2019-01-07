using MongoDB.Driver;
using SevenTiny.Bantina.Bankinate.Cache;
using SevenTiny.Bantina.Bankinate.Configs;
using SevenTiny.Bantina.Bankinate.DataAccessEngine;
using System;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class DbContext
    {
        public DbContext(DataBaseType dataBaseType)
        {
            DataBaseType = dataBaseType;
        }

        public DataBaseType DataBaseType { get; private set; }
        public string DataBaseName { get; protected set; }
        public string TableName { get; internal set; }
        public string SqlStatement { get; internal set; }

        //Cache Control
        /// <summary>
        /// 一级缓存
        /// 查询条件级别的缓存（filter），可以暂时缓存根据查询条件查询到的数据
        /// 如果开启二级缓存，且当前操作对应的表已经在二级缓存里，则不进行条件缓存
        /// </summary>
        public bool OpenQueryCache { get; protected set; } = false;
        /// <summary>
        /// 二级缓存
        /// 配置表缓存标签对整张数据库表进行缓存
        /// </summary>
        public bool OpenTableCache { get; protected set; } = false;
        /// <summary>
        /// 查询缓存的默认缓存时间
        /// </summary>
        private TimeSpan _QueryCacheExpiredTimeSpan = DefaultValue.QueryCacheExpiredTimeSpan;
        public TimeSpan QueryCacheExpiredTimeSpan
        {
            get { return _QueryCacheExpiredTimeSpan; }
            protected set
            {
                if (value > MaxExpiredTimeSpan)
                {
                    MaxExpiredTimeSpan = value;
                }
                _QueryCacheExpiredTimeSpan = value;
            }
        }
        /// <summary>
        /// 表缓存的缓存时间
        /// </summary>
        private TimeSpan _TableCacheExpiredTimeSpan = DefaultValue.TableCacheExpiredTimeSpan;
        public TimeSpan TableCacheExpiredTimeSpan
        {
            get { return _TableCacheExpiredTimeSpan; }
            protected set
            {
                if (value > MaxExpiredTimeSpan)
                {
                    MaxExpiredTimeSpan = value;
                }
                _TableCacheExpiredTimeSpan = value;
            }
        }
        /// <summary>
        /// 数据是否从缓存中获取
        /// </summary>
        public bool IsFromCache { get; internal set; } = false;
        /// <summary>
        /// Cache 存储媒介,默认本地缓存
        /// </summary>
        public CacheMediaType CacheMediaType { get; protected set; } = DefaultValue.CacheMediaType;
        /// <summary>
        /// Cache 第三方存储媒介服务地址
        /// </summary>
        public string CacheMediaServer { get; protected set; }

        /// <summary>
        /// 最大的缓存时间（用于缓存缓存键）
        /// </summary>
        internal TimeSpan MaxExpiredTimeSpan { get; set; } = DefaultValue.CacheKeysMaxExpiredTime;

        /// <summary>
        /// 清空全部缓存
        /// </summary>
        public void FlushAllCache()=> DbCacheManager.FlushAllCache(this);
        /// <summary>
        /// 清空一级缓存
        /// </summary>
        public void FlushQueryCache()=> QueryCacheManager.FlushAllCache(this);
        /// <summary>
        /// 清空二级缓存
        /// </summary>
        public void FlushTableCache()=> TableCacheManager.FlushAllCache(this);
        /// <summary>
        /// NoSql的文档集合
        /// </summary>
        internal dynamic NoSqlCollection { get; set; }
    }
}
