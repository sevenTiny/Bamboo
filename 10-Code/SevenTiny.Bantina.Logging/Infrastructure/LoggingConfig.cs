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
    [ConfigClass(Name = "Logging")]
    public class LoggingConfig : ConfigBase<LoggingConfig>
    {
        public string Group { get; set; }
        public string Level { get; set; }
        public string StorageMedium { get; set; }
        public string Directory { get; set; }
        public string ConnectionString { get; set; }
        public int[] Levels { get; set; }
        public int[] StorageMediums { get; set; }

        private static LoggingConfig _loggingConfig;

        public static LoggingConfig Get()
        {
            if (_loggingConfig != null)
            {
                return _loggingConfig;
            }
            //if group not found,load root
            _loggingConfig = Configs.FirstOrDefault(t => t.Group.Equals("Root"))?.ExtendLevel()?.ExtendStorageMediums();
            if (_loggingConfig != null)
            {
                return _loggingConfig;
            }
            return new LoggingConfig();
        }

        public static LoggingConfig Get(string group)
        {
            if (_loggingConfig != null)
            {
                return _loggingConfig;
            }
            //load group config
            _loggingConfig = Configs.FirstOrDefault(t => t.Group.Equals(group))?.ExtendLevel()?.ExtendStorageMediums();
            if (_loggingConfig != null)
            {
                return _loggingConfig;
            }
            //if group not found,load root
            _loggingConfig = Configs.FirstOrDefault(t => t.Group.Equals("Root"))?.ExtendLevel()?.ExtendStorageMediums();
            if (_loggingConfig != null)
            {
                return _loggingConfig;
            }
            return new LoggingConfig();
        }
    }
    public static class ConfigExtension
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
