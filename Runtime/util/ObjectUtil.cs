//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Reflection;
using Object = UnityEngine.Object;
using UnityEngine;

namespace mulova.comunity {
	public static class ObjectUtil {

		public static bool EqualsEx(object o1, object o2) {
			if (o1 == null ^ o2 == null) {
				return false;
			}
			if (o1 != null) {
				return o1.Equals(o2);
			}
			return true;
		}
	}
}
