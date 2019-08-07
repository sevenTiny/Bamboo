using System;
using System.Collections.Generic;

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
    }
}
