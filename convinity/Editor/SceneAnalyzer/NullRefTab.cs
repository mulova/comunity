//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Text;
using System.Reflection;
using Nullable = mulova.commons.Nullable;
using mulova.commons;
using comunity;
using UnityEngine.Ex;

namespace convinity {
	/// <summary>
	/// Find the null Object references 
	/// </summary>
	class NullRefTab : EditorTab {
		public readonly string EXCLUDE_DESC = "inspector_settings/null_ref_exclusion.bytes";
        private MemberInfoRegistry registry = new MemberInfoRegistry(MemberInfoRegistryEx.ObjectRefFilter);
		private static TextAsset excludeDesc;

		public NullRefTab(TabbedEditorWindow window) : base("Null Ref", window) {}
		
		public override void OnEnable() {
			if (excludeDesc == null) {
				excludeDesc = AssetDatabase.LoadAssetAtPath<TextAsset>(EXCLUDE_DESC);
				SetExcludes();
			}
		}
		public override void OnDisable() {}
		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		
		private Object root;

		private List<NullRefData> nullRefs = new List<NullRefData>();
		public override void OnHeaderGUI() {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			if (EditorGUILayoutUtil.ObjectField<Object>("Root", ref root, true)) {
				SearchNullRefs();
			}
			if (EditorGUILayoutUtil.ObjectField<TextAsset>("Exclude", ref excludeDesc, false)) {
				SetExcludes();
				SearchNullRefs();
			}
			EditorGUILayout.EndVertical();
			if (GUILayout.Button("Search", GUILayout.Height(30))) {
				SetExcludes();
				SearchNullRefs();
			}
			EditorGUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI() {
			var drawer = new ListDrawer<NamedObj>(nullRefs.ConvertAll(t=> (NamedObj)t), new NamedObjDrawer<NamedObj>());
			drawer.Draw();
		}
		
		public override void OnFooterGUI() {
		}

		public override void OnFocus(bool focus) {}
		public override void OnSelected(bool sel) {}

		private void SetExcludes() {
			registry = new MemberInfoRegistry(MemberInfoRegistryEx.ObjectRefFilter);
			if (excludeDesc != null) {
				foreach (string l in excludeDesc.text.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries)) {
					string line = l.Trim();
					if (line.StartsWith("#")) {
						continue;
					}
					int index = line.Trim().LastIndexOf('.');
					if (index > 0) {
						string typeName = line.Substring(0, index);
						string varName = line.Substring(index+1, line.Length-(index+1));
						registry.ExcludeField(typeName, varName);
					}
				}
			}
		}
		
		private void SearchNullRefs() {
			nullRefs = new List<NullRefData>();
			if (root != null) {
				if (root is GameObject) {
					foreach (Transform t in(root as GameObject).GetComponentsInChildren<Transform>(true)) {
						Component[] comps = t.GetComponents(typeof(Component));
						for (int i=0; i<comps.Length; ++i) {
							Component c = comps[i];
							SearchNullRef(c);
						}
					}
				} else if (root is Component) {
					SearchNullRef(root as Component);
				} else if (AssetDatabase.IsMainAsset(root)) {
					Object[] assets = EditorAssetUtil.ListAssets<Object>(AssetDatabase.GetAssetPath(root), FileType.All);
					foreach (Object a in assets) {
						SearchNullRef(a);
					}
				}
			}
		}

		private void SearchNullRef(Object obj)
		{
			if (obj == null) {
				return;
			}
			foreach (FieldInfo f in registry.GetFields(obj.GetType())) {
				if (ReflectionUtil.GetAttribute<Nullable>(f) == null) {
					object o = f.GetValue(obj);
					if (o.IsNull()) {
						nullRefs.Add(new NullRefData(obj, f));
						break;
					}
				}
			}
			
			if (obj is Material) {
				Material mat = obj as Material;
				if (mat.mainTexture == null) {
					nullRefs.Add(new NullRefData(obj, null));
				}
			}
		}
	}

	public class NullRefData : NamedObj {
		public Object obj;
		public FieldInfo field;
		private string str;

		public NullRefData(Object c, FieldInfo f) {
			this.obj = c;
			this.field = f;
			if (f != null) {
				this.str = c.GetType().FullName+"."+f.Name;
			} else {
				this.str = c.GetType().FullName+".?";
			}
		}

		public override string ToString() {
			return str;
		}

		public Object Obj { get { return obj;} }
		public string Name { get { return str;} }
	}
}