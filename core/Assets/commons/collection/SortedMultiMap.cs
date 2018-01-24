//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace commons {
	[System.Serializable]
	public class SortedMultiMap<K, V> : IEnumerable<KeyValuePair<K, List<V>>> where K: IComparable<K>
	{
		private SortedDictionary<K, List<V>> map;
		private int totalCount;
		
		public SortedMultiMap()
		{
			map = new SortedDictionary<K, List<V>>();
		}
		
		public SortedMultiMap(IComparer<K> comparer)
		{
			map = new SortedDictionary<K, List<V>>(comparer);
		}
		
		public List<V> this[K k] {
			get {
				return GetSlot(k);
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
		
		public void Remove(K k, V v) {
			if (GetSlot(k).Remove(v)) {
				totalCount--;
			}
		}

		public bool ContainsKey(K k) {
			return map.ContainsKey(k);
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
		
		public void RemoveAll() {
			foreach (KeyValuePair<K, List<V>> pair in map) {
				pair.Value.Clear();
			}
			totalCount = 0;
		}
		
		private List<V> GetSlot(K k) {
			List<V> list = null;
			if (!map.TryGetValue(k, out list)) {
				list = new List<V>();
				map[k] = list;
			}
			return list;
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
//				if (Build.isEditor) {
//					int count = 0;
//					foreach (KeyValuePair<K, List<V>> pair in map) {
//						count += pair.Value.Count;
//					}
//					core.Assert.AreEqual(totalCount, count);
//				}
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
