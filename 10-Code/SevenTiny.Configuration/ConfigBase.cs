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
using MySql.Data.MySqlClient;
using SevenTiny.Configuration.Extentions;
using SevenTiny.Configuration.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Configuration
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
        /// GetConnection Method:Must be override!
        /// </summary>
        /// <returns></returns>
        public virtual string GetConnectionString() => throw new NotImplementedException("GetConnection must be override and provide connection string!");

        /// <summary>
        /// Configs List
        /// </summary>
        protected static IEnumerable<T> Configs => GetConfigs();

        private static string ConnectionString
        {
            get
            {
                //get connection string from static value
                if (!string.IsNullOrEmpty(_connectionString))
                {
                    return _connectionString;
                }
                //get connection string from attribute
                _connectionString = ConfigClassAttribute.GetConnectionString(tType);
                if (!string.IsNullOrEmpty(_connectionString))
                {
                    return _connectionString;
                }
                //get connnection string from 'GetConnectionString' method
                _connectionString = tType.GetMethod("GetConnectionString").Invoke(System.Activator.CreateInstance<T>(), null).ToString();
                return _connectionString;
            }
        }

        /// <summary>
        /// Get Configs
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<T> GetConfigs()
        {
            int cacheConfigCode = $"SevenTinyConfig-{_tableName}".GetHashCode();
            int cacheVersionCode = $"SevenTinyConfigVersion-{_tableName}".GetHashCode();

            //modifyTime reagard as config version
            DateTime configDbVersion = ModifyTime;
            //if memory cache exist
            if (MemoryCacheHelper.Exist(cacheVersionCode) && MemoryCacheHelper.Exist(cacheVersionCode))
            {
                //if cache version >= db version,cache is available.
                if (Convert.ToDateTime(MemoryCacheHelper.Get<int, DateTime>(cacheVersionCode)) >= configDbVersion)
                {
                    return MemoryCacheHelper.Get<int, IEnumerable<T>>(cacheConfigCode);
                }
            }
            //if memory cache not exist
            var dbConfig = GetConfigsFromDb();
            MemoryCacheHelper.Put(cacheConfigCode, dbConfig);
            MemoryCacheHelper.Put(cacheVersionCode, configDbVersion);
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
        /// Query modifyTime of config table,means config version
        /// </summary>
        /// <returns></returns>
        private static DateTime ModifyTime
        {
            get
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    using (var cmd = new MySqlCommand())
                    {
                        string sql = $"select UPDATE_TIME from information_schema.TABLES where TABLE_SCHEMA='{TABLE_SCHEMA}' and information_schema.TABLES.TABLE_NAME='{_tableName}';";
                        SqlCommandPrepare(conn, cmd, sql);
                        var result = cmd.ExecuteScalar();
                        return Convert.ToDateTime(result);
                    }
                }
            }
        }

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
