/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-18 21:10:34
 * Modify: 2018-02-18 21:10:34
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Configuration;
using System;
using System.Linq;

namespace SevenTiny.Bantina.Logging.Infrastructure
{
    [ConfigName("Logging")]
    internal class LoggingConfig : MySqlRowConfigBase<LoggingConfig>
    {
        public static LoggingConfig Instance = new LoggingConfig();

        [ConfigProperty]
        public string Group { get; set; }
        [ConfigProperty]
        public string Level { get; set; }
        [ConfigProperty]
        public string StorageMedium { get; set; }
        [ConfigProperty]
        public string Directory { get; set; }
        [ConfigProperty]
        public string ConnectionString { get; set; }
        [ConfigProperty]
        public int[] Levels { get; set; }
        [ConfigProperty]
        public int[] StorageMediums { get; set; }

        protected override string _ConnectionString => GetConnectionStringFromAppSettings("SevenTinyConfig");

        private static LoggingConfig _loggingConfig;

        public static LoggingConfig Get()
        {
            if (_loggingConfig != null)
            {
                return _loggingConfig;
            }
            //load group config
            _loggingConfig = Instance.Config?.FirstOrDefault(t => t.Group.Equals(AppSettingsConfigHelper.GetAppName()))?.ExtendLevel()?.ExtendStorageMediums();
            if (_loggingConfig != null)
            {
                return _loggingConfig;
            }
            //if group not found,load root
            _loggingConfig = Instance.Config?.FirstOrDefault(t => t.Group.Equals("Default"))?.ExtendLevel()?.ExtendStorageMediums();
            if (_loggingConfig != null)
            {
                return _loggingConfig;
            }
            throw new EntryPointNotFoundException("[AppName] group not fount and [Default] goup not found also.please set least one group in logging config.");
        }
    }
    internal static class ConfigExtension
    {
        public static LoggingConfig ExtendLevel(this LoggingConfig loggingConfig)
        {
            try
            {
                loggingConfig.Levels = loggingConfig.Level.Split(',')?.Select(t => Convert.ToInt32(t))?.ToArray();
                return loggingConfig;
            }
            catch (Exception)
            {
                return loggingConfig;
            }
        }
        public static LoggingConfig ExtendStorageMediums(this LoggingConfig loggingConfig)
        {
            try
            {
                loggingConfig.StorageMediums = loggingConfig.StorageMedium.Split(',')?.Select(t => Convert.ToInt32(t))?.ToArray();
                return loggingConfig;
            }
            catch (Exception)
            {
                return loggingConfig;
            }
        }
    }
}
