using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using UnityEditorInternal;
using commons;
using comunity;
using Rotorz.Games.Collections;

namespace shortcut {

	public class ShortcutTab : EditorTab {
		
		private ShortcutWindowInfo info = new ShortcutWindowInfo();
		private List<ShortcutSection> sections = new List<ShortcutSection>();
		private string dir;
		
		public ShortcutTab(object id, string dir, TabbedEditorWindow window) : base(id, window) {
			this.dir = dir;
		}

        private ObjListFilter<UnityObjId> assetFilter;
        private ObjListFilter<UnityObjId> sceneFilter;
		public override void OnEnable() {
			OnFocus(true);
			LoadInfo();
			sections = LoadShortcutSections();
            assetFilter = new ObjListFilter<UnityObjId>("Assets", true, null);
            sceneFilter = new ObjListFilter<UnityObjId>("Scene Objects", false, null);
		}

		public override void OnDisable() { }
		public override void OnSelected(bool sel) {}
		public override void OnFocus(bool focus) {}

		public override void OnChangePlayMode(PlayModeStateChange stateChange) {
			OnChangeScene(null);
		}

		public override void OnChangeScene(string sceneName) {
			foreach (ShortcutSection s in sections) {
				s.ClearCache();
			}
			Repaint();
		}
		
		public override void OnInspectorUpdate() {
		}

		protected override void Repaint() {
			base.Repaint();
			pathMap.Clear();
		}
		
		private void LoadInfo() {
			string infoPath = PathUtil.Combine(dir, "shortcut_window.info");
			info = SerializationUtil.ReadObject<ShortcutWindowInfo>(infoPath);
			if (info == null) {
				info = new ShortcutWindowInfo();
			}
		}
		
		private void SaveInfo() {
			string infoPath = PathUtil.Combine(dir, "shortcut_window.info");
			info.Set(sections);
			SerializationUtil.WriteObject<ShortcutWindowInfo>(infoPath, info);
		}
		
		private ShortcutSection GetSection(string name) {
			foreach (ShortcutSection s in sections) {
				if (s.name == name) {
					return s;
				}
			}
			return null;
		}
		
		private static int sectionCount = 1;
		
		private Vector2 mousePosition;
		
		private void OnSectionGUI(int i, ShortcutSection sect, bool contents) {
			bool changed = false;
			GUI.enabled = true;
			if (contents) {
				EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(sect.name, EditorStyles.boldLabel);
				if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) && EditorUtility.DisplayDialog("Warning", "Remove "+sect.name, "OK", "Cancel")) {
					sect.Delete();
					sections.Remove(sect);
					SaveInfo();
					changed = true;
				}
				EditorGUILayout.EndHorizontal();
				if (!info.sections.Contains(sect.name)) {
					EditorGUILayout.BeginHorizontal();
					EditorGUIUtil.TextField("Name", ref sect.name);
					if (GUILayout.Button("Save")) {
						sect.Save(GetSceneName());
						SaveInfo();
					}
					EditorGUILayout.EndHorizontal();
				}

                UnityObjList assetRefs = sect.GetAssetRefs();
                UnityObjList sceneRefs = sect.GetSceneObjects(GetSceneName());
				sect.scroll = EditorGUILayout.BeginScrollView(sect.scroll, GUILayout.ExpandHeight(true));
				changed |= DrawShortcutList(assetRefs, assetFilter);
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				changed |= DrawShortcutList(sceneRefs, sceneFilter);
				EditorGUILayout.EndScrollView();

				Object[] drag = EditorGUIUtil.DnD();
				if (drag != null) {
					foreach (Object o in drag) {
						if (o == null) {
							continue;
						}
						var obj = new UnityObjId(o);
						if (AssetDatabase.IsMainAsset(o) || AssetDatabase.IsSubAsset(o)) {
							if (!assetRefs.Contains(obj)) {
								assetRefs.Add(obj);
							}
						} else {
							if (!sceneRefs.Contains(obj)) {
								sceneRefs.Add(obj);
							}
						}
						changed = true;
					}
				}
				GUI.enabled = true;

				if (changed) {
					sect.Save(GetSceneName());
					Repaint();
				}
				EditorGUILayout.EndVertical();
			} else {
				if (GUILayout.Button(sect.name)) {
					sections.Remove(sect);
					sections.Insert(0, sect);
				}
			}
		}

        private bool DrawShortcutList(UnityObjList list, ObjListFilter<UnityObjId> filter) {
            AndPredicate<UnityObjId> predicate = filter.GetPredicate(list);
			#if !INTERNAL_REORDER
			var drawer = new UnityObjListDrawer(list);
			#else
			var drawer = new UnityObjIdReorderList(null, list);
			#endif
            drawer.allowSceneObject = false;
            drawer.Filter(predicate.Accept);
			#if !INTERNAL_REORDER
			return drawer.Draw(ReorderableListFlags.ShowIndices);
			#else
			return drawer.Draw();
			#endif
        }

		private Dictionary<Object, string> pathMap = new Dictionary<Object, string>();
		private string GetObjectPath(Object o) {
			string path = pathMap.Get(o);
			if (path != null) {
				return path;
			}
			path = AssetDatabase.GetAssetPath(o);
			if (path.IsEmpty()) {
				if (o is GameObject) {
					path = (o as GameObject).transform.GetScenePath();
				} else if (o is Component) {
					path = (o as Component).transform.GetScenePath();
				} else {
					path = o.ToString();
				}
			}
			pathMap[o] = path;
			return path;
		}
		
		private List<ShortcutSection> LoadShortcutSections() {
			List<ShortcutSection> sections = new List<ShortcutSection>();
			foreach (string name in info.sections) {
				ShortcutSection sect = new ShortcutSection(name, dir);
				sect.Load();
				sections.Add(sect);
			}
			if (info.IsEmpty()) {
				sections.Add(new ShortcutSection("NoName", dir));
			}
			return sections;
		}
		
		public static List<Object> LoadList(string filePath) {
			List<Object> objList = new List<Object>();
			if (File.Exists(filePath)) {
				List<string> guidList = SerializationUtil.ReadObject<List<string>>(filePath);
				if (guidList != null) {
					foreach (string guid in guidList) {
						Object obj = EditorAssetUtil.GetObject(guid);
						if (obj != null) {
							objList.Add(obj);
						}
					}
				}
			}
			return objList;
		}
		
		public override void OnHeaderGUI() {
			EditorGUILayout.BeginHorizontal();
			if (EditorGUIUtil.IntField("Column", ref info.columnCount, GUILayout.ExpandWidth(false))) {
				SaveInfo();
			}
			if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
				sections.Insert(0, new ShortcutSection("Section"+sectionCount, dir));
				sectionCount++;
			}
			EditorGUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI() {
			EditorGUILayout.BeginVertical();
			// Display Section
			int count = 0;
			for (int r=0; r<info.columnCount && count<sections.Count; r++) {
//				float width = GetWidth()/Math.Min(info.columnCount, sections.Count);
				EditorGUILayout.BeginHorizontal();
				for (int c=0; c<info.columnCount && count<sections.Count; c++) {
					ShortcutSection s = sections[r*info.columnCount+c];
					EditorGUILayout.BeginVertical();
					OnSectionGUI(count, s, r == 0);
					EditorGUILayout.EndVertical();
					++count;
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}

		public override void OnFooterGUI() {
		}
	}
}
