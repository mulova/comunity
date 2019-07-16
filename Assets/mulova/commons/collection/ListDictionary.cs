#if FULL
using System.Collections.Generic;
using System;

namespace commons {
	/// <summary>
	/// Ordered by the added order but faster search by key
	/// </summary>
	[System.Serializable]
	public class ListDictionary<T, U> {
		
		private U[] values;
		private Dictionary<T, int> indexMap;
		
		public ListDictionary() {
			values = new U[]{};
			indexMap = new Dictionary<T, int>();
		}
		
		public Dictionary<T, int> IndexMap {
			get {
				return indexMap;
			}
		}
		
		public U[] Values {
			get {
				return values;
			}
		}
		
		public int Count {
			get {
				return values.Length;
			}
		}
		
		public Dictionary<T, int>.KeyCollection Keys {
			get {
				return indexMap.Keys;
			}
		}
		
		public void Add(T key, U val) {
			int index = -1;
			if (indexMap.TryGetValue(key, out index)) {
				values[index] = val;
			} else {
				ArrayUtil.Add(ref values, val);
				indexMap.Add(key, values.Length-1);
			}
		}
		
		public void Remove(T key) {
			int index = -1;
			if (indexMap.TryGetValue(key, out index)) {
				indexMap.Remove(key);
				values = values.Remove(index);
			}
		}
		
		public void Remove(U val) {
			int index = -1;
			for (int i=0; i<values.Length; i++) {
				if (val.Equals(val)) {
					index = i;
					break;
				}
			}
			if (index > -1) {
				values = values.Remove(index);
				foreach (T key in indexMap.Keys) {
					if (indexMap[key] == index) {
						indexMap.Remove(key);
						break;
					}
				}
			}
		}
		
		public void Set(T key, U val) {
			int index = -1;
			if (indexMap.TryGetValue(key, out index)) {
				values[index] = val;
			} else {
				Add (key, val);
			}
		}
		
		public U Get(T key) {
			int index = -1;
			if (indexMap.TryGetValue(key, out index)) {
				return values[index];
			}
			return default(U);
		}
		
		public bool TryGetValue(T key, out U val) {
			int index = -1;
			if (indexMap.TryGetValue(key, out index)) {
				val = values[index];
				return true;
			} else {
				val = default(U);
			}
			return false;
		}
		
		public bool ContainsKey(T key) {
			int index = -1;
			return indexMap.TryGetValue(key, out index);
		}
		
		public void Clear() {
			indexMap.Clear();
			values = new U[]{};
		}
		
	}
}

#endif