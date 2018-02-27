/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-27 13:13:38
 * Modify: 2018-02-27 13:13:38
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/

namespace SevenTiny.Bantina.Cache
{
    public interface ICache
    {
        TValue Get<TValue>(int key);
        TValue Get<TValue>(string key);

        /// <summary>
        /// Post can do put
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        TValue Post<TKey, TValue>(TKey key, TValue value);

        TValue Put<TKey, TValue>(TKey key, TValue value);

        void Delete<TKey>(TKey key);

        void Clear();

        int Count();
    }
}
