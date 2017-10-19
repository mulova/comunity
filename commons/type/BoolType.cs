//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

/// <summary>
/// Nullabel bool type
/// </summary>
public enum BoolType {
	Null,
	True,
	False
}


public static class BoolTypeEx {
	public static BoolType Parse(string str) {
		if (str == null) {
			return BoolType.False;
		}
		str = str.Trim();
        return (string.Equals("yes", str, StringComparison.OrdinalIgnoreCase)
                || string.Equals("o", str, StringComparison.OrdinalIgnoreCase)
                || string.Equals("true", str, StringComparison.OrdinalIgnoreCase))
                .GetBoolType();
	}

	public static BoolType Parse(bool b) {
		return b? BoolType.True: BoolType.False;
	}
	
	public static bool IsTrue(this BoolType b) {
		return b == BoolType.True;
	}

	public static BoolType GetBoolType(this bool b) {
		return b? BoolType.True: BoolType.False;
	}

	public static BoolType Or(this BoolType b1, bool b2) {
		return BoolTypeEx.Parse(b1.IsTrue() || b2);
	}

	public static BoolType And(this BoolType b1, bool b2) {
		return BoolTypeEx.Parse(b1.IsTrue() && b2);
	}
}
