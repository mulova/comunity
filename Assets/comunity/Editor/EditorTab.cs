using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;
using commons;

namespace comunity {
	public abstract class EditorTab {
		
		public readonly object id;
		private string error = string.Empty;
		private string warning = string.Empty;
		private string info = string.Empty;
		private TabbedEditorWindow window;
		
		public EditorTab(object id, TabbedEditorWindow window) {
			this.id = id;
			this.window = window;
		}

		protected float GetWidth() {
			return position.width;
		}
		protected TabbedEditorWindow GetWindow() {
			return window;
		}

		public abstract void OnEnable();
		public abstract void OnHeaderGUI();
		public abstract void OnInspectorGUI();
		public abstract void OnFooterGUI();
		public abstract void OnDisable();
		public abstract void OnChangePlayMode(PlayModeStateChange stateChange);
		public abstract void OnChangeScene(string sceneName);
		public abstract void OnSelected(bool sel);
		/// <summary>
		/// Called when the tab is activated
		/// </summary>
		public abstract void OnFocus(bool focus);
		public virtual void OnInspectorUpdate() {}
		public virtual void OnSelectionChange() {}
		
		protected virtual void Repaint() {
			window.Repaint();
		}
		
		protected Rect position {
			get {
				return window.position;
			}
		}

		protected string GetSceneName() {
			return window.SceneName;
		}
		
		protected void ClearResults() {
			error = string.Empty;
			warning = string.Empty;
			info = string.Empty;
		}
		
		protected void SetError(Exception ex) {
			if (ex == null) {
				SetError(string.Empty);
			} else {
				SetError(ex.Message);
			}
		}
		
		protected void SetError(string format, params object[] param) {
			if (param != null) {
				this.error = format;
			} else {
				this.error = string.Format(format, param);
			}
		}
		
		protected void SetWarning(string format, params object[] param) {
			if (param != null) {
				this.warning = format;
			} else {
				this.warning = string.Format(format, param);
			}
		}
		
		protected void SetInfo(string format, params object[] param) {
			if (param != null) {
				this.info = format;
			} else {
				this.info = string.Format(format, param);
			}
		}
		
		public void ShowResult() {
			if (!string.IsNullOrEmpty(error)) {
				EditorGUILayout.HelpBox(error, MessageType.Error);
			}
			if (!string.IsNullOrEmpty(warning)) {
				EditorGUILayout.HelpBox(warning, MessageType.Warning);
			}
			if (!string.IsNullOrEmpty(info)) {
				EditorGUILayout.HelpBox(info, MessageType.Info);
			}
		}

		/// <summary>
		/// Apply scene changes to prefab
		/// </summary>
		/// <param name="col">Col.</param>
		protected void SaveChange(IEnumerable<GameObject> col) {
			bool changed = false;
			HashSet<GameObject> saved = new HashSet<GameObject>();
			foreach (GameObject o in col) {
				GameObject objRoot = PrefabUtility.FindPrefabRoot(o);
				if (objRoot != null && !saved.Contains(objRoot) && PrefabUtility.GetPrefabType(objRoot) == PrefabType.PrefabInstance) {
					saved.Add(objRoot);
					#if UNITY_2018_2_OR_NEWER
					PrefabUtility.ReplacePrefab(objRoot, PrefabUtility.GetCorrespondingObjectFromSource(objRoot), ReplacePrefabOptions.ConnectToPrefab);
					#else
					PrefabUtility.ReplacePrefab(objRoot, PrefabUtility.GetPrefabParent(objRoot), ReplacePrefabOptions.ConnectToPrefab);
					#endif
				}
				changed = true;
			}
			if (changed) {
				EditorSceneBridge.SaveScene();
				AssetDatabase.SaveAssets();
			}
		}
	}
}


