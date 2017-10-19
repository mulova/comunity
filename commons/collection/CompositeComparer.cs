//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections;

namespace commons {

	public class CompositeComparer : IComparer
	{
		private IComparer[] elements;
		public CompositeComparer(params IComparer[] elements) {
			this.elements = elements;
		}

		public int Compare(object x, object y)
		{
			foreach (IComparer c in elements) {
				int result = c.Compare(x, y);
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}
	}

	public class CompositeComparer<T> : System.Collections.Generic.IComparer<T>
	{
		private System.Collections.Generic.IComparer<T>[] elements;
		public CompositeComparer(params System.Collections.Generic.IComparer<T>[] elements) {
			this.elements = elements;
		}

		public int Compare(T x, T y)
		{
			foreach (System.Collections.Generic.IComparer<T> c in elements) {
				int result = c.Compare(x, y);
				if (result != 0) {
					return result;
				}
			}
			return 0;
		}
	}
	
}