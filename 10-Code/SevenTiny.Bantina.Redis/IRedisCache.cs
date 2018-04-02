/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-28 23:27:20
 * Modify: 2018-02-28 23:27:20
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;

namespace SevenTiny.Bantina.Redis
{
    public interface IRedisCache
    {
        string Get(string key);

        void Post(string key, string value);
        void Post(string key, string value,TimeSpan absoluteExpirationRelativeToNow);
        void Post(string key, string value, DateTime absoluteExpiration);
        void Put(string key, string value);
        void Delete(string key);
        bool Exist(string key);
    }
}
