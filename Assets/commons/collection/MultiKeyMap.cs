//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic.Ex;

namespace commons {
	[System.Serializable]
	public class MultiKeyMap<K1, K2, V> : IEnumerable<KeyValuePair<K1, Dictionary<K2, V>>>
	{
		private Dictionary<K1, Dictionary<K2, V>> map;
		
		public MultiKeyMap()
		{
			map = new Dictionary<K1, Dictionary<K2, V>>();
		}
		
		public IDictionary<K2, V> this[K1 k] {
			get {
				return GetSlot(k);
			}
		}

		public V this[K1 k1, K2 k2] {
			get {
				return GetSlot(k1).Get(k2);
			}
			set {
				IDictionary<K2, V> slot = GetSlot(k1);
				slot[k2] = value;
			}
		}
		
		public ICollection<K1> Keys {
			get {
				return map.Keys;
			}
		}
		
		public void Add(K1 k1, K2 k2, V val) {
			GetSlot(k1)[k2] = val;
		}
		
		public void Remove(K1 k, K2 k2) {
			GetSlot(k).Remove(k2);
		}
		
		public void Remove(K1 k) {
			map.Remove(k);
		}
		
		public V Get(K1 k, K2 k2) {
			IDictionary<K2, V> slot = GetSlot(k);
			if (slot == null) {
				return default(V);
			}
			return slot.Get(k2);
		}

		public bool Contains(K1 k, K2 k2) {
			IDictionary<K2, V> slot = GetSlot(k);
			if (slot == null) {
				return false;
			}
			return slot.ContainsKey(k2);
		}

		public bool TryGetValue(K1 k, K2 k2, out V v) {
			IDictionary<K2, V> slot = GetSlot(k);
			if (slot != null) {
				if (slot.ContainsKey(k2)) {
					v = slot.Get(k2);
					return true;
				}
			}
			v = default(V);
			return false;
		}

		public void Clear() {
			foreach (KeyValuePair<K1, Dictionary<K2, V>> pair in map) {
				pair.Value.Clear();
			}
		}
		
		public IDictionary<K2, V> GetSlot(K1 k1) {
			Dictionary<K2, V> slot = null;
			if (!map.TryGetValue(k1, out slot)) {
				slot = new Dictionary<K2, V>();
				map[k1] = slot;
			}
			return slot;
		}
		
		public int Count {
			get { return map.Count; }
		}
		
		#region IEnumerable
		IEnumerator<KeyValuePair<K1, Dictionary<K2, V>>> IEnumerable<KeyValuePair<K1, Dictionary<K2, V>>>.GetEnumerator()
		{
			return map.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return map.GetEnumerator();
		}
#endregion
		
	}	
}
