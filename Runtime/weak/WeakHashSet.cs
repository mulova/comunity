//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;

namespace mulova.commons
{
    /// <summary>
    /// Weak hash set.
    /// </summary>
    public class WeakHashSet<T> where T:class {
		private WeakValueDictionary<int, T> store1 = new WeakValueDictionary<int, T>();
		private WeakValueDictionary<int, List<T>> store2 = new WeakValueDictionary<int, List<T>>();
		
		public int Count {
			get {
				int count = store1.Count;
				foreach (KeyValuePair<int, List<T>> slot in store2) {
					count+= slot.Value.Count;
				}
				return count;
			}
		}
		
		/// <summary>
		/// return false if the same object exists already
		/// </summary>
		/// <param name="obj">Object.</param>
		public bool Add(T obj) {
			int key = obj.GetHashCode();
			T existing = store1[key];
			if (existing == null) {
				store1[key] = obj;
			} else {
				if (existing.Equals(obj)) {
					return false;
				} else {
					List<T> conflict = store2[key];
					if (conflict == null) {
						conflict = new List<T>();
						store2[key] = conflict;
					} else {
						foreach (T c in conflict) {
							if (c.Equals(obj)) {
								return false;
							}
						}
					}
					conflict.Add(obj);
				}
			}
			return true;
		}
		
		public bool Remove(T obj) {
			int key = obj.GetHashCode();
			if (store1[key] == null) {
				return false;
			}
			List<T> conflict = store2[key];
			if (store1[key] == obj) {
				if (conflict != null) {
					store1[key] = conflict[conflict.Count-1];
					if (conflict.Count == 1) {
						store2.Remove(key);
					} else {
						conflict.RemoveAt(conflict.Count-1);
					}
				} else {
					store1.Remove(key);
				}
				return true;
			} else {
				if (conflict == null) {
					return false;
				}
				for (int i=0; i<conflict.Count; ++i) {
					T c = conflict[i];
					if (c == obj) {
						if (conflict.Count == 1) {
							store2.Remove(key);
						} else {
							conflict.RemoveAt(i);
						}
						return true;
					}
				}
				return false;
			}
		}
		
		public bool Contains(T obj) {
			int key = obj.GetHashCode();
			T val = store1[key];
			if (val == null) {
				return false;
			} else {
				if (val == obj) {
					return true;
				}
				List<T> conflict = store2[key];
				if (conflict == null) {
					return false;
				}
				foreach (T c in conflict) {
					if (c == obj) {
						return true;
					}
				}
				return false;
			}
		}
		
		public void CleanDeadReferences() {
			store1.CleanDeadReferences();
			store2.CleanDeadReferences();
		}
		
		public void Clear() {
			store1.Clear();
			store2.Clear();
		}
	}
}
