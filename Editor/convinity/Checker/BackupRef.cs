//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using System;
using System.Ex;
using System.Text;
using mulova.commons;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace convinity
{
    /// <summary>
    /// path only: GameObject
    /// path, comp: Component
    /// path, comp, varName: Component Value
    /// </summary>
    [System.Serializable]
	public class BackupRef {
		public bool asset;
		private string objId;
		public string comp;
		public string varName;
		
		public BackupRef() {
		}
		
		public BackupRef(Object o) {
			if (o != null) {
				this.objId = EditorAssetUtil.GetObjectId(o, out asset);
				if (o is Component) {
					comp = o.GetType().FullName;
				}
			}
		}
		
		public BackupRef(Component c, string variableName) : this(c.gameObject) {
			this.comp = c.GetType().FullName;
			this.varName = variableName;
		}
		
		public Object GetObject() {
			if (objId == null) {
				return null;
			}
			Object o = EditorAssetUtil.GetObject(objId);
			if (o == null) {
				return null;
			}
			if (!string.IsNullOrEmpty(comp) && o is GameObject) {
				Type type = TypeEx.GetType(comp);
				if (type != null) {
					Component c = (o as GameObject).GetComponent(type);
					return c;
				}
			}
			return o;
		}
		
		public override string ToString() {
			StringBuilder str = new StringBuilder();
			if (asset) {
				str.Append(AssetDatabase.GUIDToAssetPath(objId));
			} else {
				str.Append(objId);
			}
			if (!string.IsNullOrEmpty(comp)) {
				str.Append('.').Append(comp);
			}
			if (!string.IsNullOrEmpty(varName)) {
				str.Append('.').Append(varName);
			}
			return str.ToString();
		}
	}
}