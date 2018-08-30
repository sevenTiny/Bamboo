using SevenTiny.Bantina.Bankinate.Cache;
using System;

namespace SevenTiny.Bantina.Bankinate.Configs
{
    internal class DefaultValue
    {
        /// <summary>
        /// 默认缓存过期时间
        /// </summary>
        internal static readonly TimeSpan CacheExpiredTime = TimeSpan.FromMinutes(30);
        /// <summary>
        /// QueryCache默认过期时间
        /// </summary>
        internal static readonly TimeSpan QueryCacheExpiredTimeSpan = TimeSpan.Zero;
        /// <summary>
        /// TableCache默认国企时间
        /// </summary>
        internal static readonly TimeSpan TableCacheExpiredTimeSpan = TimeSpan.Zero;
        /// <summary>
        /// 默认缓存存储媒介
        /// </summary>
        internal static readonly CacheMediaType CacheMediaType = CacheMediaType.Local;
        /// <summary>
        /// QueryCache前缀
        /// </summary>
        internal const string CacheKey_QueryCache = "BankinateQueryCache__";
        /// <summary>
        /// TableCache前缀
        /// </summary>
        internal const string CacheKey_TableCache = "BankinateTableCache_";
        /// <summary>
        /// 表扫描key前缀
        /// </summary>
        internal const string CacheKey_TableScanning = "BankinateCacheScaning_";
        /// <summary>
        /// TableCache扫描键最多存在时间
        /// </summary>
        internal static readonly TimeSpan SpanScaningKeyExpiredTime = TimeSpan.FromMinutes(20);

    }
}
