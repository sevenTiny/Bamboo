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
using System;
using System.IO;
using System.Text;

namespace Bamboo.Logging
{
    public class BambooLogger<TCategoryName> : ILogger<TCategoryName>
    {
        static BambooLogger()
        {
            //如果配置文件不存在，则输出配置文件
            if (!File.Exists(DefaultLog4NetConfigFileName))
            {
                var directory = Path.GetDirectoryName(DefaultLog4NetConfigFileName);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(DefaultLog4NetConfigFileName, LoggingConst.ConfigContent, Encoding.UTF8);
            }
        }

        private static string DefaultLog4NetConfigFileName = Path.Combine(AppContext.BaseDirectory, "SevenTinyConfig", "log4net.config");
        private ILogger _logger;

        public BambooLogger()
        {
            //创建默认执行器
            var provider = new Log4NetProvider(DefaultLog4NetConfigFileName);
            _logger = provider.CreateLogger(typeof(TCategoryName).Name);
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
