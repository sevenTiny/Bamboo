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

namespace Bamboo.Logging
{
    public class BambooLogger : ILogger
    {
        private static string DefaultLog4NetConfigFileName = Path.Combine(AppContext.BaseDirectory, "BambooConfig", "log4net.config");
        private readonly ILogger _logger;

        public BambooLogger(string categoryName = "BambooLogger")
        {
            //创建默认执行器
            var provider = new Log4NetProvider(new Log4NetProviderOptions
            {
                Log4NetConfigFileName = DefaultLog4NetConfigFileName,
            });

            _logger = provider.CreateLogger(categoryName);
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
