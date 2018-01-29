//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace System.Collections.Generic
{
	public static class HashSetEx
	{
		public static void AddAll<T>(this HashSet<T> hashSet, IEnumerable<T> objs) {
			foreach (T t in objs) {
				hashSet.Add(t);
			}
		}

		public static void RemoveAll<T>(this HashSet<T> hashSet, IEnumerable<T> objs) {
			foreach (T t in objs) {
				hashSet.Remove(t);
			}
		}

	}
}

