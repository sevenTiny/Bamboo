/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-18 21:10:21
 * Modify: 2018-02-18 21:10:21
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Logging.Infrastructure;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Logging
{
    public class LogManager : ILog
    {
        public LogManager()
        {
            LogStorage._LoggingConfig = LoggingConfig.Get();
        }
        public LogManager(string group)
        {
            LogStorage._LoggingConfig = LoggingConfig.Get(group);
        }

        public void Debug(string message) => LogMessage(LoggingLevel.Debug, $"(Debug)]:{message}");

        public void Error(string message) => LogMessage(LoggingLevel.Error, $"(Error)]:{message}");

        public void Error(Exception exception) => LogException(LoggingLevel.Error, exception);

        public void Error(string message, Exception exception) => LogException(LoggingLevel.Fatal, message, exception);

        public void Fatal(string message) => LogMessage(LoggingLevel.Fatal, $"(Fatal)]:{message}");

        public void Fatal(Exception exception) => LogException(LoggingLevel.Fatal, exception);

        public void Fatal(string message, Exception exception) => LogException(LoggingLevel.Fatal, message, exception);

        public void Info(string message) => LogMessage(LoggingLevel.Info, $"(Info)]:{message}");

        public void Warn(string message) => LogMessage(LoggingLevel.Warn, $"(Warn)]:{message}");

        private void LogMessage(LoggingLevel loggingLevel, string message)
        {
            LogStorage.Storage(loggingLevel, $"\n---- [{DateTime.Now.ToString("yyyyMMdd hh:mm:ss")} {message}");
        }

        private void LogException(LoggingLevel loggingLevel, Exception exception)
        {
            Task.Run(() =>
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"Message:{exception.Message}\r\n");
                builder.Append($"Source:{exception.Source}");
                builder.Append($"StackTrace:{exception.StackTrace}\r\n");
                builder.Append($"InnerException:{exception.InnerException}\r\n");
                LogStorage.Storage(loggingLevel, builder.ToString());
            });
        }
        private void LogException(LoggingLevel loggingLevel, string message, Exception exception)
        {
            Task.Run(() =>
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"CustomMessage:{message}\r\n");
                builder.Append($"Message:{exception.Message}\r\n");
                builder.Append($"Source:{exception.Source}");
                builder.Append($"StackTrace:{exception.StackTrace}\r\n");
                builder.Append($"InnerException:{exception.InnerException}\r\n");
                LogStorage.Storage(loggingLevel, builder.ToString());
            });
        }
    }
}
