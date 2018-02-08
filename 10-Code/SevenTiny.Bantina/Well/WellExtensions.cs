/*********************************************************
 * CopyRight: QIXIAO CODE BUILDER. 
 * Version: 5.0.0
 * Author: sevenTiny
 * Address: Earth
 * Create: 2017-12-05 00:11:52
 * Update: 2017-12-05 00:11:52
 * E-mail: dong@qixiao.me | wd8622088@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;

namespace SevenTiny.Bantina.Well
{
    /// <summary>
    /// Well Extensions,use for register service more sample
    /// </summary>
    public static class WellExtensions
    {
        /// <summary>
        /// register service lazy instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="well"></param>
        /// <returns></returns>
        public static IWell RegisterService<T>(this IWell well) where T : class
        {
            well.Register(new Lazy<T>(true).Value);//thread safe
            return well;
        }

        /// <summary>
        /// register service by T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="well"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IWell RegisterService<T>(this IWell well, T t) where T : class
        {
            well.Register<T>(t);
            return well;
        }

        /// <summary>
        /// register service by func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="well"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IWell RegisterService<T>(this IWell well,Func<T> func) where T : class
        {
            well.Register(new Lazy<T>(func, true).Value);//thread safe
            return well;
        }
    }
}
