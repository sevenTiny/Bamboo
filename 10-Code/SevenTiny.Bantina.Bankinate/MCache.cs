/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-20 17:05:42
 * Modify: 2018-04-20 17:05:42
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using cache = SevenTiny.Bantina.Bankinate.MemoryCacheHelper;

namespace SevenTiny.Bantina.Bankinate
{
    /**
     * The cache memory1,quick memory value from sql statement.
     * */
    internal class MCache
    {
        //cache table modify mark,it will be cached when add/update/delete
        public const string MCTable = "BankinateCache_Table_";
        //cache level 2 key prefix
        public const string MC2 = "BankinateCache_MC2_";
        //cache time
        public static TimeSpan ExpiredTime => TimeSpan.FromDays(1);

        private static readonly object locker = new object();

        public static void MarkTableModify(string tableName)
        {
            //no expired
            cache.Put($"{MCTable}{tableName}", 1,DateTime.MaxValue);
        }

        public static TEntity GetInCacheIfNotExistReStoreEntity<TEntity>(bool localCache, string tableName, string sqlstatement, Expression<Func<TEntity, bool>> filter, Func<TEntity> func) where TEntity : class
        {
            if (localCache)
            {
                TEntity result;
                //check if table data has be changed
                string mcTableKey = $"{MCTable}{tableName}";
                int sqlstatementKey = sqlstatement.GetHashCode();

                lock (locker)
                {
                    if (cache.Exist(mcTableKey))
                    {
                        result = cache.Put(sqlstatementKey, func());
                        cache.Delete(mcTableKey);
                    }
                    else
                    {
                        //1.if sqlstatement key exist,quick return!
                        if (cache.Exist(sqlstatementKey))
                        {
                            return result = cache.Get<object, TEntity>(sqlstatementKey);
                        }
                        //2.if sqlstatement key not exist,search in table cache.
                        if (filter != null)
                        {
                            List<TEntity> list = GetTableListInCache<TEntity>(tableName);
                            if (list != null && list.Any())
                            {
                                return result = list.Where(filter.Compile()).ToList().FirstOrDefault();
                            }
                        }
                        //3.search in db,but start a thread to cache table at the sametime
                        Task.Run(() =>
                        {
                            MCache.PutTableListIntoCache(tableName, DbHelper.ExecuteList<TEntity>($"SELECT * FROM {tableName}"));
                        });

                        return result = cache.Put(sqlstatementKey, func(), ExpiredTime);
                    }
                }
                return result;
            }
            return func();
        }

        public static List<TEntity> GetInCacheIfNotExistReStoreEntities<TEntity>(bool localCache, string tableName, string sqlstatement, Expression<Func<TEntity, bool>> filter, Func<List<TEntity>> func) where TEntity : class
        {
            if (localCache)
            {
                List<TEntity> result;
                //check if table data has be changed
                string mcTableKey = $"{MCTable}{tableName}";
                int sqlstatementKey = sqlstatement.GetHashCode();

                lock (locker)
                {
                    if (cache.Exist(mcTableKey))
                    {
                        result = cache.Put(sqlstatementKey, func(), ExpiredTime);
                        cache.Delete(mcTableKey);
                    }
                    else
                    {
                        //1.if sqlstatement key exist,quick return!
                        if (cache.Exist(sqlstatementKey))
                        {
                            return result = cache.Get<object, List<TEntity>>(sqlstatementKey);
                        }
                        //2.if sqlstatement key not exist,search in table cache.

                        List<TEntity> list = GetTableListInCache<TEntity>(tableName);
                        if (filter != null)
                        {
                            if (list != null && list.Any())
                            {
                                return result = list.Where(filter.Compile()).ToList();
                            }
                        }
                        //3.search in db,but start a thread to cache table at the sametime
                        Task.Run(() =>
                        {
                            MCache.PutTableListIntoCache(tableName, DbHelper.ExecuteList<TEntity>($"SELECT * FROM {tableName}"));
                        });
                        return result = cache.Put(sqlstatementKey, func(), ExpiredTime);
                    }
                }
                return result;
            }
            return func();
        }

        private static List<TEntity> GetTableListInCache<TEntity>(string tableName) where TEntity : class
        {
            string key = $"{MC2}{tableName}";
            return cache.Get<object, List<TEntity>>(key);
        }
        private static void PutTableListIntoCache<TEntity>(string tableName, List<TEntity> listOfTable) where TEntity : class
        {
            string key = $"{MC2}{tableName}";
            cache.Put(key, listOfTable, ExpiredTime);
        }
    }
}