/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-18 20:53:04
 * Modify: 2018-02-18 20:53:04
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Logging.Infrastructure
{
    internal sealed class LogStorage
    {
        private const string FOLDER_NAME = "SevenTinyLogs";

        private static string SavePath = Path.Combine(AppContext.BaseDirectory, FOLDER_NAME);

        private static string FilePath = Path.Combine(SavePath, $"{DateTime.Now.Date.ToString("yyyyMMdd")}.log");

        public static LoggingConfig _LoggingConfig { get; set; }

        public static void Storage(LoggingLevel loggingLevel, string message)
        {
            if (LoggingConfigHelper.CheckLevelOpen(loggingLevel))
            {
                StorageForMedium(message);
            }
        }

        private static void StorageForMedium(string message)
        {
            ToFile(message);
        }

        /// <summary>
        /// storage to file
        /// </summary>
        /// <param name="message"></param>
        private static void ToFile(string message)
        {
            if (!string.IsNullOrEmpty(_LoggingConfig.Directory))
            {
                SavePath = _LoggingConfig.Directory;
            }
            FilePath = Path.Combine(SavePath, $"{DateTime.Now.Date.ToString("yyyyMMdd")}.log");
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
            using (Mutex myMutex = new Mutex(true, "log mutex lock"))
            {
                myMutex.WaitOne();
                try
                {
                    using (StreamWriter writer = new StreamWriter(FilePath, true))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine(message);
                    }
                }
                finally
                {
                    myMutex.ReleaseMutex();
                }
            }
        }
        /// <summary>
        /// storage to database
        /// </summary>
        /// <param name="message"></param>
        private static void ToDataBase(string message)
        {

        }
    }
}
