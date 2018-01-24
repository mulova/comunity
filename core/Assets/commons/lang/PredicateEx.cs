//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Linq;
using System;

public static class PredicateEx {
	
	public static bool Call<T> (this Predicate<T> handler, T t) {
		if(handler != null) {
			return handler(t);
		}
		return false;
	}

	public static bool FreePassPredicate<T>(T t) {
		return true;
	}
}
