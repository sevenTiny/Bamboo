using SevenTiny.Bantina.Bankinate.Caching.Helpers;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate.Caching
{
    /// <summary>
    /// 查询缓存管理器（一级缓存管理器）
    ///QueryCache存储结构，以表为缓存单位，便于在对单个表进行操作以后释放单个表的缓存，每个表的缓存以hash字典的方式存储
    ///Key(表相关Key)
    ///Dictionary<int, T>
    ///{
    ///     sql.HashCode(),值
    ///}
    /// </summary>
    internal class QueryCacheManager : CacheManagerBase
    {
        internal QueryCacheManager(DbContext context, CacheOptions cacheOptions) : base(context, cacheOptions) { }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void FlushAllCache()
        {
            if (CacheStorageManager.IsExist(CachingConst.GetQueryCacheKeysCacheKey(DbContext.DataBaseName), out HashSet<string> keys))
            {
                foreach (var item in keys)
                {
                    CacheStorageManager.Delete(item);
                }
            }
        }

        /// <summary>
        /// 清空单个表相关的所有缓存
        /// </summary>
        /// <param name="dbContext"></param>
        public void FlushCollectionCache(string collectionName = null)
        {
            CacheStorageManager.Delete(GetQueryCacheKey(collectionName));
        }

        /// <summary>
        /// 构建sql查询缓存的总key
        /// </summary>
        /// <returns></returns>
        private string GetQueryCacheKey(string collectionName = null)
        {
            string key = $"{CachingConst.CacheKey_QueryCache}{collectionName ?? DbContext.CollectionName}";

            //缓存键更新
            if (!CacheStorageManager.IsExist(CachingConst.GetQueryCacheKeysCacheKey(DbContext.DataBaseName), out HashSet<string> keys))
                keys = new HashSet<string>();

            keys.Add(key);

            CacheStorageManager.Put(CachingConst.GetQueryCacheKeysCacheKey(DbContext.DataBaseName), keys, CacheOptions.MaxExpiredTimeSpan);

            return key;
        }

        /// <summary>
        /// 构建sql查询的sql语句缓存键Key
        /// </summary>
        /// <returns></returns>
        private string GetSqlQueryCacheKey() => DbContext.GetQueryCacheKey();

        /// <summary>
        /// 从缓存中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DbContext"></param>
        /// <returns></returns>
        internal T GetEntitiesFromCache<T>()
        {
            //如果QueryCache里面有该缓存键，则直接获取，并从单个表单位中获取到对应sql的值
            if (CacheStorageManager.IsExist(GetQueryCacheKey(), out Dictionary<string, object> t))
            {
                string sqlQueryCacheKey = GetSqlQueryCacheKey();
                if (t.ContainsKey(sqlQueryCacheKey))
                {
                    DbContext.IsFromCache = true;
                    return TypeConvertHelper.ToGenericType<T>(t[sqlQueryCacheKey]);
                }
            }

            return default(T);
        }

        /// <summary>
        /// QueryCache级别存储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheValue"></param>
        internal void CacheData<T>(T cacheValue)
        {
            string sqlQueryCacheKey = GetSqlQueryCacheKey();
            string queryCacheKey = GetQueryCacheKey();
            //如果缓存中存在，则拿到表单位的缓存并更新
            //这里用object类型进行存储，因为字典的value可能有list集合，int，object等多种类型，泛型使用会出现识别异常
            if (CacheStorageManager.IsExist(queryCacheKey, out Dictionary<string, object> t))
            {
                //如果超出单表的query缓存键阈值，则按先后顺序进行移除
                if (t.Count >= CacheOptions.QueryCacheMaxCountPerTable)
                    t.Remove(t.First().Key);

                t.AddOrUpdate(sqlQueryCacheKey, cacheValue);
                CacheStorageManager.Put(queryCacheKey, t, CacheOptions.QueryCacheExpiredTimeSpan);
            }
            //如果缓存中没有表单位的缓存，则直接新增表单位的sql键缓存
            else
            {
                var dic = new Dictionary<string, object> { { sqlQueryCacheKey, cacheValue } };
                CacheStorageManager.Put(queryCacheKey, dic, CacheOptions.QueryCacheExpiredTimeSpan);
            }
        }
    }
}
