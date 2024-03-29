﻿/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-13 22:32:44
 * Modify: 2023年7月29日
 * Modify: 2019年6月19日 23点14分 基本完成了自动同步远程模式
 * E-mail: dong@7tiny.com | Bamboo@foxmail.com 
 * GitHub: https://github.com/Bamboo 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using Bamboo.Configuration.Core.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;

namespace Bamboo.Configuration
{
    public abstract class ConfigBase<T> where T : class, new()
    {
        /// <summary>
        /// the real configuration file path
        /// </summary>
        protected static string ConfigurationFilePath = string.Empty;
        /// <summary>
        /// the configuration name
        /// </summary>
        protected static string ConfigurationName = typeof(T).Name;
        /// <summary>
        /// Current configuration root
        /// </summary>
        private static IConfigurationRoot _ConfigurationRoot { get; set; }
        /// <summary>
        /// configuration initializer
        /// </summary>
        private static Func<IConfigurationRoot> _initializer;
        /// <summary>
        /// initializer lock
        /// </summary>
        private static object _initializerLock = new object();

        static ConfigBase()
        {
            if (!Directory.Exists(ConfigPathHelper.BaseConfigDir))
                Directory.CreateDirectory(ConfigPathHelper.BaseConfigDir);

            //This code aim to trigger child class static structurer execute
            new T();
        }

        /// <summary>
        /// copy configuration file to bamboo configuration directory
        /// </summary>
        /// <param name="configurationFilePath">The configuration file path, If not provided, try to get it from </param>
        protected static void InitializeConfigurationFile(string configurationFilePath = null)
        {
            configurationFilePath = GetConfigurationFilePath(configurationFilePath);

            if (!File.Exists(configurationFilePath))
                throw new FileNotFoundException("configuration file not found", configurationFilePath);

            var newPath = Path.Combine(ConfigPathHelper.BaseConfigDir, Path.GetFileName(configurationFilePath));

            if (File.Exists(newPath))
            {
                //if it is same file, no copy
                if (new FileInfo(configurationFilePath).FullName.Equals(new FileInfo(newPath).FullName))
                {
                    ConfigurationFilePath = newPath;
                    return;
                }
            }

            File.Copy(configurationFilePath, newPath, true);

            ConfigurationFilePath = newPath;
        }

        protected static void RegisterInitializer(Func<IConfigurationRoot> initializer)
        {
            _initializer = initializer;
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
                    if (_initializer == null)
                        throw new ArgumentException("The initializer is not set correctly");

                    lock (_initializerLock)
                    {
                        if (_ConfigurationRoot == null)
                        {
                            _ConfigurationRoot = _initializer();

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

        protected abstract string SerializeConfigurationInstance();

        /// <summary>
        /// write configuration serilized string to file
        /// </summary>
        /// <param name="configurationFilePath">configuration save path, default is configuration path</param>
        public void WriteToFile(string configurationFilePath = null)
        {
            configurationFilePath = GetConfigurationFilePath(configurationFilePath);

            var configContent = SerializeConfigurationInstance();

            File.WriteAllText(configurationFilePath, configContent, Encoding.UTF8);

            // initialize the configuration root, that it will re-build next time
            _ConfigurationRoot = null;
        }

        private static string GetConfigurationFilePath(string configurationFilePath = null)
        {
            if (string.IsNullOrEmpty(configurationFilePath))
                configurationFilePath = ConfigFileAttribute.GetFilePath(typeof(T));

            if (string.IsNullOrWhiteSpace(configurationFilePath))
                throw new ArgumentException("config file path must be provide in the correct file path format");

            //if relative path, combine the base path
            //Directory.GetCurrentDirectory() is different in web programs and test programs.
            if (!Path.IsPathRooted(configurationFilePath))
                configurationFilePath = Path.Combine(AppContext.BaseDirectory, configurationFilePath);

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