/*********************************************************
 * CopyRight: QIXIAO CODE BUILDER. 
 * Version: 5.0.0
 * Author: sevenTiny
 * Address: Earth
 * Create: 2017-12-03 21:21:02
 * Update: 2017-12-03 21:21:02
 * E-mail: dong@qixiao.me | wd8622088@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/

namespace SevenTiny.Bantina.IOC
{
    /// <summary>
    /// object inject type when well starting
    /// </summary>
    public enum InjectType
    {
        /// <summary>
        /// inject when well start
        /// </summary>
        Initialize = 0,
        /// <summary>
        /// lazy load when resolve
        /// </summary>
        LazyLoad = 1
    }
}
