//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using UnityEditor;

namespace comunity
{
	public static class PrefabTypeEx {
		public static bool IsValidPrefab(this PrefabType type) {
			switch (type) {
				case PrefabType.ModelPrefab:
				case PrefabType.ModelPrefabInstance:
				case PrefabType.Prefab:
				case PrefabType.PrefabInstance:
					return true;
				default:
					return false;
			}
		}
		
		public static bool IsMissingPrefab(this PrefabType type) {
			switch (type) {
				case PrefabType.DisconnectedModelPrefabInstance:
					return true;
				default:
					return false;
			}
		}
		
		public static bool IsPrefabInstance(this PrefabType type) {
			switch (type) {
				case PrefabType.DisconnectedModelPrefabInstance:
				case PrefabType.DisconnectedPrefabInstance:
				case PrefabType.MissingPrefabInstance:
				case PrefabType.ModelPrefabInstance:
				case PrefabType.PrefabInstance:
					return true;
				default:
					return false;
			}
		}
	}
}

