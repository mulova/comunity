//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace convinity {

	[System.Serializable]
	public class GameObjectRefs {
		public string path;
		public List<CompRefs> refs = new List<CompRefs>();

		public GameObjectRefs() {}

		public GameObjectRefs(string path) {
			this.path = path;
		}

		public void AddRef(CompRefs r) {
			refs.Add(r);
		}

		public override string ToString ()
		{
			return path;
		}
	}
}