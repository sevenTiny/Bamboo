/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Pekin
* Create: 9/28/2018, 3:32:19 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 
* Thx , Best Regards ~
*********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Bantina
{
    public static class PagingHelper
    {
        /// <summary>
        /// 对有参数限制的方法进行参数分页
        /// </summary>
        /// <typeparam name="TSource">待分页资源类型</typeparam>
        /// <typeparam name="TResult">方法返回值资源类型</typeparam>
        /// <param name="func">执行方法体</param>
        /// <param name="sources">全部待分页参数</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static IEnumerable<TResult> ArgumentsPaging<TSource, TResult>(Func<IEnumerable<TSource>, IEnumerable<TResult>> func, IEnumerable<TSource> sources, int pageSize)
        {
            List<TResult> result = new List<TResult>();
            int pageIndex = 0;
            int count = sources?.Count() ?? 0;
            while (count > (pageSize * pageIndex))
            {
                result.AddRange(func(sources.Skip(pageIndex * pageSize).Take(pageSize)));
                pageIndex++;
            }
            return result;
        }
    }
}