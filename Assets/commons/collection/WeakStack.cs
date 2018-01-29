//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons {
	public class WeakStack<T> : IEnumerable<WeakReference<T>> where T : class
	{
		private bool strong;
		private Stack<WeakReference<T>> stack = new Stack<WeakReference<T>>();

		public int Count {
			get { return stack.Count; }
		}

		public bool Strong {
			get { return strong; }
			set {
				this.strong = value;
				foreach (WeakReference<T> w in stack) {
					w.Strong = value;
				}
			}
		}

		public WeakStack() {
		}

		public bool IsEmpty() {
			return stack.Count <= 0;
		}

		public bool Contains(T o) {
			return stack.Contains(WeakReference<T>.Create(o, strong));
		}

		public void Push(T o) {
			stack.Push(WeakReference<T>.Create(o, strong));
		}

		public T Pop() {
			while (stack.Count > 0) {
				WeakReference<T> r = stack.Pop();
				T t = r.Target;
				if (t != null) {
					return t;
				}
			}
			return null;
		}

		public T Peek() {
			while (stack.Count > 0) {
				WeakReference<T> r = stack.Peek();
				T t = r.Target;
				if (t != null) {
					return t;
				} else {
					stack.Pop();
				}
			}
			return null;
		}

		public void Clear() {
			stack.Clear();
		}

		public IEnumerator<WeakReference<T>> GetEnumerator()
		{
			return stack.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return stack.GetEnumerator();
		}
	}
}

