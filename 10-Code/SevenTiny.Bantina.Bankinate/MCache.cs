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
using System.Reflection;
using System.Threading.Tasks;
using cache = SevenTiny.Bantina.Bankinate.MemoryCacheHelper;

namespace SevenTiny.Bantina.Bankinate
{
    /**
     * The cache memory1,quick memory value from sql statement.
     * 
     * Dictionary<string, Dictionary<string, object>>
     * cache dictionary,storage cache like this structure
     * {
     *      "MCTable_TableName1":{
     *                              "sqlkey":"data",
     *                              "filterkey":"data",
     *                              "tablekey":"data"
     *                          },
     *      "MCTable_TableName2":{
     *                              "sqlkey":"data",
     *                              "filterkey":"data",
     *                              "tablekey":"data"
     *                          }
     * }
     * */
    internal class MCache
    {
        //cache entry point,also used to check table modify
        public const string MC0 = "BankinateCache_MC0_";
        //cache level 1 key prefix
        public const string MC1 = "BankinateCache_MC1_";
        //cache level 2 key prefix
        public const string MC2 = "BankinateCache_MC2_";
        //cache time
        public static TimeSpan ExpiredTimeSpan { get; set; } = TimeSpan.FromDays(1);

        private static string EnterPoint { get; set; }
        private static string SqlKey { get; set; }
        private static string FilterKey { get; set; }
        private static string TableKey { get; set; }

        private static void SetKeys<TEntity>(string tableName, string sqlstatement, Expression<Func<TEntity, bool>> filter)
        {
            SetEnterPoint(tableName);
            SqlKey = $"{MC1}{sqlstatement?.Trim() ?? string.Empty}";
            FilterKey = $"{MC1}{filter?.ToString()?.Trim() ?? string.Empty}";
            TableKey = $"{MC2}{tableName}";
        }
        private static void SetEnterPoint(string tableName)
        {
            EnterPoint = $"{MC0}{tableName}";
        }

        private static readonly object locker = new object();
        private static readonly object tableQueryLocker = new object();

        //clear all cache about table
        public static void MarkTableModify(string tableName)
        {
            SetEnterPoint(tableName);
            if (cache.Exist(EnterPoint))
            {
                cache.Delete(EnterPoint);
            }
        }
        //if tableKey exist,update tableKey,no clear cache all about table.
        public static void MarkTableModifyAdd<TEntity>(string tableName, TEntity entity) where TEntity : class
        {
            SetEnterPoint(tableName);
            if (cache.Exist(EnterPoint))
            {
                var enterPointDic = cache.Get<string, Dictionary<string, object>>(EnterPoint);
                if (enterPointDic.ContainsKey(TableKey))
                {
                    List<TEntity> list = enterPointDic[TableKey] as List<TEntity>;
                    if (list != null)
                    {
                        list.Add(entity);
                        enterPointDic[TableKey] = list;
                        cache.Put(EnterPoint, enterPointDic);
                    }
                    else
                    {
                        list = new List<TEntity>();
                        list.Add(entity);
                        enterPointDic[TableKey] = list;
                        cache.Put(EnterPoint, enterPointDic);
                    }
                }
                else
                {
                    cache.Delete(EnterPoint);
                }
            }
        }
        //if tableKey exist,update tableKey,no clear cache all about table.
        public static void MarkTableModifyUpdate<TEntity>(string tableName, Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            SetEnterPoint(tableName);
            if (cache.Exist(EnterPoint))
            {
                var enterPointDic = cache.Get<string, Dictionary<string, object>>(EnterPoint);
                if (enterPointDic.ContainsKey(TableKey))
                {
                    List<TEntity> list = enterPointDic[TableKey] as List<TEntity>;
                    if (list != null && list.Any())
                    {
                        var t = list.FirstOrDefault(filter.Compile());
                        if (t != null)
                        {
                            list.Remove(t);
                            //modify property without autocrease key!
                            foreach (var item in typeof(TEntity).GetProperties())
                            {
                                if (item.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                                {
                                    item.SetValue(entity, item.GetValue(t));
                                }
                            }
                            list.Add(entity);
                            enterPointDic[TableKey] = list;
                            cache.Put(EnterPoint, enterPointDic);
                        }
                    }
                }
                else
                {
                    cache.Delete(EnterPoint);
                }
            }
        }
        //if tableKey exist,update tableKey,no clear cache all about table.
        public static void MarkTableModifyDelete<TEntity>(string tableName, Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SetEnterPoint(tableName);
            if (cache.Exist(EnterPoint))
            {
                var enterPointDic = cache.Get<string, Dictionary<string, object>>(EnterPoint);
                if (enterPointDic.ContainsKey(TableKey))
                {
                    List<TEntity> list = enterPointDic[TableKey] as List<TEntity>;
                    if (list != null && list.Any())
                    {
                        var t = list.FirstOrDefault(filter.Compile());
                        if (t != null)
                        {
                            list.Remove(t);
                            enterPointDic[TableKey] = list;
                            cache.Put(EnterPoint, enterPointDic);
                        }
                    }
                }
                else
                {
                    cache.Delete(EnterPoint);
                }
            }
        }

        //get table int a new thread in background
        private static void GetTableBackground<TEntity>(string tableName) where TEntity : class
        {
            //if enter point not exist,get table async
            if (!cache.Exist(EnterPoint))
            {
                Task.Run(() =>
                {
                    lock (tableQueryLocker)
                    {
                        List<TEntity> list = DbHelper.ExecuteList<TEntity>($"SELECT * FROM {tableName}");
                        if (!cache.Exist(EnterPoint))
                        {
                            cache.Put(EnterPoint, new Dictionary<string, object>
                                 {
                                     {TableKey,list }
                                 }, ExpiredTimeSpan);
                        }
                        else
                        {
                            var enterPointDicInCache = cache.Get<string, Dictionary<string, object>>(EnterPoint);
                            if (!enterPointDicInCache.ContainsKey(TableKey))
                            {
                                enterPointDicInCache[TableKey] = list;
                                cache.Put(EnterPoint, enterPointDicInCache, ExpiredTimeSpan);
                            }
                        }
                    }
                });
            }
            else if (!cache.Get<string, Dictionary<string, object>>(EnterPoint).ContainsKey(TableKey))
            {
                //check if has query all table sql
                var enterPointDic = cache.Get<string, Dictionary<string, object>>(EnterPoint);
                if (enterPointDic.ContainsKey($"{MC1}{$"SELECT * FROM {tableName}".Trim()}"))
                {
                    enterPointDic[TableKey] = enterPointDic[SqlKey];
                }
                else
                {
                    Task.Run(() =>
                    {
                        lock (tableQueryLocker)
                        {
                            List<TEntity> list = DbHelper.ExecuteList<TEntity>($"SELECT * FROM {tableName}");
                            //thread safe(query agin)
                            var enterPointDicInCache = cache.Get<string, Dictionary<string, object>>(EnterPoint);
                            if (!enterPointDicInCache.ContainsKey(TableKey))
                            {
                                enterPointDicInCache[TableKey] = list;
                                cache.Put(EnterPoint, enterPointDicInCache, ExpiredTimeSpan);
                            }
                        }
                    });
                }
            }
        }

        #region Get in cache

        public static object GetFromCacheIfNotExistReStoreCount<TEntity>(bool localCache, string tableName, string sqlstatement, Expression<Func<TEntity, bool>> filter, Func<object> func, out bool fromCache) where TEntity : class
        {
            //mark if value from cache ?
            fromCache = false;
            //first call must!
            SetKeys(tableName, sqlstatement, filter);

            if (localCache)
            {
                object result = new object();
                //0
                GetTableBackground<TEntity>(tableName);

                Dictionary<string, object> enterPointDic;

                if (cache.Exist(EnterPoint))
                {
                    //get enterPoint of table
                    enterPointDic = cache.Get<string, Dictionary<string, object>>(EnterPoint);
                    //1.if sqlstatement key exist,quick return!
                    if (!string.IsNullOrEmpty(sqlstatement))
                    {
                        if (enterPointDic.ContainsKey(SqlKey))
                        {
                            fromCache = true;
                            return enterPointDic[SqlKey];
                        }
                    }
                    //1/2.if filter key exist,quick return!
                    if (filter != null)
                    {
                        if (enterPointDic.ContainsKey(FilterKey))
                        {
                            fromCache = true;
                            return enterPointDic[FilterKey];
                        }
                    }
                    //2.if sqlstatement key not exist,search in table cache.
                    if (enterPointDic.ContainsKey(TableKey))
                    {
                        List<TEntity> list = enterPointDic[TableKey] as List<TEntity>;
                        if (list != null && list.Any())
                        {
                            fromCache = true;
                            return list.Where(filter.Compile()).Count();
                        }
                    }
                }
                else
                {
                    enterPointDic = new Dictionary<string, object>();
                }

                //3.search in db,but start a thread to cache table at the sametime
                if (!string.IsNullOrEmpty(sqlstatement))
                {
                    result = func();
                    enterPointDic.Add(SqlKey, result);
                    cache.Put(EnterPoint, enterPointDic, ExpiredTimeSpan);
                    return result;
                }
                if (filter != null)
                {
                    result = func();
                    enterPointDic.Add(FilterKey, result);
                    cache.Put(EnterPoint, enterPointDic, ExpiredTimeSpan);
                    return result;
                }
            }
            return func();
        }

        public static TEntity GetFromCacheIfNotExistReStoreEntity<TEntity>(bool localCache, string tableName, string sqlstatement, Expression<Func<TEntity, bool>> filter, Func<TEntity> func, out bool fromCache) where TEntity : class
        {
            //mark if value from cache ?
            fromCache = false;
            //first call must!
            SetKeys(tableName, sqlstatement, filter);

            if (localCache)
            {
                TEntity result = default(TEntity);

                //0
                GetTableBackground<TEntity>(tableName);

                Dictionary<string, object> enterPointDic;

                if (cache.Exist(EnterPoint))
                {
                    //get enterPoint of table
                    enterPointDic = cache.Get<string, Dictionary<string, object>>(EnterPoint);

                    //1.if sqlstatement key exist,quick return!
                    if (!string.IsNullOrEmpty(sqlstatement))
                    {
                        if (enterPointDic.ContainsKey(SqlKey))
                        {
                            fromCache = true;
                            return enterPointDic[SqlKey] as TEntity;
                        }
                    }
                    //1/2.if filter key exist,quick return!
                    if (filter != null)
                    {
                        if (enterPointDic.ContainsKey(FilterKey))
                        {
                            fromCache = true;
                            return enterPointDic[FilterKey] as TEntity;
                        }
                        //2.if sqlstatement key not exist,search in table cache.
                        if (enterPointDic.ContainsKey(TableKey))
                        {
                            List<TEntity> list = enterPointDic[TableKey] as List<TEntity>;
                            if (list != null && list.Any())
                            {
                                fromCache = true;
                                return list.FirstOrDefault(filter.Compile());
                            }
                        }
                    }
                }

                //3.search in db,but start a thread to cache table at the sametime
                if (!string.IsNullOrEmpty(sqlstatement))
                {
                    result = func();
                    if (cache.Exist(EnterPoint))
                    {
                        enterPointDic = cache.Get<string, Dictionary<string, object>>(EnterPoint);
                    }
                    else
                    {
                        enterPointDic = new Dictionary<string, object>();
                    }
                    enterPointDic.Add(SqlKey, result);
                    cache.Put(EnterPoint, enterPointDic, ExpiredTimeSpan);
                    return result;
                }
                if (filter != null)
                {
                    result = func();
                    if (cache.Exist(EnterPoint))
                    {
                        enterPointDic = cache.Get<string, Dictionary<string, object>>(EnterPoint);
                    }
                    else
                    {
                        enterPointDic = new Dictionary<string, object>();
                    }
                    enterPointDic.Add(FilterKey, result);
                    cache.Put(EnterPoint, enterPointDic, ExpiredTimeSpan);
                    return result;
                }
                return result;
            }
            return func();
        }

        public static List<TEntity> GetFromCacheIfNotExistReStoreEntities<TEntity>(bool localCache, string tableName, string sqlstatement, Expression<Func<TEntity, bool>> filter, Func<List<TEntity>> func, out bool fromCache) where TEntity : class
        {
            //mark if value from cache ?
            fromCache = false;
            //first call must!
            SetKeys(tableName, sqlstatement, filter);

            if (localCache)
            {
                List<TEntity> result = default(List<TEntity>);

                //0
                GetTableBackground<TEntity>(tableName);

                Dictionary<string, object> enterPointDic;

                if (cache.Exist(EnterPoint))
                {
                    //get enterPoint of table
                    enterPointDic = cache.Get<string, Dictionary<string, object>>(EnterPoint);

                    //1.if sqlstatement key exist,quick return!
                    if (!string.IsNullOrEmpty(sqlstatement))
                    {
                        if (enterPointDic.ContainsKey(SqlKey))
                        {
                            fromCache = true;
                            return enterPointDic[SqlKey] as List<TEntity>;
                        }
                    }
                    //1/2.if filter key exist,quick return!
                    if (filter != null)
                    {
                        if (enterPointDic.ContainsKey(FilterKey))
                        {
                            fromCache = true;
                            return enterPointDic[FilterKey] as List<TEntity>;
                        }
                        //2.if sqlstatement key not exist,search in table cache.
                        if (enterPointDic.ContainsKey(TableKey))
                        {
                            List<TEntity> list = enterPointDic[TableKey] as List<TEntity>;
                            if (list != null && list.Any())
                            {
                                fromCache = true;
                                return list.Where(filter.Compile()).ToList();
                            }
                        }
                    }
                    //3.search in db,but start a thread to cache table at the sametime
                    if (!string.IsNullOrEmpty(sqlstatement))
                    {
                        enterPointDic.Add(SqlKey, func());
                        return cache.Put(EnterPoint, enterPointDic, ExpiredTimeSpan) as List<TEntity>;
                    }
                    if (filter != null)
                    {
                        enterPointDic.Add(FilterKey, func());
                        return cache.Put(EnterPoint, enterPointDic, ExpiredTimeSpan) as List<TEntity>;
                    }
                }
                else
                {
                    enterPointDic = new Dictionary<string, object>();
                }

                //3.search in db,but start a thread to cache table at the sametime
                if (!string.IsNullOrEmpty(sqlstatement))
                {
                    result = func();
                    enterPointDic.Add(SqlKey, result);
                    cache.Put(EnterPoint, enterPointDic, ExpiredTimeSpan);
                    return result;
                }
                if (filter != null)
                {
                    result = func();
                    enterPointDic.Add(FilterKey, result);
                    cache.Put(EnterPoint, enterPointDic, ExpiredTimeSpan);
                    return result;
                }
                return result;
            }
            return func();
        }
        public static List<TEntity> GetFromCacheIfNotExistReStoreEntitiesPaging<TEntity>(bool localCache, string tableName, string sqlstatement, Expression<Func<TEntity, bool>> filter, int pageIndex, int pageSize, Expression<Func<TEntity, object>> orderBy, bool isDESC, Func<List<TEntity>> func, out int count, out bool fromCache) where TEntity : class
        {
            count = 0;
            //mark if value from cache ?
            fromCache = false;
            //first call must!
            SetKeys(tableName, sqlstatement, filter);

            if (localCache)
            {
                List<TEntity> result = default(List<TEntity>);

                //0
                GetTableBackground<TEntity>(tableName);

                Dictionary<string, object> enterPointDic;

                if (cache.Exist(EnterPoint))
                {
                    //get enterPoint of table
                    enterPointDic = cache.Get<string, Dictionary<string, object>>(EnterPoint);

                    //1.if sqlstatement key exist,quick return!
                    if (!string.IsNullOrEmpty(sqlstatement))
                    {
                        if (enterPointDic.ContainsKey(SqlKey))
                        {
                            fromCache = true;
                            List<TEntity> list = enterPointDic[SqlKey] as List<TEntity>;
                            count = list.Count;
                            return list;
                        }
                    }
                    //2.if sqlstatement key not exist,search in table cache.
                    if (enterPointDic.ContainsKey(TableKey))
                    {
                        List<TEntity> list = enterPointDic[TableKey] as List<TEntity>;
                        if (list != null && list.Any())
                        {
                            fromCache = true;
                            count = list.Count;
                            //filter
                            if (filter != null && enterPointDic.ContainsKey(FilterKey))
                            {
                                list = list.Where(filter.Compile()).ToList();
                            }
                            //desc orderby
                            if (isDESC)
                            {
                                list = list.OrderByDescending(orderBy.Compile()).ToList();
                            }
                            else
                            {
                                list = list.OrderBy(orderBy.Compile()).ToList();
                            }
                            //page
                            list = list.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                            return list;
                        }
                    }
                    //3.search in db,but start a thread to cache table at the sametime
                    if (!string.IsNullOrEmpty(sqlstatement))
                    {
                        result = func();
                        enterPointDic.Add(SqlKey, result);
                        count = result.Count;
                        return cache.Put(EnterPoint, enterPointDic, ExpiredTimeSpan) as List<TEntity>;
                    }
                }
                else
                {
                    enterPointDic = new Dictionary<string, object>();
                }

                //3.search in db,but start a thread to cache table at the sametime
                if (!string.IsNullOrEmpty(sqlstatement))
                {
                    result = func();
                    count = result.Count;
                    enterPointDic.Add(SqlKey, result);
                    cache.Put(EnterPoint, enterPointDic, ExpiredTimeSpan);
                    return result;
                }

                return result;
            }
            var re = func();
            count = re.Count;
            return func();
        }

        #endregion
    }
}