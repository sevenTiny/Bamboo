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

        /// <summary>
        /// 安全获取字典数据的扩展方法
        /// 如果字典不存在key，那么返回默认值（默认值可以手动传递）
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="sourse"></param>
        /// <param name="key"></param>
        /// <param name="ifNotGetSetValue"></param>
        /// <returns></returns>
        public static TValue SafeGet<TKey, TValue>(this IDictionary<TKey, TValue> sourse, TKey key, TValue ifNotGetSetValue = default(TValue))
        {
            return sourse.TryGetValue(key, out TValue value) ? value : ifNotGetSetValue;
        }

        /// <summary>
        /// Contains Key，Continue的方式处理安全转化为字典
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TElement> SafeToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            if (source == null || !source.Any())
                return new Dictionary<TKey, TElement>(0);

            Dictionary<TKey, TElement> dic = new Dictionary<TKey, TElement>();

            foreach (var item in source)
            {
                var key = keySelector(item);

                if (dic.ContainsKey(key))
                    continue;

                dic.Add(key, elementSelector(item));
            }
            return dic;
        }
    }
}
