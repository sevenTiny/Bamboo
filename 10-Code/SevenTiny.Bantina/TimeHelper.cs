/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-20 10:37:51
 * Modify: 2018-04-20 10:37:51
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;

namespace SevenTiny.Bantina
{
    public static class TimeHelper
    {
        /// <summary>
        /// convert timestamp to datetime
        /// </summary>
        /// <param name="timestamp">unix timestamp length 13</param>
        /// <returns>datetime</returns>
        public static DateTime GetDateTime(long timestamp)
        {
            return TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local).Add(new TimeSpan(timestamp * 10000));
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <param name="time">时间，如不填默认当前时刻</param>
        /// <returns></returns>
        public static long GetTimeStamp(DateTime time = default(DateTime))
        {
            if (DateTime.Equals(time, default(DateTime)))
                time = DateTime.UtcNow;

            TimeSpan ts = time - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
    }
}
