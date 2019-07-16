//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons {
	/// Used for the Dictionary keys on iOS AOT
	public class IntWrapper : IComparable<IntWrapper>, IEquatable<IntWrapper>
	{
		public readonly int i;
		
		public IntWrapper(int i) {
			this.i = i;
		}
		
		public int CompareTo(IntWrapper that) {
			if (that == null) {
				return -1;
			}
			return this.i - that.i;
		}

		public bool Equals(IntWrapper that) {
			if (that != null) {
				return i == that.i;
			} else {
				return false;
			}
		}

		public override int GetHashCode ()
		{
			return i;
		}

		public static explicit operator IntWrapper(int i) {
			return ValueOf(i);
		}

		public static explicit operator int(IntWrapper i) {
			return i.i;
		}

		public override string ToString ()
		{
			return i.ToString();
		}
		
		private static readonly int POOL_SIZE = 65536;
		private static readonly IntWrapper[] pool = new IntWrapper[POOL_SIZE];
		public static IntWrapper ValueOf(int i) {
			if (i >= POOL_SIZE || i < 0) {
				return new IntWrapper(i);
			}
			if (pool[i] == null) {
				pool[i] = new IntWrapper(i);
			}
			return pool[i];
		}
	}
	
	public class IntWrapperComparer : IComparer<IntWrapper> {
		public int Compare(IntWrapper x, IntWrapper y) {
			if (x != null) {
				if (y != null) {
					return x.i - y.i;
				} else {
					return 0;
				}
			} else {
				return 1;
			}
		}
	}
	
	public class IntWrapperEqualityComparer : IEqualityComparer<IntWrapper> {
		public bool Equals(IntWrapper x, IntWrapper y) {
			if (x != null) {
				if (y != null) {
					return x.i == y.i;
				} else {
					return false;
				}
			} else {
				return y == null;
			}
		}
		
		public int GetHashCode(IntWrapper obj) {
			return obj.i;
		}
	}

	public class IntEqualityComparer : IEqualityComparer<int> {
		bool IEqualityComparer<int>.Equals(int x, int y)
		{
			return x == y;
		}
		int IEqualityComparer<int>.GetHashCode(int i)
		{
			return i;
		}
	}
}
