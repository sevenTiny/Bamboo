/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-20 17:05:42
 * Modify: 2018-04-20 17:05:42
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using cache = SevenTiny.Bantina.Bankinate.MemoryCacheHelper;

namespace SevenTiny.Bantina.Bankinate
{
    /**
     * The cache memory1,quick memory value from sql statement.
     * */
    internal class MemoryCache
    {
        const string MCTable = "BankinateCache_Table_";
        //cache level 2 key prefix
        const string MC2 = "BankinateCache_CM2_";

        public static TResult GetInCacheIfNotExistReStore<TResult>(string tableName,string sqlstatement, Func<TResult> func)
        {
            //check if table data has be changed
            string mcTableKey = $"{MCTable}{tableName}";
            int key = sqlstatement.GetHashCode();
            TResult result;

            if (cache.Exist(mcTableKey))
            {
                result = func();
                cache.Put(key, result);
                cache.Delete(mcTableKey);
            }
            else
            {
                if (cache.Exist(key))
                {
                    result = cache.Get<object, TResult>(key);
                }
                else
                {
                    result = func();
                    cache.Put(key, result);
                }
            }
            
            return result;
        }
    }
}
