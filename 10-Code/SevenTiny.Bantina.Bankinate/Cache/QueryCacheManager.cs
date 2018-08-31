using SevenTiny.Bantina.Bankinate.Configs;
using SevenTiny.Bantina.Bankinate.DbContexts;
using System.Collections.Generic;

namespace SevenTiny.Bantina.Bankinate.Cache
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
    internal abstract class QueryCacheManager
    {
        /// <summary>
        /// 清空所有缓存
        /// </summary>
        internal static void FlushAllCache(DbContext dbContext)
        {
            if (CacheStorageManager.IsExist(dbContext, DefaultValue.GetQueryCacheKeysCacheKey(dbContext.DataBaseName), out HashSet<string> keys))
            {
                foreach (var item in keys)
                {
                    CacheStorageManager.Delete(dbContext, item);
                }
            }
        }

        /// <summary>
        /// 清空单个表相关的所有缓存
        /// </summary>
        /// <param name="dbContext"></param>
        internal static void FlushTableCache(DbContext dbContext)
        {
            CacheStorageManager.Delete(dbContext, GetQueryCacheKey(dbContext));
        }

        private static string GetQueryCacheKey(DbContext dbContext)
        {
            string key = $"{DefaultValue.CacheKey_QueryCache}{dbContext.TableName}";
            //缓存键更新
            if (!CacheStorageManager.IsExist(dbContext, DefaultValue.GetQueryCacheKeysCacheKey(dbContext.DataBaseName), out HashSet<string> keys))
            {
                keys = new HashSet<string>();
            }
            keys.Add(key);
            CacheStorageManager.Put(dbContext, DefaultValue.GetQueryCacheKeysCacheKey(dbContext.DataBaseName), keys, dbContext.MaxExpiredTimeSpan);
            return key;
        }

        /// <summary>
        /// 从缓存中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        internal static T GetEntitiesFromCache<T>(DbContext dbContext)
        {
            //1.检查是否开启了Query缓存
            if (dbContext.OpenQueryCache)
            {
                //2.如果QueryCache里面有该缓存键，则直接获取，并从单个表单位中获取到对应sql的值
                if (CacheStorageManager.IsExist(dbContext, GetQueryCacheKey(dbContext), out Dictionary<int, T> t))
                {
                    if (t.ContainsKey(dbContext.SqlStatement.GetHashCode()))
                    {
                        dbContext.IsFromCache = true;
                        return t[dbContext.SqlStatement.GetHashCode()];
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// QueryCache级别存储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="cacheValue"></param>
        internal static void CacheData<T>(DbContext dbContext, T cacheValue)
        {
            if (dbContext.OpenQueryCache)
            {
                if (cacheValue != null)
                {
                    int sqlQueryCacheKey = dbContext.SqlStatement.GetHashCode();
                    //如果缓存中存在，则拿到表单位的缓存并更新
                    if (CacheStorageManager.IsExist(dbContext, GetQueryCacheKey(dbContext), out Dictionary<int, T> t))
                    {
                        if (t.ContainsKey(sqlQueryCacheKey))
                            t[sqlQueryCacheKey] = cacheValue;
                        else
                            t.Add(sqlQueryCacheKey, cacheValue);
                    }
                    //如果缓存中没有表单位的缓存，则直接新增表单位的sql键缓存
                    else
                    {
                        var dic = new Dictionary<int, T>();
                        dic.Add(sqlQueryCacheKey, cacheValue);
                        CacheStorageManager.Put(dbContext, GetQueryCacheKey(dbContext), dic, dbContext.QueryCacheExpiredTimeSpan);
                    }
                }
            }
        }
    }
}
