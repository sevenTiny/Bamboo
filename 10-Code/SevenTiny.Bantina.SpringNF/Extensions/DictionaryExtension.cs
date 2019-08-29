using System.Collections.Generic;

namespace SevenTiny.Bantina.Spring.Extensions
{
    internal static class DictionaryExtension
    {
        public static void AddOrUpdate<T, K>(this IDictionary<T, K> keyValuePairs, T key, K value)
        {
            if (keyValuePairs.ContainsKey(key))
                keyValuePairs[key] = value;
            else
                keyValuePairs.Add(key, value);
        }
    }
}
