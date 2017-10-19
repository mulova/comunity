//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace core {
	/// <summary>
	/// Store variables' reference values of a component
	/// </summary>
	[System.Serializable]
	public class CompRefs {
		public string compType;
		public int index;
		public Dictionary<string, BackupRef> refs = new Dictionary<string, BackupRef>();

		public CompRefs() {}

		public CompRefs(Type compType, int index) {
			this.compType = compType.FullName;
			this.index = index;
		}

		public void AddRef(string varName, Object val) {
			refs[varName] = new BackupRef(val);
		}

		public BackupRef GetValRef(string varName) {
			BackupRef r = null;
			if (refs.TryGetValue(varName, out r)) {
				return r;
			}
			return null;
		}

		public Object GetValue(string varName) {
			BackupRef r = GetValRef(varName);
			if (r != null) {
				return r.GetObject();
			}
			return null;
		}
	}
}