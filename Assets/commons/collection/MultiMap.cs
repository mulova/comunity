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
	/// <summary>
	/// One key, multiple values
	/// </summary>
	[System.Serializable]
	public class MultiMap<K, V> : IEnumerable<KeyValuePair<K, List<V>>>
	{
		private Dictionary<K, List<V>> map;
		private int totalCount;
		
		public MultiMap()
		{
			map = new Dictionary<K, List<V>>();
		}
		
		public MultiMap(IEqualityComparer<K> comparer)
		{
			map = new Dictionary<K, List<V>>(comparer);
		}
		
		public List<V> this[K k] {
			get {
				return GetSlot(k);
			}
			set {
                var old = map.Get(k);
                if (old != null)
                {
                    totalCount -= old.Count;
                }
				map[k] = value;
                if (value != null)
                {
                    totalCount += value.Count;
                }
			}
		}
		
		public ICollection<K> Keys {
			get {
				return map.Keys;
			}
		}
		
		public void Add(K k, V v) {
			GetSlot(k).Add(v);
			totalCount++;
		}

        public void AddRange(K k, IEnumerable<V> v) {
            List<V> slot = GetSlot(k);
            int oldCount = slot.Count;
            slot.AddRange(v);
            totalCount += slot.Count-oldCount;
        }
		
		public void Remove(K k, V v) {
			if (GetSlot(k).Remove(v)) {
				totalCount--;
			}
		}

		public V RemoveOne(K k) {
			List<V> list = GetSlot(k);
			if (list.IsEmpty()) {
				return default(V);
			}
			V val = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
            totalCount--;
			return val;
		}
		
		public void RemoveAt(K k, int i) {
			GetSlot(k).RemoveAt(i);
			totalCount--;
		}
		
		public void Remove(K k) {
			List<V> slot = GetSlot(k);
			totalCount -= slot.Count;
			map.Remove(k);
		}

		public void Clear() {
			RemoveAll();
		}
		
		public void RemoveAll() {
			foreach (KeyValuePair<K, List<V>> pair in map) {
				pair.Value.Clear();
			}
			totalCount = 0;
		}

		/// <summary>
		/// Gets the slot.
		/// </summary>
		/// <returns>The slot. Non-null</returns>
		/// <param name="k">K.</param>
		public List<V> GetSlot(K k) {
			List<V> list = null;
			if (!map.TryGetValue(k, out list)) {
				list = new List<V>();
				map[k] = list;
			}
			return list;
		}

		public bool ContainsKey(K k) {
			return map.ContainsKey(k);
		}

		public int GetCount(K k) {
			List<V> slot = GetSlot(k);
			if (slot == null) {
				return 0;
			}
			return slot.Count;
		}

		public bool IsEmpty(K k) {
			return GetCount(k) == 0;
		}

		public bool IsNotEmpty(K k) {
			return GetCount(k) != 0;
		}

		public bool Contains(K k, V v) {
			List<V> slot = GetSlot(k);
			if (slot == null) {
				return false;
			}
			return slot.Contains(v);
		}

		/// <summary>
		/// The size of map's key
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public int Count {
			get { return map.Count; }
		}
		
		/// <summary>
		/// The count of map values
		/// </summary>
		/// <value>
		/// The total count.
		/// </value>
		public int TotalCount {
			get {
				return totalCount;
			}
		}
		
		#region IEnumerable
		IEnumerator<KeyValuePair<K, List<V>>> IEnumerable<KeyValuePair<K, List<V>>>.GetEnumerator()
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
