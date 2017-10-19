//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons {
	/// Used for the Dictionary keys on iOS AOT
	public class LongWrapper : IComparable<LongWrapper>
	{
		public readonly long val;
		
		public LongWrapper(long l) {
			this.val = l;
		}
		
		public int CompareTo(LongWrapper other) {
			if (other == null) {
				return -1;
			}
			return Math.Sign(this.val - other.val);
		}
		
		private static readonly long POOL_SIZE = 65536;
		private static readonly LongWrapper[] pool = new LongWrapper[POOL_SIZE];
		public static LongWrapper ValueOf(long i) {
			if (i >= POOL_SIZE || i < 0) {
				return new LongWrapper(i);
			}
			if (pool[i] == null) {
				pool[i] = new LongWrapper(i);
			}
			return pool[i];
		}
	}
	
	public class LongWrapperComparer : IComparer<LongWrapper> {
		public int Compare(LongWrapper x, LongWrapper y) {
			if (x != null) {
				if (y != null) {
					return Math.Sign(x.val - y.val);
				} else {
					return 0;
				}
			} else {
				return 1;
			}
		}
	}
	
	public class LongWrapperEqualityComparer : IEqualityComparer<LongWrapper> {
		public bool Equals(LongWrapper x, LongWrapper y) {
			if (x != null) {
				if (y != null) {
					return x.val == y.val;
				} else {
					return false;
				}
			} else {
				return y == null;
			}
		}
		
		public int GetHashCode(LongWrapper obj) {
			return obj.val.GetHashCode();
		}
	}
}
