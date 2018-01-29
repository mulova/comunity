//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    public static class IDictionaryEx
    {
        public static V Get<K, V>(this IDictionary<K, V> dict, K key) {
            if (dict != null && key != null)
            {
                V val = default(V);
                if (dict.TryGetValue(key, out val))
                {
                    return val;
                }
            }
            return default(V);
        }
        
        public static V Get<K, V>(this IDictionary<K, V> dict, K key, V defaultVal) {
            V val = defaultVal;
            if (dict != null && key != null && dict.TryGetValue(key, out val)) {
                return val;
            }
            return defaultVal;
        }
    }
}

