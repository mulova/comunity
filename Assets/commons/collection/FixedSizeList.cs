//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons {
	public class FixedSizeList<T> : LinkedList<T>
	{
		private int capacity;

		public FixedSizeList(int capacity) {
			this.capacity = capacity;
		}

		public void AddFirstEx(T t) {
			if (capacity == this.Count) {
				RemoveLast();
			}
			base.AddFirst(t);
		}

		public void AddLastEx(T t) {
			if (capacity == this.Count) {
				RemoveFirst();
			}
			base.AddLast(t);
		}

		public void AddFirstEx(IList<T> list) {
			for (int i=list.Count-1; i>=0; --i) {
				AddFirstEx(list[i]);
			}
		}
		
		public void AddLastEx(IList<T> list) {
			foreach (T t in list) {
				AddLastEx(t);
			}
		}

		public bool IsFull() {
			return this.capacity == base.Count;
		}
	}
}

