/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-18 20:03:17
 * Modify: 2018-02-18 20:03:17
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;

namespace SevenTiny.Bantina.Logging
{
    public interface ILog
    {
        void Info(string message);
        void Debug(string message);
        void Warn(string message);
        void Error(string message);
        void Error(Exception exception);
        void Fatal(string message);
        void Fatal(Exception exception);
    }
}
