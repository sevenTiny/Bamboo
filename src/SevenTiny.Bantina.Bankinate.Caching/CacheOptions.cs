using System;

namespace SevenTiny.Bantina.Bankinate.Caching
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public class CacheOptions
    {
        /// <summary>
        /// 一级缓存
        /// 查询条件级别的缓存（filter），可以暂时缓存根据查询条件查询到的数据
        /// 如果开启二级缓存，且当前操作对应的表已经在二级缓存里，则不进行条件缓存
        /// </summary>
        public bool OpenQueryCache { get; set; } = false;
        /// <summary>
        /// 二级缓存
        /// 配置表缓存标签对整张数据库表进行缓存
        /// </summary>
        public bool OpenTableCache { get; set; } = false;
        /// <summary>
        /// 查询缓存的默认缓存时间
        /// </summary>
        private TimeSpan _QueryCacheExpiredTimeSpan = CachingConst.QueryCacheExpiredTimeSpan;
        public TimeSpan QueryCacheExpiredTimeSpan
        {
            get { return _QueryCacheExpiredTimeSpan; }
            set
            {
                if (value > MaxExpiredTimeSpan)
                {
                    MaxExpiredTimeSpan = value;
                }
                else if (value == default)
                {
                    value = CachingConst.QueryCacheExpiredTimeSpan;
                }
                _QueryCacheExpiredTimeSpan = value;
            }
        }
        /// <summary>
        /// 表缓存的缓存时间
        /// </summary>
        private TimeSpan _TableCacheExpiredTimeSpan = CachingConst.TableCacheExpiredTimeSpan;
        public TimeSpan TableCacheExpiredTimeSpan
        {
            get { return _TableCacheExpiredTimeSpan; }
            set
            {
                if (value > MaxExpiredTimeSpan)
                {
                    MaxExpiredTimeSpan = value;
                }
                else if (value == default)
                {
                    value = CachingConst.TableCacheExpiredTimeSpan;
                }
                _TableCacheExpiredTimeSpan = value;
            }
        }
        /// <summary>
        /// 每张表一级缓存的最大个数，超出数目将会按从早到晚的顺序移除缓存键
        /// </summary>
        public int QueryCacheMaxCountPerTable { get; set; } = CachingConst.QueryCacheMaxCountPerTable;
        /// <summary>
        /// Cache 存储媒介,默认本地缓存
        /// </summary>
        public CacheMediaType CacheMediaType { get; set; } = CacheMediaType.Local;
        /// <summary>
        /// Cache 第三方存储媒介服务地址
        /// </summary>
        public string CacheMediaServer { get; set; }
        /// <summary>
        /// 最大的缓存时间（用于缓存缓存键）
        /// </summary>
        internal TimeSpan MaxExpiredTimeSpan { get; set; } = CachingConst.CacheKeysMaxExpiredTime;
    }
}
