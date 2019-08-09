using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Bantina.Extensions
{
    /// <summary>
    /// Collections Extensions
    /// 集合类型扩展方法
    /// </summary>
    public static class CollectionsExtensions
    {
        /// <summary>
        /// sample foreach
        /// </summary>
        /// <typeparam name="T">T type</typeparam>
        /// <param name="source">sorce</param>
        /// <param name="action">action for item T t</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        /// <summary>
        /// paging source collection to collection for collection
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sources">source collection</param>
        /// <param name="pageSize">page size</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<TResult>> Paging<TResult>(this IEnumerable<TResult> sources, int pageSize)
        {
            IList<IEnumerable<TResult>> result = new List<IEnumerable<TResult>>();
            int pageIndex = 0;
            int count = sources?.Count() ?? 0;
            while (count > (pageSize * pageIndex))
            {
                result.Add(sources.Skip(pageIndex * pageSize).Take(pageSize));
                pageIndex++;
            }
            return result;
        }
    }
}
