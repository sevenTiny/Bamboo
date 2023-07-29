using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace Bamboo.Configuration
{
    internal static class ConnectionStringManager
    {
        /// <summary>
        /// locker
        /// </summary>
        private static readonly object locker = new object();

        /// <summary>
        /// store connectionString
        /// </summary>
        private static Dictionary<string, string> _ConnectionStringDic = new Dictionary<string, string>();

        private static string GetConnectionStringFromCache(string connectionStringName, Func<string> func)
        {
            //get connection string from value
            if (_ConnectionStringDic.ContainsKey(connectionStringName))
                return _ConnectionStringDic[connectionStringName];

            lock (locker)
            {
                //multi-check
                if (_ConnectionStringDic.ContainsKey(connectionStringName))
                    return _ConnectionStringDic[connectionStringName];

                string connectionString = func();

                if (!string.IsNullOrEmpty(connectionString))
                    _ConnectionStringDic.Add(connectionStringName, connectionString);

                return connectionString;
            }
        }

        /// <summary>
        /// key in appsettings.json connection string config
        /// </summary>
        private const string DefaultAppSettingsConnectionStringKey = "BambooConfig";

        public static string GetConnectionStringFromAppSettings<T>(string configName)
        {
            return GetConnectionStringFromCache(configName, () =>
            {
                //get connection string from config file
                string baseDirectory = AppContext.BaseDirectory;

                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(baseDirectory)//current base directory
                    .AddJsonFile("appsettings.json", false, false)
                    .Build();

                string connectionString = config.GetConnectionString(ConfigConnectionStringAttribute.GetName(typeof(T)) ?? DefaultAppSettingsConnectionStringKey);

                return connectionString ?? throw new FileNotFoundException($"'appsettings.json' not find in {baseDirectory}, if existed,maybe '{configName}' node has not exist in the 'appsettings.json' ConnectionStrings file.");
            });
        }
    }
}
