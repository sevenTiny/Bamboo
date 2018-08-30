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
        /// 默认缓存存储媒介
        /// </summary>
        internal static readonly CacheMediaType CacheMediaType = CacheMediaType.Local;
    }
}
