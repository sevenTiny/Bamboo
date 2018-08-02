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

        public void Debug(string message) => LogMessage(LoggingLevel.Debug, message);

        public void Error(string message) => LogMessage(LoggingLevel.Error, message);

        public void Error(Exception exception) => ExceptionLog(LoggingLevel.Error, exception);

        public void Error(string message, Exception exception) => ExceptionLog(LoggingLevel.Error, message, exception);

        public void Fatal(string message) => LogMessage(LoggingLevel.Fatal, message);

        public void Fatal(Exception exception) => ExceptionLog(LoggingLevel.Fatal, exception);

        public void Fatal(string message, Exception exception) => ExceptionLog(LoggingLevel.Fatal, message, exception);

        public void Info(string message) => LogMessage(LoggingLevel.Info, message);

        public void Warn(string message) => LogMessage(LoggingLevel.Warn, message);

        private void ExceptionLog(LoggingLevel loggingLevel, Exception exception)
        {
            Task.Run(() =>
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"Message:{exception.Message}\r\n");
                builder.Append($"Source:{exception.Source}\r\n");
                builder.Append($"StackTrace:{exception.StackTrace}\r\n");
                builder.Append($"InnerException:{exception.InnerException}\r\n");
                LogMessage(loggingLevel, builder.ToString());
            });
        }
        private void ExceptionLog(LoggingLevel loggingLevel, string message, Exception exception)
        {
            Task.Run(() =>
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"CustomMessage:{message}\r\n");
                builder.Append($"Message:{exception.Message}\r\n");
                builder.Append($"Source:{exception.Source}\r\n");
                builder.Append($"StackTrace:{exception.StackTrace}\r\n");
                builder.Append($"InnerException:{exception.InnerException}\r\n");
                LogMessage(loggingLevel, builder.ToString());
            });
        }
        
        /// <summary>
        /// Connon storage
        /// </summary>
        /// <param name="loggingLevel"></param>
        /// <param name="message"></param>
        private void LogMessage(LoggingLevel loggingLevel, string message)
        {
            string loggingLevelString = "";
            switch (loggingLevel)
            {
                case LoggingLevel.Info:
                    loggingLevelString = "Info";
                    break;
                case LoggingLevel.Debug:
                    loggingLevelString = "Debug";
                    break;
                case LoggingLevel.Warn:
                    loggingLevelString = "Warn";
                    break;
                case LoggingLevel.Error:
                    loggingLevelString = "Error";
                    break;
                case LoggingLevel.Fatal:
                    loggingLevelString = "Fatal";
                    break;
                default:
                    break;
            }
            LogStorage.Storage(loggingLevel, $"\r\n---- [{DateTime.Now.ToString()}] {loggingLevelString}:\r\n{message}");
        }
    }
}
