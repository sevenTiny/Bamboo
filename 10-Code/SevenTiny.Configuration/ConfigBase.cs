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
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Configuration
{
    public abstract class ConfigBase<T> where T : class
    {
        const string TABLE_SCHEMA = "SevenTinyConfig";

        private static Type tType = typeof(T);

        /// <summary>
        /// GetConnection Method:Must be override!
        /// </summary>
        /// <returns></returns>
        public abstract string GetConnectionString();

        /// <summary>
        /// Configs List
        /// </summary>
        protected static IEnumerable<T> Configs => GetConfigs();

        private static string ConnectionString => tType.GetMethod("GetConnectionString").Invoke(System.Activator.CreateInstance<T>(), null).ToString();

        /// <summary>
        /// Get Configs
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<T> GetConfigs()
        {
            string tableName = tType.Name;
            int cacheConfigCode = $"SevenTinyConfig-{tableName}".GetHashCode();
            int cacheVersionCode = $"SevenTinyConfigVersion-{tableName}".GetHashCode();

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
                    string sql = $"select * from {typeof(T).Name}";
                    //SqlCommandPrepare(conn, cmd, sql, new MySqlParameter("@tbName", typeof(T).Name));//if use this,it is must be provider 'Allow User Variables=true' in connection string.
                    SqlCommandPrepare(conn, cmd, sql);
                    return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection).ToEnumerable<T>();
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
                        string sql = $"select UPDATE_TIME from information_schema.TABLES where TABLE_SCHEMA='{TABLE_SCHEMA}' and information_schema.TABLES.TABLE_NAME='{tType.Name}';";
                        SqlCommandPrepare(conn,cmd,sql);
                        var result = cmd.ExecuteScalar();
                        return Convert.ToDateTime(result);
                    }
                }
            }
        }

        private static void SqlCommandPrepare(MySqlConnection conn, MySqlCommand cmd, string commandText, params MySqlParameter[] parameters)
        {
            if (conn.State==System.Data.ConnectionState.Closed)
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
