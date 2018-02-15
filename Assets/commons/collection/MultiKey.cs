//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections;

namespace commons {
	public class MultiKey<K1, K2> {
		public readonly K1 key1;
		public readonly K2 key2;

		public MultiKey(K1 k1, K2 k2) {
			this.key1 = k1;
			this.key2 = k2;
		}

		public override bool Equals(System.Object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj) {
				return true;
			}
			if (obj is MultiKey<K1, K2>) {
				MultiKey<K1, K2> that = obj as MultiKey<K1, K2>;
				if (this.key1.Equals(that.key1) && this.key2.Equals(that.key2)) {
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hash = 0;
			if (key1 != null) {
				hash = key1.GetHashCode();
			}
			if (key2 != null) {
				hash ^= key2.GetHashCode();
			}
			return hash;
		}
	}
}
