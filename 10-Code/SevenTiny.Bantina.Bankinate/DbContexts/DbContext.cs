using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Cache;
using SevenTiny.Bantina.Bankinate.Configs;
using SevenTiny.Bantina.Bankinate.DataAccessEngine;
using System;
using System.Collections.Generic;
using System.Data;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class DbContext
    {
        public DbContext(DataBaseType dataBaseType, string connectionString) : this(dataBaseType, connectionString, connectionString) { }

        public DbContext(DataBaseType dataBaseType, string connectionString_ReadWrite, string connectionString_Read)
        {
            DataBaseType = dataBaseType;
            ConnString_RW = connectionString_ReadWrite;
            ConnString_R = connectionString_Read;
        }

        /// <summary>
        /// 连接字符串 ConnString_RW 读写数据库使用
        /// </summary>
        public string ConnString_RW { get; set; }
        /// <summary>
        /// 连接字符串 ConnString_R 读数据库使用
        /// </summary>
        public string ConnString_R { get; set; }
        public DataBaseType DataBaseType { get; private set; }
        public string DataBaseName { get; protected set; }
        public string TableName { get; internal set; }
        /// <summary>
        /// Sql语句
        /// </summary>
        public string SqlStatement { get; internal set; }
        /// <summary>
        /// 命令类型，可以在运行时随时灵活调整
        /// </summary>
        public CommandType CommandType { get; set; } = CommandType.Text;
        /// <summary>
        /// 参数化查询参数
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }
        /// <summary>
        /// NoSql的文档集合
        /// </summary>
        internal dynamic NoSqlCollection { get; set; }

        //Db Control
        /// <summary>
        /// 真实执行持久化操作开关，如果为false，则只执行准备动作，不实际操作数据库（友情提示：测试框架代码执行情况可以将其关闭）
        /// </summary>
        public bool OpenRealExecutionSaveToDb { get; protected set; } = true;

        //Cache Control
        /// <summary>
        /// 一级缓存
        /// 查询条件级别的缓存（filter），可以暂时缓存根据查询条件查询到的数据
        /// 如果开启二级缓存，且当前操作对应的表已经在二级缓存里，则不进行条件缓存
        /// </summary>
        public bool OpenQueryCache { get; protected set; } = false;
        /// <summary>
        /// 二级缓存
        /// 配置表缓存标签对整张数据库表进行缓存
        /// </summary>
        public bool OpenTableCache { get; protected set; } = false;
        /// <summary>
        /// 查询缓存的默认缓存时间
        /// </summary>
        private TimeSpan _QueryCacheExpiredTimeSpan = DefaultValue.QueryCacheExpiredTimeSpan;
        public TimeSpan QueryCacheExpiredTimeSpan
        {
            get { return _QueryCacheExpiredTimeSpan; }
            protected set
            {
                if (value > MaxExpiredTimeSpan)
                {
                    MaxExpiredTimeSpan = value;
                }
                _QueryCacheExpiredTimeSpan = value;
            }
        }
        /// <summary>
        /// 表缓存的缓存时间
        /// </summary>
        private TimeSpan _TableCacheExpiredTimeSpan = DefaultValue.TableCacheExpiredTimeSpan;
        public TimeSpan TableCacheExpiredTimeSpan
        {
            get { return _TableCacheExpiredTimeSpan; }
            protected set
            {
                if (value > MaxExpiredTimeSpan)
                {
                    MaxExpiredTimeSpan = value;
                }
                _TableCacheExpiredTimeSpan = value;
            }
        }
        /// <summary>
        /// 数据是否从缓存中获取
        /// </summary>
        public bool IsFromCache { get; internal set; } = false;
        /// <summary>
        /// Cache 存储媒介,默认本地缓存
        /// </summary>
        public CacheMediaType CacheMediaType { get; protected set; } = DefaultValue.CacheMediaType;
        /// <summary>
        /// Cache 第三方存储媒介服务地址
        /// </summary>
        public string CacheMediaServer { get; protected set; }
        /// <summary>
        /// 最大的缓存时间（用于缓存缓存键）
        /// </summary>
        internal TimeSpan MaxExpiredTimeSpan { get; set; } = DefaultValue.CacheKeysMaxExpiredTime;

        /// <summary>
        /// 清空全部缓存
        /// </summary>
        public void FlushAllCache() => DbCacheManager.FlushAllCache(this);
        /// <summary>
        /// 清空一级缓存
        /// </summary>
        public void FlushQueryCache() => QueryCacheManager.FlushAllCache(this);
        /// <summary>
        /// 清空二级缓存
        /// </summary>
        public void FlushTableCache() => TableCacheManager.FlushAllCache(this);

        //Validate Control
        /// <summary>
        /// 属性值校验开关，如开启，则Add/Update等操作会校验输入的值是否满足特性标签标识的条件
        /// </summary>
        public bool OpenPropertyDataValidate { get; protected set; } = false;

        //内置方法
        /// <summary>
        /// 根据实体获取表明
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public string GetTableName<TEntity>() where TEntity : class
        => TableAttribute.GetName(typeof(TEntity));
    }
}
