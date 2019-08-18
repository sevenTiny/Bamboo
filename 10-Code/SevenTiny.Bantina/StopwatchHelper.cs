/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-15 19:47:45
 * Modify: 2018-4-2 18:38:50
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;
using System.Diagnostics;

namespace SevenTiny.Bantina
{
    public abstract class StopwatchHelper
    {
        /// <summary>
        /// Caculate ExecuteTime
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TimeSpan Caculate(Action action)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            return sw.Elapsed;
        }
        /// <summary>
        /// Caculate ExecuteTime with Execute Times,add at:2018-4-2 18:38:42
        /// </summary>
        /// <param name="executTime"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TimeSpan Caculate(int executTimes, Action action)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (executTimes > 0)
            {
                action();
                executTimes--;
            }
            sw.Stop();
            return sw.Elapsed;
        }
    }
}
