using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate.Caching
{
    /// <summary>
    /// 表缓存管理器(二级缓存管理器）
    /// </summary>
    internal class TableCacheManager : CacheManagerBase
    {
        internal TableCacheManager(DbContext context, CacheOptions cacheOptions) : base(context, cacheOptions) { }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void FlushAllCache()
        {
            if (CacheStorageManager.IsExist(CachingConst.GetTableCacheKeysCacheKey(DbContext.DataBaseName), out HashSet<string> keys))
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
            CacheStorageManager.Delete(GetTableCacheKey(collectionName));
        }

        private string GetTableCacheKey(string collectionName = null)
        {
            string key = $"{CachingConst.CacheKey_TableCache}_{DbContext.DataBaseName}_{collectionName ?? DbContext.CollectionName}";

            //缓存键更新
            if (!CacheStorageManager.IsExist(CachingConst.GetTableCacheKeysCacheKey(DbContext.DataBaseName), out HashSet<string> keys))
                keys = new HashSet<string>();

            keys.Add(key);

            CacheStorageManager.Put(CachingConst.GetTableCacheKeysCacheKey(DbContext.DataBaseName), keys, CacheOptions.MaxExpiredTimeSpan);

            return key;
        }

        /// <summary>
        /// 更新数据到缓存（Add）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        internal void AddCache<TEntity>(TEntity entity)
        {
            var tableName = TableAttribute.GetName(typeof(TEntity));
            //如果存在表级别缓存，则更新数据到缓存
            if (CacheStorageManager.IsExist(GetTableCacheKey(tableName), out List<TEntity> entities))
            {
                if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
                {
                    entities.Add(entity);
                    //如果过期时间为0，则取上下文的过期时间
                    CacheStorageManager.Put(GetTableCacheKey(tableName), entities, tableCacheTimeSpan == TimeSpan.Zero ? CacheOptions.TableCacheExpiredTimeSpan : tableCacheTimeSpan);
                }
            }
        }
        /// <summary>
        /// 更新数据到缓存（Add）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="values"></param>
        internal void AddCache<TEntity>(IEnumerable<TEntity> values)
        {
            var tableName = TableAttribute.GetName(typeof(TEntity));
            //如果存在表级别缓存，则更新数据到缓存
            if (CacheStorageManager.IsExist(GetTableCacheKey(tableName), out List<TEntity> entities))
            {
                if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
                {
                    //如果过期时间为0，则取上下文的过期时间
                    TimeSpan timeSpan = tableCacheTimeSpan == TimeSpan.Zero ? CacheOptions.TableCacheExpiredTimeSpan : tableCacheTimeSpan;

                    entities.AddRange(values);
                    CacheStorageManager.Put(GetTableCacheKey(tableName), entities, tableCacheTimeSpan);
                }
            }
        }
        /// <summary>
        /// 更新数据到缓存（Update）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <param name="filter"></param>
        internal void UpdateCache<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            Ensure.ArgumentNotNullOrEmpty(filter, nameof(filter));

            var tableName = TableAttribute.GetName(typeof(TEntity));
            //如果存在表级别缓存，则更新数据到缓存
            if (CacheStorageManager.IsExist(GetTableCacheKey(tableName), out List<TEntity> entities))
            {
                if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
                {
                    //如果过期时间为0，则取上下文的过期时间
                    TimeSpan timeSpan = tableCacheTimeSpan == TimeSpan.Zero ? CacheOptions.TableCacheExpiredTimeSpan : tableCacheTimeSpan;
                    //从缓存集合中寻找该记录，如果找到，则更新该记录
                    var val = entities.Where(filter.Compile()).FirstOrDefault();
                    if (val != null)
                    {
                        val = entity;
                        CacheStorageManager.Put(GetTableCacheKey(tableName), entities, timeSpan);
                    }
                }
            }
        }
        /// <summary>
        /// 更新数据到缓存（Delete）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="filter"></param>
        internal void DeleteCache<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            Ensure.ArgumentNotNullOrEmpty(filter, nameof(filter));

            var tableName = TableAttribute.GetName(typeof(TEntity));
            //如果存在表级别缓存，则更新数据到缓存
            if (CacheStorageManager.IsExist(GetTableCacheKey(tableName), out List<TEntity> entities))
            {
                if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
                {
                    //从缓存集合中寻找该记录，如果找到，则更新该记录
                    var list = entities.Where(filter.Compile()).ToList();
                    if (list != null && list.Any())
                    {
                        entities.RemoveAll(t => list.Contains(t));
                        //如果过期时间为0，则取上下文的过期时间
                        CacheStorageManager.Put(GetTableCacheKey(tableName), entities, tableCacheTimeSpan == TimeSpan.Zero ? CacheOptions.TableCacheExpiredTimeSpan : tableCacheTimeSpan);
                    }
                }
            }
        }
        /// <summary>
        /// 更新数据到缓存（Delete）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        internal void DeleteCache<TEntity>(TEntity entity)
        {
            var tableName = TableAttribute.GetName(typeof(TEntity));
            //如果存在表级别缓存，则更新数据到缓存
            if (CacheStorageManager.IsExist(GetTableCacheKey(tableName), out List<TEntity> entities))
            {
                if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
                {
                    //如果过期时间为0，则取上下文的过期时间
                    TimeSpan timeSpan = tableCacheTimeSpan == TimeSpan.Zero ? CacheOptions.TableCacheExpiredTimeSpan : tableCacheTimeSpan;
                    //从缓存集合中寻找该记录，如果找到，则更新该记录
                    var val = entities.Find(t => t.Equals(entity));
                    if (val != null)
                    {
                        entities.Remove(val);
                        CacheStorageManager.Put(GetTableCacheKey(tableName), entities, tableCacheTimeSpan);
                    }
                }
            }
        }

        /// <summary>
        /// 从缓存中获取数据，如果没有，则后台执行扫描表任务
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        internal List<TEntity> GetEntitiesFromCache<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            Ensure.ArgumentNotNullOrEmpty(filter, nameof(filter));

            //1.如果TableCache里面有该缓存键，则直接获取
            if (CacheStorageManager.IsExist(GetTableCacheKey(TableAttribute.GetName(typeof(TEntity))), out List<TEntity> entities))
            {
                DbContext.IsFromCache = true;
                return entities.Where(filter.Compile()).ToList();
            }

            //2.则判断是否需要对该表进行扫描（含有TableCachingAttribute的标记的类才可以有扫描全表的权限）
            if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
            {
                //执行扫描全表数据任务
                ScanTableBackground<TEntity>(tableCacheTimeSpan);
            }

            return null;
        }

        private readonly object tableScaningLocker = new object();
        /// <summary>
        /// 后台扫描全表数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext">上下文</param>
        /// <param name="tableCacheTimeSpan">tableCache过期时间</param>
        private void ScanTableBackground<TEntity>(TimeSpan tableCacheTimeSpan) where TEntity : class
        {
            string scanKey = $"{CachingConst.CacheKey_TableScanning}{DbContext.CollectionName}";
            //1.判断正在扫描键是否存在，如果存在，则返回null，继续等待扫描任务完成
            if (CacheStorageManager.IsExist(scanKey))
                return;

            //设置扫描键，标识当前正在进行扫描
            CacheStorageManager.Put(scanKey, 1, CachingConst.SpanScaningKeyExpiredTime);

            //2.如果没有扫描键，则执行后台扫描任务
            Task.Factory.StartNew(() =>
            {
                //对扫描任务加锁，防止多线程环境多次执行任务
                lock (tableScaningLocker)
                {
                    var tableName = TableAttribute.GetName(typeof(TEntity));
                    //双重校验当前缓存是否存在TableCache，防止重复获取
                    if (CacheStorageManager.IsExist(GetTableCacheKey(tableName)))
                        return;

                    //如果过期时间为0，则取上下文的过期时间
                    TimeSpan timeSpan = tableCacheTimeSpan == TimeSpan.Zero ? CacheOptions.TableCacheExpiredTimeSpan : tableCacheTimeSpan;

                    //执行扫描全表任务，并将结果存入缓存中
                    var data = DbContext.GetFullCollectionData<TEntity>();

                    //如果没查到，会存个空的集合
                    CacheStorageManager.Put(GetTableCacheKey(tableName), data ?? new List<TEntity>(), CacheOptions.TableCacheExpiredTimeSpan);
                }
                //将扫描键移除，表示已经扫描完成
                CacheStorageManager.Delete(scanKey);
            });
        }
    }
}
