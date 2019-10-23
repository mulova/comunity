//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace mulova.commons {
	public class WeakQueue<T> where T: class
	{
		private bool strong;
		private Queue<WeakReference<T>> queue = new Queue<WeakReference<T>>();

		public int Count {
			get { return queue.Count; }
		}

		public bool Strong {
			get { return strong; }
			set {
				this.strong = value;
				foreach (WeakReference<T> w in queue) {
					w.Strong = value;
				}
			}
		}

		public WeakQueue()
		{
		}

		public bool IsEmpty() {
			return queue.Count <= 0;
		}

		public void Enqueue(T o) {
			queue.Enqueue(WeakReference<T>.Create(o, strong));
		}

		public T Dequeue() {
			while (queue.Count > 0) {
				WeakReference<T> r = queue.Dequeue();
				T t = r.Target;
				if (t != null) {
					return t;
				}
			}
			return null;
		}

		public void Clear() {
			queue.Clear();
		}

		public void ForEach(Action<T> action)
		{
			foreach (WeakReference<T> r in queue)
			{
				T t = r.Target;
				if (t != null)
				{
					action(t);
				}
			}
		}

		public void CleanDeadReferences() {
			Queue<WeakReference<T>> queue2 = new Queue<WeakReference<T>>();
			foreach (WeakReference<T> r in queue) {
				if (r.Target != null) {
					queue2.Enqueue(r);
				}
			}
			queue = queue2;
		}
	}
}

