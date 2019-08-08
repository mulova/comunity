//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using Object = UnityEngine.Object;
using System;

namespace comunity
{
	public class ToStringFilter {
		private ToString toString;
		private string[] filters = new string[0];
		
		public ToStringFilter(ToString toStr, string filter) {
			this.toString = toStr!=null? toStr: ObjToString.ScenePathToString;
			SetFilter(filter);
		}
		
		public void SetFilter(string filter) {
			if (filter != null) {
				this.filters = filter.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			}
		}
		
		public bool Filter(object obj)  {
			string name = toString(obj);
			foreach (string f in filters) {
				if (name.IndexOf(f, StringComparison.OrdinalIgnoreCase)<0) {
					return false;
				}
			}
			return true;
		}
		
		public bool Filter<T>(T t)  {
			return Filter((object)t);
		}
	}
}

