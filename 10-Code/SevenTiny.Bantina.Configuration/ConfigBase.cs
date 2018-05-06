/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-13 22:32:44
 * Modify: 2018-05-06 22:00:27
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

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
        /// connection string
        /// </summary>
        private static string _connectionString;

        /// <summary>
        /// search table name in db
        /// </summary>
        private static string ConfigName => ConfigNameAttribute.GetName(typeof(T));

        /// <summary>
        /// config base directory
        /// </summary>
        private static string BaseConfigPath => $"{Path.Combine(AppContext.BaseDirectory, TABLE_SCHEMA)}";

        /// <summary>
        /// config file full path xxx.json
        /// </summary>
        private static string ConfigFileFullPath => $"{Path.Combine(BaseConfigPath, ConfigName)}.json";

        /// <summary>
        /// locker
        /// </summary>
        private static readonly object locker = new object();

        #endregion

        private static IEnumerable<T> _Configs;

        /// <summary>
        /// Configs List
        /// </summary>
        protected static IEnumerable<T> Configs
        {
            get
            {
                if (_Configs != null)
                {
                    return _Configs;
                }
                //mutex locker
                using (Mutex myMutex = new Mutex(true, "config mutex lock"))
                {
                    myMutex.WaitOne();
                    try
                    {
                        if (_Configs != null)
                        {
                            return _Configs;
                        }
                        //1.from local file
                        if (Directory.Exists(BaseConfigPath))
                        {
                            if (File.Exists(ConfigFileFullPath))
                            {
                                return _Configs = JsonConvert.DeserializeObject<IEnumerable<T>>(File.ReadAllText(ConfigFileFullPath));
                            }
                        }
                        //2.from remote config server
                        using (var db = new ConfigDbContext(ConnectionString))
                        {
                            _Configs = db.ExecuteQueryListSql<T>($"SELECT * FROM {ConfigName}");
                            if (!Directory.Exists(BaseConfigPath))
                            {
                                Directory.CreateDirectory(BaseConfigPath);
                            }
                            using (StreamWriter writer = new StreamWriter(ConfigFileFullPath, true))
                            {
                                writer.AutoFlush = true;
                                writer.WriteLine(JsonConvert.SerializeObject(_Configs));
                            }
                            return _Configs;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        myMutex.ReleaseMutex();
                    }
                }
            }
        }


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
                lock (locker)
                {
                    //multi-check
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
                    throw new FileNotFoundException($"'appsettings.json' not find in {baseDirectory}, if existed,maybe 'SevenTinyConfig' node has not exist in the 'appsettings.json'.ConnectionStrings");
                }
            }
        }
    }
}