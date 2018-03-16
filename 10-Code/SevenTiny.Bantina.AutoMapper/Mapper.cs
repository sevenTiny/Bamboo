/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-03-16 10:11:43
 * Modify: 2018-03-16 10:11:43
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

namespace SevenTiny.Bantina.AutoMapper
{
    public sealed class Mapper
    {
        private Mapper() { }
        public static TValue AutoMapper<TValue, TSource>(TSource source) where TValue : class where TSource : class
        {
            //create instance of TValue
            TValue value = Activator.CreateInstance<TValue>();

            return default(TValue);
        }
    }
}
