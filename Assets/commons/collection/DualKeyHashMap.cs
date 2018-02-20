//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace commons {
	/**
	 * 두개의 key를 사용한 HashMap
	 * @author mulova
	 *
	 */
	public class DualKeyHashMap<K1, K2, V> : Dictionary<K1, Dictionary<K2, V>> {
		
		public int TotalCount {
			get {
				int count = 0;
				foreach (K1 key in this.Keys) {
					Dictionary<K2, V> innerMap = null;
					if (TryGetValue(key, out innerMap) && innerMap != null) {
						count += innerMap.Count;
					}
				}
				return count;
			}
		}
		
		/**
		 * K1에 해당되는 모든 개체를 반환한다.
		 * @param key1
		 * @return
		 * @author mulova
		 */
		public new ICollection<V> Values(K1 key1) {
			Dictionary<K2, V> innerMap = this[key1];
			if (innerMap == null)
				return null;
			return innerMap.Values;
		}
		
		public ICollection<V> AllValues() {
			LinkedList<V> list = new LinkedList<V>();
			foreach (K1 key in this.Keys) {
				foreach (V v in Values(key)) {
					list.AddLast(v);
				}
			}
			return list;
		}
		
		public V Get(K1 key1, K2 key2) {
			V val = default(V);
			Dictionary<K2, V> innerMap = null;
			if (TryGetValue(key1, out innerMap) && innerMap != null) {
				innerMap.TryGetValue(key2, out val);
			}
			return val;
		}
		
		public void Put(K1 key1, K2 key2, V value) {
			Dictionary<K2, V> innerMap = null;
			if (!TryGetValue(key1, out innerMap)) {
				innerMap = new Dictionary<K2, V>();
				this[key1] = innerMap;
			}
			innerMap[key2] = value;
		}
	}
}