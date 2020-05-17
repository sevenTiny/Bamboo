/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-18 21:10:21
 * Modify: 2018-02-18 21:10:21
 * Modify: 2020-03-28 16:22:00
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using Microsoft.Extensions.Logging;
using SevenTiny.Bantina.Logging.Infrastructure;
using System;
using System.IO;
using System.Text;

namespace SevenTiny.Bantina.Logging
{
    public class LogManager : ILogger
    {
        static LogManager()
        {
            //如果配置文件不存在，则输出配置文件
            if (!File.Exists(DefaultLog4NetConfigFileName))
            {
                File.WriteAllText(DefaultLog4NetConfigFileName, LoggingConst.ConfigContent, Encoding.UTF8);
            }
        }

        public LogManager()
        {
            //创建默认执行器
            CreateDefaultLogger();
        }

        private const string DefaultLog4NetConfigFileName = "./SevenTinyConfig/log4net.config";

        private static string CategoryName => AppSettingsConfigHelper.GetAppName();

        private ILogger _logger;

        /// <summary>
        /// 创建默认的logger对象
        /// </summary>
        private void CreateDefaultLogger()
        {
            var provider = new Log4NetProvider(DefaultLog4NetConfigFileName);
            _logger = provider.CreateLogger(CategoryName);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log<TState>(logLevel, eventId, state, exception, formatter);
        }
    }
}
