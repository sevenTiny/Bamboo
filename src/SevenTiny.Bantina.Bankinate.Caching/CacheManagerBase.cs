using SevenTiny.Bantina.Bankinate.DbContexts;

namespace SevenTiny.Bantina.Bankinate.Caching
{
    /// <summary>
    /// 缓存管理器基类
    /// </summary>
    public abstract class CacheManagerBase
    {
        protected CacheManagerBase(DbContext context, CacheOptions cacheOptions)
        {
            DbContext = context;
            CacheOptions = cacheOptions;
            CacheStorageManager = new CacheStorageManager(cacheOptions);
        }

        protected DbContext DbContext { get; set; }
        internal CacheStorageManager CacheStorageManager { get; set; }
        protected CacheOptions CacheOptions { get; private set; }
    }
}
