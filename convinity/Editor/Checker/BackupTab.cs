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
using mulova.commons;
using mulova.comunity;
using convinity;
using UnityEngine.Ex;

namespace mulova.comunity {
	class BackupTab : EditorTab {
		
		private static string backupName = "";
		private FileInfo fileToLoad;
        private MemberInfoRegistry registry = new MemberInfoRegistry(MemberInfoRegistryEx.ObjectRefFilter);
		private DirectoryInfo dir;
		private string sceneName;

		public BackupTab(TabbedEditorWindow window) : base("Backup", window) {
		}

		private DirectoryInfo GetDir() {
			string currentSceneName = EditorAssetUtil.GetCurrentScene();
			if (sceneName != currentSceneName) {
				dir = new DirectoryInfo(Application.dataPath+"/../Library/backup/"+EditorAssetUtil.GetCurrentScene());
				if (!dir.Exists) {
					dir.Create();
				}
				sceneName = currentSceneName;
			}
			return dir;
		}

		public override void OnEnable() { }
		public override void OnDisable() {}
		public override void OnSelected(bool sel) {}
		public override void OnFocus(bool focus) {}

		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}
		
		private GameObject root;
		
		public override void OnHeaderGUI() {
			GUI.enabled = true;
			FileInfo[] files = GetDir().GetFiles();
			if (files.Length > 0) {
				EditorGUILayout.BeginHorizontal();
				if (EditorGUILayoutUtil.PopupNullable("BackUp", ref fileToLoad, files, FileInfoToString)) {
					if (fileToLoad != null) {
						backupName = fileToLoad.Name;
						Load();
					} else {
						refDiffs.Clear();
					}
				}
				if (GUILayout.Button("Diff", GUILayout.Width(50))) {
					Diff();
				}
				if (GUILayout.Button("Apply", GUILayout.Width(50))) {
					Apply();
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		public override void OnInspectorGUI() {
			if (refDiffs != null) {
				EditorGUI.indentLevel++;
				EditorGUILayoutUtil.ObjectFieldList(refDiffs);
				EditorGUI.indentLevel--;
			}
		}	
		
		public override void OnFooterGUI() {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			if (EditorGUILayoutUtil.ObjectField<GameObject>("Root", ref root, true)) {
				if (root != null) {
					backup = SearchRefs();
				}
			}
			EditorGUILayoutUtil.TextField("Name", ref backupName);
			EditorGUILayout.EndVertical();
			GUI.enabled = !string.IsNullOrEmpty(backupName) && root != null;
			if (GUILayout.Button("Save", GUILayout.Width(50), GUILayout.Height(40))) {
				Save(backup);
			}
			EditorGUILayout.EndHorizontal();
		}

		private List<RefDiff> refDiffs = new List<RefDiff>();
		private List<GameObjectRefs> backup;
		private void Load() {
			ClearResults();
			refDiffs.Clear();
			BinarySerializer reader = new BinarySerializer(fileToLoad.FullName, FileAccess.Read);
			List<GameObjectRefs> backup = reader.Deserialize<List<GameObjectRefs>>();
			foreach (GameObjectRefs refs in backup) {
				Transform t = TransformUtil.Search(refs.path);
				if (t != null) {
					Component[] comps = t.GetComponents(typeof(Component));
					for (int i=0; i<comps.Length && i<refs.refs.Count; ++i) {
						Component c = comps[i];
						if (c == null) {
							continue;
						}
						CompRefs compRefs = refs.refs[i];
						if (compRefs.compType == c.GetType().FullName) {
							foreach (FieldInfo f in registry.GetFields(c.GetType())) {
								Object backupVal = compRefs.GetValue(f.Name);
								if (backupVal != null) {
									refDiffs.Add(new RefDiff(c, f, backupVal));
								}
							}
						}
					}
				}
			}
			reader.Close();
		}

		private void Diff() {
			List<RefDiff> newDiffs = new List<RefDiff>();
			foreach (RefDiff diff in refDiffs) {
				if (!diff.IsSame()) {
					newDiffs.Add(diff);
				}
			}
			refDiffs.Clear();
			refDiffs.AddRange(newDiffs);
		}

		private void Apply() {
			foreach (RefDiff diff in refDiffs) {
				diff.Apply();
			}
		}

		private List<GameObjectRefs> SearchRefs() {
			refDiffs.Clear();
			List<GameObjectRefs> result = new List<GameObjectRefs>();
			foreach (Transform t in root.GetComponentsInChildren<Transform>(true)) {
				GameObjectRefs goRefs = new GameObjectRefs(t.GetScenePath());
				Component[] comps = t.GetComponents(typeof(Component));
				for (int i=0; i<comps.Length; ++i) {
					Component c = comps[i];
					if (c != null) {
						CompRefs compRefs = new CompRefs(c.GetType(), i);
						foreach (FieldInfo f in registry.GetFields(c.GetType())) {
							Object val = f.GetValue(c) as Object;
							if (val != null) {
								compRefs.AddRef(f.Name, val);
								refDiffs.Add(new RefDiff(c, f, val));
							}
						}
						goRefs.AddRef(compRefs);
					} else {
						goRefs.AddRef(new CompRefs());
					}
				}
				result.Add(goRefs);
			}
			return result;
		}
		
		private void Save(List<GameObjectRefs> refs) {
			BinarySerializer writer = new BinarySerializer(GetPath(backupName), FileAccess.Write);
			Exception ex = writer.Serialize(refs);
			if (ex == null) {
				ex = writer.Close();
			} else {
				writer.Close();
			}
		}
		
		private string GetPath(string id) {
			return PathUtil.Combine(GetDir().FullName, id);
		}

		private string FileInfoToString(object o) {
			FileInfo f = o as FileInfo;
			if (f == null) {
				return "-";
			}
			return f.Name;
		}

		private string DrawRow(RefDiff row) {
			row.DrawGUI();
			return row.name;
		}
    }

    class RefDiff {
		public readonly Object obj;
		public readonly Component comp;
		public readonly FieldInfo field;
		public readonly string name;

		public RefDiff(Component c, FieldInfo f, Object o) {
			this.comp = c;
			this.field = f;
			this.obj = o;
			this.name = f.DeclaringType.FullName+"."+field.Name+ " in "+comp.transform.GetScenePath();
		}

		public bool IsSame() {
			return object.Equals(field.GetValue(comp), obj);
		}

		public void Apply() {
			if (!IsSame())  {
				field.SetValue(comp, obj);
			}
		}

		public void DrawGUI()  {
			EditorGUILayout.LabelField(name, EditorStyles.miniLabel);
			Type compType = comp != null? comp.GetType(): typeof(Component);
			Type objType = obj != null? obj.GetType(): typeof(Object);
			EditorGUILayout.ObjectField(comp, compType, true);
			EditorGUILayout.ObjectField(obj, objType, true);
		}
	}
}