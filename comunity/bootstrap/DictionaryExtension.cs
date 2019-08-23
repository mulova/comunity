using System;

namespace System.Collections.Generic
{
    public static class DictionaryExtension
    {
        public static V Find<K, V>(this IDictionary<K, V> dict, K key)
        {
            if (dict != null&&key != null)
            {
                V val = default(V);
                if (dict.TryGetValue(key, out val))
                {
                    return val;
                }
            }
            return default(V);
        }

        public static V Find<K, V>(this IDictionary<K, V> dict, K key, V defaultVal)
        {
            V val = defaultVal;
            if (dict != null&&key != null&&dict.TryGetValue(key, out val))
            {
                return val;
            }
            return defaultVal;
        }
    }
}

