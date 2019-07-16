#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;

namespace commons
{
	public class ArrayPool<T>
	{
		private SortedMultiMap<long, T[]> pool = new SortedMultiMap<long, T[]>();
		private long maxSize;
		
		public ArrayPool(long maxSize) {
			this.maxSize = maxSize;
		}
		
		public void Release(ref T[] obj) {
			if (obj == null) {
				return;
			}
			for (long i=0; i<obj.Length; i++) {
				obj[i] = default(T);
			}
			pool.Add(obj.Length, obj);
			obj = null;
			
			if (pool.TotalCount > maxSize) {
				IEnumerator<long> i = pool.Keys.GetEnumerator();
				i.MoveNext();
				pool.Remove(i.Current);
			}
		}
		
		public T[] Get(long size) {
			List<T[]> slot = pool[size];
			if (slot == null || slot.Count == 0) {
				return new T[size];
			}
			T[] arr = slot[slot.Count-1];
			pool.Remove(size, arr);
			return arr;
		}
	}
}


#endif