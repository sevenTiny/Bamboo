/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Chengdu
 * Create: 2018-02-13 22:32:44
 * E-mail: dong@7tiny.com | seventiny@foxmail.com
 * GitHub: https://github.com/Bamboo 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using Bamboo.Configuration.Helpers;
using Bamboo.Configuration.Providers;
using Bamboo.Configuration.Remote;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;

namespace Bamboo.Configuration
{
    public abstract class ConfigBase<T> where T : class, new()
    {
        /// <summary>
        /// the configuration name
        /// </summary>
        private static string ConfigurationName = typeof(T).Name;
        /// <summary>
        /// Current configuration root
        /// </summary>
        protected static IConfigurationRoot _ConfigurationRoot { get; set; }
        /// <summary>
        /// initializer lock
        /// </summary>
        private static object _initializerLock = new object();

        static ConfigBase()
        {
            if (!Directory.Exists(ConfigPathHelper.BaseConfigDir))
                Directory.CreateDirectory(ConfigPathHelper.BaseConfigDir);

            //fetch remote
            RemoteManager.Fetch();
        }

        /// <summary>
        /// get configuration root
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public static IConfigurationRoot ConfigurationRoot
        {
            get
            {
                if (_ConfigurationRoot == null)
                {
                    lock (_initializerLock)
                    {
                        if (_ConfigurationRoot == null)
                        {
                            var configurationFilePath = GetConfigurationFilePath();

                            if (!File.Exists(configurationFilePath))
                                throw new FileNotFoundException("configuration file not found", configurationFilePath);

                            _ConfigurationRoot = ConfigurationProviderManager.GetProvider(configurationFilePath).GetConfigurationRoot(configurationFilePath);

                            if (_ConfigurationRoot == null)
                                throw new ApplicationException($"The config '{ConfigurationName}' initialization fail");
                        }
                    }
                }

                return _ConfigurationRoot;
            }
        }

        /// <summary>
        /// get configuration instance
        /// </summary>
        /// <returns></returns>
        public static T Get()
        {
            return ConfigurationRoot.Get<T>();
        }

        /// <summary>
        /// get configuration value
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValue<TValue>(string key)
        {
            return ConfigurationRoot.GetValue<TValue>(key);
        }

        /// <summary>
        /// verify if the configuration file is exists
        /// </summary>
        /// <returns></returns>
        public static bool FileExists()
        {
            return File.Exists(GetConfigurationFilePath());
        }

        /// <summary>
        /// get the configuration file setting path in attribute
        /// </summary>
        /// <returns></returns>
        public static string GetFilePath()
        {
            return ConfigFileAttribute.GetFilePath(typeof(T));
        }

        /// <summary>
        /// get the configuration file full path
        /// </summary>
        /// <returns></returns>
        public static string GetFileFullPath()
        {
            return GetConfigurationFilePath();
        }

        /// <summary>
        /// write configuration serilized string to file
        /// </summary>
        /// <returns>file save path</returns>
        public string WriteToFile()
        {
            var configurationFilePath = GetConfigurationFilePath();

            var configContent = ConfigurationProviderManager.GetProvider(configurationFilePath).Serilize(this);

            File.WriteAllText(configurationFilePath, configContent, Encoding.UTF8);

            // initialize the configuration root, that it will re-build next time
            _ConfigurationRoot = null;

            return configurationFilePath;
        }

        private static string GetConfigurationFilePath()
        {
            var configurationFilePath = ConfigFileAttribute.GetFilePath(typeof(T));

            if (string.IsNullOrWhiteSpace(configurationFilePath))
                throw new ArgumentException("config file path must be provide in the correct file path format");

            // if relative path, combine the base path
            // Note: Directory.GetCurrentDirectory() is different in web programs and test programs.
            if (!Path.IsPathRooted(configurationFilePath) && !"appsettings.json".Equals(configurationFilePath))
                configurationFilePath = Path.Combine(ConfigurationConst.ConfigurationBaseFolder, configurationFilePath);

            return configurationFilePath;
        }
    }

    public static class ConfigurationExtension
    {
        /// <summary>
        /// bind the configuration to instance
        /// </summary>
        /// <returns></returns>
        public static T Bind<T>(this T instance) where T : class, new()
        {
            ConfigBase<T>.ConfigurationRoot.Bind(instance);
            return instance;
        }
    }
}