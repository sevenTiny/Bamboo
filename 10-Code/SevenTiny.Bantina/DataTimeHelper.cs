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
    public static class DataTimeHelper
    {
        /// <summary>
        /// Current timestamp
        /// </summary>
        public static long TimestampNow => (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

    }
}
