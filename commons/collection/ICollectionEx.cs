//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;

public static class ICollectionEx {
	
	public static bool IsEmpty<T>(this ICollection<T> col) {
		if (col == null) {
			return true;
		}
		return col.Count <= 0;
	}

	public static bool IsNotEmpty<T>(this ICollection<T> col) {
		if (col == null) {
			return false;
		}
		return col.Count > 0;
	}

	public static bool Empty(this ICollection col) {
		if (col == null) {
			return true;
		}
		return col.Count <= 0;
	}
	
	public static bool NotEmpty(this ICollection col) {
		if (col == null) {
			return false;
		}
		return col.Count > 0;
	}
}