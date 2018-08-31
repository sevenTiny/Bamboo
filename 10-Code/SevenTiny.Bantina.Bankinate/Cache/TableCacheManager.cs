using MongoDB.Driver;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Configs;
using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate.Cache
{
    /// <summary>
    /// 表缓存管理器(二级缓存管理器）
    /// </summary>
    internal abstract class TableCacheManager
    {
        /// <summary>
        /// 清空所有缓存
        /// </summary>
        internal static void FlushAllCache(DbContext dbContext)
        {
            if (CacheStorageManager.IsExist(dbContext, DefaultValue.GetTableCacheKeysCacheKey(dbContext.DataBaseName), out HashSet<string> keys))
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
            CacheStorageManager.Delete(dbContext, GetTableCacheKey(dbContext));
        }

        private static string GetTableCacheKey(DbContext dbContext)
        {
            string key = $"{DefaultValue.CacheKey_TableCache}{dbContext.TableName}";
            //缓存键更新
            if (!CacheStorageManager.IsExist(dbContext, DefaultValue.GetTableCacheKeysCacheKey(dbContext.DataBaseName), out HashSet<string> keys))
            {
                keys = new HashSet<string>();
            }
            keys.Add(key);
            CacheStorageManager.Put(dbContext, DefaultValue.GetTableCacheKeysCacheKey(dbContext.DataBaseName), keys, dbContext.MaxExpiredTimeSpan);
            return key;
        }

        /// <summary>
        /// 更新数据到缓存（Add）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        internal static void AddCache<TEntity>(DbContext dbContext, TEntity entity)
        {
            if (dbContext.OpenTableCache)
            {
                //如果存在表级别缓存，则更新数据到缓存
                if (CacheStorageManager.IsExist(dbContext, GetTableCacheKey(dbContext), out List<TEntity> entities))
                {
                    if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
                    {
                        //如果过期时间为0，则取上下文的过期时间
                        TimeSpan timeSpan = tableCacheTimeSpan == TimeSpan.Zero ? dbContext.TableCacheExpiredTimeSpan : tableCacheTimeSpan;

                        entities.Add(entity);
                        CacheStorageManager.Put(dbContext, GetTableCacheKey(dbContext), entities, tableCacheTimeSpan);
                    }
                }
            }
        }
        /// <summary>
        /// 更新数据到缓存（Add）
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="values"></param>
        internal static void AddCache<TEntity>(DbContext dbContext, IEnumerable<TEntity> values)
        {
            if (dbContext.OpenTableCache)
            {
                //如果存在表级别缓存，则更新数据到缓存
                if (CacheStorageManager.IsExist(dbContext, GetTableCacheKey(dbContext), out List<TEntity> entities))
                {
                    if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
                    {
                        //如果过期时间为0，则取上下文的过期时间
                        TimeSpan timeSpan = tableCacheTimeSpan == TimeSpan.Zero ? dbContext.TableCacheExpiredTimeSpan : tableCacheTimeSpan;

                        entities.AddRange(values);
                        CacheStorageManager.Put(dbContext, GetTableCacheKey(dbContext), entities, tableCacheTimeSpan);
                    }
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
        internal static void UpdateCache<TEntity>(DbContext dbContext, TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            if (dbContext.OpenTableCache)
            {
                //如果存在表级别缓存，则更新数据到缓存
                if (CacheStorageManager.IsExist(dbContext, GetTableCacheKey(dbContext), out List<TEntity> entities))
                {

                    if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
                    {
                        //如果过期时间为0，则取上下文的过期时间
                        TimeSpan timeSpan = tableCacheTimeSpan == TimeSpan.Zero ? dbContext.TableCacheExpiredTimeSpan : tableCacheTimeSpan;
                        //从缓存集合中寻找该记录，如果找到，则更新该记录
                        var val = entities.Where(filter.Compile()).FirstOrDefault();
                        if (val != null)
                        {
                            val = entity;
                            CacheStorageManager.Put(dbContext, GetTableCacheKey(dbContext), entities, tableCacheTimeSpan);
                        }
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
        internal static void DeleteCache<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter)
        {
            if (dbContext.OpenTableCache)
            {
                //如果存在表级别缓存，则更新数据到缓存
                if (CacheStorageManager.IsExist(dbContext, GetTableCacheKey(dbContext), out List<TEntity> entities))
                {

                    if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
                    {
                        //如果过期时间为0，则取上下文的过期时间
                        TimeSpan timeSpan = tableCacheTimeSpan == TimeSpan.Zero ? dbContext.TableCacheExpiredTimeSpan : tableCacheTimeSpan;
                        //从缓存集合中寻找该记录，如果找到，则更新该记录
                        var val = entities.Where(filter.Compile()).FirstOrDefault();
                        if (val != null)
                        {
                            entities.Remove(val);
                            CacheStorageManager.Put(dbContext, GetTableCacheKey(dbContext), entities, tableCacheTimeSpan);
                        }
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
        internal static List<TEntity> GetEntitiesFromCache<TEntity>(DbContext dbContext, Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            //1.检查是否开启了Table缓存
            if (!dbContext.OpenTableCache)
            {
                return null;
            }

            //2.如果TableCache里面有该缓存键，则直接获取
            if (CacheStorageManager.IsExist(dbContext, GetTableCacheKey(dbContext), out List<TEntity> entities))
            {
                dbContext.IsFromCache = true;
                return entities.Where(filter.Compile()).ToList();
            }

            //3.则判断是否需要对该表进行扫描（含有TableCachingAttribute的标记的类才可以有扫描全表的权限）
            if (TableCachingAttribute.IsExistTaleCaching(typeof(TEntity), out TimeSpan tableCacheTimeSpan))
            {
                //执行扫描全表数据任务
                ScanTableBackground<TEntity>(dbContext, tableCacheTimeSpan);
            }

            return null;
        }

        private static readonly object tableScaningLocker = new object();
        /// <summary>
        /// 后台扫描全表数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext">上下文</param>
        /// <param name="tableCacheTimeSpan">tableCache过期时间</param>
        private static void ScanTableBackground<TEntity>(DbContext dbContext, TimeSpan tableCacheTimeSpan) where TEntity : class
        {
            string scanKey = $"{DefaultValue.CacheKey_TableScanning}{dbContext.TableName}";
            //1.判断正在扫描键是否存在，如果存在，则返回null，继续等待扫描任务完成
            if (CacheStorageManager.IsExist(dbContext, scanKey))
            {
                return;
            }
            //2.如果没有扫描键，则执行后台扫描任务
            Task.Run(() =>
            {
                //设置扫描键，标识当前正在进行扫描
                CacheStorageManager.Put(dbContext, scanKey, 1, DefaultValue.SpanScaningKeyExpiredTime);
                //对扫描任务加锁，防止多线程环境多次执行任务
                lock (tableScaningLocker)
                {
                    //双重校验当前缓存是否存在TableCache，防止多个进程在锁外等待，所释放后再次执行
                    if (CacheStorageManager.IsExist(dbContext, GetTableCacheKey(dbContext)))
                    {
                        return;
                    }
                    //如果过期时间为0，则取上下文的过期时间
                    TimeSpan timeSpan = tableCacheTimeSpan == TimeSpan.Zero ? dbContext.TableCacheExpiredTimeSpan : tableCacheTimeSpan;
                    //执行扫描全表任务，并将结果存入缓存中
                    var data = GetFullTableData<TEntity>(dbContext);
                    if (data != null)
                    {
                        CacheStorageManager.Put(dbContext, GetTableCacheKey(dbContext), data, dbContext.TableCacheExpiredTimeSpan);
                    }
                }
                //将扫描键移除，表示已经扫描完成
                CacheStorageManager.Delete(dbContext, scanKey);
            });
        }
        /// <summary>
        /// 获取全表数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private static List<TEntity> GetFullTableData<TEntity>(DbContext dbContext) where TEntity : class
        {
            switch (dbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                    return DbHelper.ExecuteList<TEntity>($"SELECT * FROM {dbContext.TableName}");
                case DataBaseType.MySql:
                    return DbHelper.ExecuteList<TEntity>($"SELECT * FROM {dbContext.TableName}");
                case DataBaseType.Oracle:
                    return DbHelper.ExecuteList<TEntity>($"SELECT * FROM {dbContext.TableName}");
                case DataBaseType.MongoDB:
                    return (dbContext.NoSqlCollection as IMongoCollection<TEntity>).Find(t => true).ToList();//获取MongoDb全文档记录
                default:
                    return null;
            }
        }
    }
}
