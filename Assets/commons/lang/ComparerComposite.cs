//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System.Collections.Generic;
using System.Collections;

namespace commons
{
	public class ComparerComposite<T> : IComparer<T> {
		private IEnumerable<IComparer<T>> comparers;
		
		public ComparerComposite(IEnumerable<IComparer<T>> comparers) {
			this.comparers = comparers;
		}
		
		public ComparerComposite(params IComparer<T>[] comparers) {
			this.comparers = comparers;
		}
		
		public int Compare(T x, T y)
		{
			if (x == null) {
				return 1;
			}
			if (y == null) {
				return -1;
			}
			foreach (IComparer<T> c in comparers) {
				int i = c.Compare(x, y);
				if (i != 0) {
					return i;
				}
			}
			return 0;
		}
	}
	
	public class ComparerComposite : IComparer {
		private IEnumerable<IComparer> comparers;
		
		public ComparerComposite(IEnumerable<IComparer> comparers) {
			this.comparers = comparers;
		}
		
		public ComparerComposite(params IComparer[] comparers) {
			this.comparers = comparers;
		}
		
		public int Compare(object x, object y)
		{
			if (x == null) {
				return 1;
			}
			if (y == null) {
				return -1;
			}
			foreach (IComparer c in comparers) {
				int i = c.Compare(x, y);
				if (i != 0) {
					return i;
				}
			}
			return 0;
		}
	}
}
