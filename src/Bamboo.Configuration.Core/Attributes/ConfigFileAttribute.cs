/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2023年7月29日
 * E-mail: seventiny@foxmail.com 
 * GitHub: https://github.com/Bamboo 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;
using System.Linq;

namespace Bamboo.Configuration
{
    /// <summary>
    /// Configuration file path setting
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ConfigFileAttribute : Attribute
    {
        /// <summary>
        /// the configuration file path contains suffix
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Configuration file path setting
        /// </summary>
        /// <param name="filePath">The path parameter must be in the correct file path format, can be relative path or absolute path. The file contains a suffix.</param>
        /// <exception cref="ArgumentException"></exception>
        public ConfigFileAttribute(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("path argument must be a correct file path format");

            this.FilePath = filePath;
        }

        public static string GetFilePath(Type type)
        {
            var filePath = (type.GetCustomAttributes(typeof(ConfigFileAttribute), true)?.FirstOrDefault() as ConfigFileAttribute)?.FilePath;

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Failed to get configuration file path, please check 'ConfigFileAttribute' has been added ont the config class and filePath argument must be a correct file path format.");

            return filePath;

        }
    }
}
