//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;

namespace comunity {
	public class Dummy
	{
		private static GameObject dummyObject;
		private static Transform trans;
		
		public static GameObject GameObject {
			get {
				if (dummyObject == null) {
					dummyObject = new GameObject("Dummy");
					dummyObject.AddComponent<MeshRenderer>();
				}
				return dummyObject;
			}
		}
		
		public static Transform transform {
			get {
				if (trans == null) {
					trans = GameObject.transform;
				}
				return trans;
			}
		}
	}
}

