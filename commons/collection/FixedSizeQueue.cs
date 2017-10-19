#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace commons {
	public class FixedSizeQueue<T> : Queue<T>
	{
		public int Capacity { get; set; }
		private Queue<T> queue = new Queue<T>();
		
		public new int Count {
			get { return queue.Count; }
		}

		public FixedSizeQueue(int capacity) {
			Capacity = capacity;
		}
		
		public bool IsEmpty() {
			return queue.Count <= 0;
		}
		
		public new void Enqueue(T o) {
			if (queue.Count >= Capacity) {
				queue.Dequeue();
			}
			queue.Enqueue(o);
		}

		public void EnqueueRange(IEnumerable<T> range) {
			foreach (T t in range) {
				Enqueue(t);
			}
		}
		
		public new T Dequeue() {
			return queue.Dequeue();
		}
		
		public new void Clear() {
			queue.Clear();
		}
	}
}

#endif