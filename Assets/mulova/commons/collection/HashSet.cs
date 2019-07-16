//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections;

//namespace commons {
//	public class HashSet<T> : IEnumerable<T>
//	{
//		private Dictionary<T, bool> map = new Dictionary<T, bool>();
//		
//		public HashSet() {}
//		
//		public HashSet(IEnumerable<T> keys) {
//			Add(keys);
//		}
//		
//		public void Add(IEnumerable<T> keys) {
//			foreach (T t in keys) {
//				map[t] = true;
//			}
//		}
//		
//		public void Remove(IEnumerable<T> keys) {
//			foreach (T t in keys) {
//				map.Remove(t);
//			}
//		}
//		
//		public bool Contains(T t) {
//			return map.ContainsKey(t);
//		}
//		
//		public void Clear() {
//			map.Clear();
//		}
//		
//		public int Count {
//			get { return map.Count; }
//		}
//		
//#region IEnumerable
//		IEnumerator<T> IEnumerable<T>.GetEnumerator()
//		{
//			return map.Keys.GetEnumerator();
//		}
//		
//		IEnumerator IEnumerable.GetEnumerator()
//		{
//			return map.Keys.GetEnumerator();
//		}
//#endregion
//		
//	}	
//}
