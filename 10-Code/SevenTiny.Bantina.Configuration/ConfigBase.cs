/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-13 22:32:44
 * Modify: 2018-02-13 22:32:44
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using SevenTiny.Bantina.Configuration.Extensions;
using SevenTiny.Bantina.Configuration.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SevenTiny.Bantina.Configuration
{
    public abstract class ConfigBase<T> where T : class
    {
        #region fields

        /// <summary>
        /// db name
        /// </summary>
        const string TABLE_SCHEMA = "SevenTinyConfig";

        /// <summary>
        /// T type
        /// </summary>
        private static readonly Type tType = typeof(T);

        /// <summary>
        /// connection string
        /// </summary>
        private static string _connectionString;

        /// <summary>
        /// search table name in db
        /// </summary>
        private static string _tableName = GetTableName();

        #endregion

        /// <summary>
        /// GetTableName
        /// 1.from attribute config
        /// 2.from Type name
        /// </summary>
        /// <returns></returns>
        private static string GetTableName()
        {
            _tableName = ConfigClassAttribute.GetName(tType);
            if (!string.IsNullOrEmpty(_tableName))
            {
                return _tableName;
            }
            return tType.Name;
        }

        /// <summary>
        /// Configs List
        /// </summary>
        protected static IEnumerable<T> Configs => GetConfigs();

        /// <summary>
        /// Connection string
        /// </summary>
        private static string ConnectionString
        {
            get
            {
                //get connection string from static value
                if (!string.IsNullOrEmpty(_connectionString))
                {
                    return _connectionString;
                }
                //get connection string from config file
                string baseDirectory = AppContext.BaseDirectory;
                IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(baseDirectory)//current base directory
                .AddJsonFile("appsettings.json", false, true)
                .Build();

                string connectionString = config.GetConnectionString("SevenTinyConfig");

                if (!string.IsNullOrEmpty(connectionString))
                {
                    return _connectionString = connectionString;
                }

                throw new FileNotFoundException($"'appsettings.json' not find in {baseDirectory}, if existed,maybe'SevenTinyConfig' node has not exist in the 'appsettings.json'.");
            }
        }

        private static int configCacheKey = $"SevenTinyConfig-{_tableName}".GetHashCode();
        private static int versionCacheKey = $"SevenTinyConfig-Version-{_tableName}".GetHashCode();
        /// <summary>
        /// Get Configs
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<T> GetConfigs()
        {
            //modifyTime reagard as config version
            DateTime configDbVersion = ModifyTime;
            //if memory cache exist
            if (MemoryCacheHelper.Exist(versionCacheKey) && MemoryCacheHelper.Exist(versionCacheKey))
            {
                //if cache version >= db version,cache is available.
                if (Convert.ToDateTime(MemoryCacheHelper.Get<int, DateTime>(versionCacheKey)) >= configDbVersion)
                {
                    return MemoryCacheHelper.Get<int, IEnumerable<T>>(configCacheKey);
                }
            }
            //if memory cache not exist
            var dbConfig = GetConfigsFromDb();
            MemoryCacheHelper.Put(configCacheKey, dbConfig);
            MemoryCacheHelper.Put(versionCacheKey, configDbVersion);
            return dbConfig;
        }

        private static IEnumerable<T> GetConfigsFromDb()
        {
            using (var conn = new MySqlConnection(ConnectionString))
            {
                using (var cmd = new MySqlCommand())
                {
                    string sql = $"select * from {_tableName}";
                    //SqlCommandPrepare(conn, cmd, sql, new MySqlParameter("@tbName", typeof(T).Name));//if use this,it is must be provider 'Allow User Variables=true' in connection string.
                    SqlCommandPrepare(conn, cmd, sql);
                    return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection).ToList<T>();
                }
            }
        }

        /// <summary>
        /// modify time expired time
        /// </summary>
        private static TimeSpan modifyTimeExpiredTime = TimeSpan.FromMinutes(10);
        /// <summary>
        /// modify cache code
        /// </summary>
        private static int modifyTimeCacheKey = $"SevenTinyConfigModifyTime".GetHashCode();
        /// <summary>
        /// Query modifyTime of config table,means config version
        /// </summary>
        /// <returns></returns>
        private static DateTime ModifyTime
        {
            get
            {
                //get from cache
                if (MemoryCacheHelper.Exist(modifyTimeCacheKey))
                {
                    var modifyTimeCacheValue = MemoryCacheHelper.Get<int, DateTime>(modifyTimeCacheKey);
                    if (modifyTimeCacheValue != null)
                    {
                        return modifyTimeCacheValue;
                    }
                }
                //get from db
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    using (var cmd = new MySqlCommand())
                    {
                        string sql = $"select UPDATE_TIME from information_schema.TABLES where TABLE_SCHEMA='{TABLE_SCHEMA}' and information_schema.TABLES.TABLE_NAME='{_tableName}';";
                        SqlCommandPrepare(conn, cmd, sql);
                        var result = Convert.ToDateTime(cmd.ExecuteScalar());
                        //sotrage into cache
                        MemoryCacheHelper.Put(modifyTimeCacheKey,result,modifyTimeExpiredTime);
                        return result;
                    }
                }
            }
        }

        /// <summary>
        /// sql prepare command
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmd"></param>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        private static void SqlCommandPrepare(MySqlConnection conn, MySqlCommand cmd, string commandText, params MySqlParameter[] parameters)
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            cmd.CommandText = commandText;
            cmd.Connection = conn;
            cmd.CommandTimeout = 60;
            if (cmd.Parameters.Any())
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parameters);
            }
        }
    }
}
