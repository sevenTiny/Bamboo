/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 12/10/2019, 20:27:28 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 
* Thx , Best Regards ~
*********************************************************/
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Helpers;
using System;

namespace SevenTiny.Bantina.Bankinate.Caching
{
    /// <summary>
    /// 缓存管理器扩展
    /// </summary>
    public static class DbCacheManagerExtensions
    {
        /// <summary>
        /// 初始化本地缓存
        /// </summary>
        /// <param name="dbContext">数据操作上下文</param>
        /// <param name="openQueryCache">是否开启一级缓存</param>
        /// <param name="openTableCache">是否开启二级缓存</param>
        /// <param name="queryCacheExpiredTimeSpan">一级缓存过期时间</param>
        /// <param name="tableCacheExpiredTimeSpan">二级缓存过期时间</param>
        public static void OpenLocalCache(this DbContext dbContext, bool openQueryCache = false, bool openTableCache = false, TimeSpan queryCacheExpiredTimeSpan = default(TimeSpan), TimeSpan tableCacheExpiredTimeSpan = default(TimeSpan))
        {
            dbContext.OpenCache(new DbCacheManager(dbContext,
                new CacheOptions()
                {
                    OpenQueryCache = openQueryCache,
                    OpenTableCache = openTableCache,
                    QueryCacheExpiredTimeSpan = queryCacheExpiredTimeSpan,
                    TableCacheExpiredTimeSpan = tableCacheExpiredTimeSpan
                }));
        }

        /// <summary>
        /// 初始化Redis缓存
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="openQueryCache"></param>
        /// <param name="openTableCache"></param>
        /// <param name="cacheServer"></param>
        public static void OpenRedisCache(this DbContext dbContext, string cacheServer, bool openQueryCache = false, bool openTableCache = false)
        {
            Ensure.ArgumentNotNullOrEmpty(cacheServer, nameof(cacheServer));

            dbContext.OpenCache(new DbCacheManager(dbContext,
                new CacheOptions()
                {
                    OpenQueryCache = openQueryCache,
                    OpenTableCache = openTableCache,
                    CacheMediaType = CacheMediaType.Redis,
                    CacheMediaServer = cacheServer
                }));
        }
    }
}
