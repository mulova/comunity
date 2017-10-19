//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons {
	public class CollectionUtil
	{
		public static bool Equals<T>(List<T> src, List<T> dst) {
			if (src == null) {
				return dst == null;
			} else if (dst == null) {
				return false;
			}
			if (src.Count != dst.Count) {
				return false;
			}
			for (int i=0; i<src.Count; i++) {
				if (src[i]!=null ^ dst[i]!=null) {
					return false;
				} else if (src[i]!=null && !src[i].Equals(dst[i])) {
					return false;
				}
			}
			return true;
		}
		
		public static bool Equals<K, V>(Dictionary<K, V> src, Dictionary<K, V> dst)
		{
			if (src==null ^ dst==null) {
				return false;
			}
			if (src != null) {
				if (src.Count != dst.Count) {
					return false;
				}
				foreach (KeyValuePair<K, V> entry in src) {
					K k = entry.Key;
					V v1 = entry.Value;
					V v2 = default(V);
					if (!dst.TryGetValue(k, out v2)) {
						return false;
					}
					if (!v1.Equals(v2)) {
						return false;
					}
				}
			}
			return true;
		}
		
		public static Dictionary<K, V> DeepCopy<K, V>(Dictionary<K, V> original) where V : ICloneable
		{
			Dictionary<K, V> ret = new Dictionary<K, V>(original.Count, original.Comparer);
			foreach (KeyValuePair<K, V> entry in original)
			{
				ret.Add(entry.Key, (V) entry.Value.Clone());
			}
			return ret;
		}
	}
}


