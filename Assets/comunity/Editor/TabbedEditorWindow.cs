using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using commons;

namespace comunity {
	public abstract class TabbedEditorWindow : EditorWindow {
		
		private TabData selected;
		private List<TabData> tabs = new List<TabData>();
		private bool closable;
		private static bool reloadStatic;

		void OnEnable() {
			CreateTabs();
			string tabName = EditorPrefs.GetString(GetWindowId());
			if (tabName.IsNotEmpty()) {
				foreach (TabData t in tabs) {
					if (tabName == t.tab.id.ToString()) {
						selected = t;
						break;
					}
				}

			} else {
				selected = tabs[0];
			}
			selected.tab.OnSelected(true);
            #if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
            #else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
            #endif
			autoRepaintOnSceneChange = true;
		}

		public string GetWindowId()
		{
			return string.Concat(GetType().FullName, "_", titleContent.text);
		}

		private bool showAllTab;
		public void ShowAllTab(bool show) {
			this.showAllTab = show;
			foreach (TabData t in tabs)
			{
				t.tab.OnSelected(true);
			}
		}

		protected abstract void CreateTabs();

		protected virtual void OnSelectionChange() {
			Repaint();
			selected.tab.OnSelectionChange();
        }
        
        protected void OnGUI() {
			try {
				if (showAllTab) {
					EditorGUILayout.BeginHorizontal();
					for (int i=0; i<tabs.Count; ++i) {
						EditorGUILayout.BeginVertical();
						TabData t = tabs[i];
						GUILayout.Label(t.tab.id.ToString(), EditorStyles.boldLabel);
						t.tab.OnHeaderGUI();
						t.tab.ShowResult();
						t.scrollPos = EditorGUILayout.BeginScrollView(t.scrollPos);
						t.tab.OnInspectorGUI();
						EditorGUILayout.EndScrollView();
						t.tab.OnFooterGUI();
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndHorizontal();
				} else {
                    Color bgColor = GUI.backgroundColor;
                    Color contentColor = GUI.contentColor;
					EditorGUILayout.BeginHorizontal();
					for (int i=0; i<tabs.Count; ++i) {
						TabData t = tabs[i];
						bool sel = t == selected;
						if (sel) {
                            GUI.backgroundColor = Color.white;
                            GUI.contentColor = Color.gray;
                            GUILayout.Button(t.tab.id.ToString(), EditorStyles.toolbarButton);
                            if (closable && GUILayout.Button("x", EditorStyles.toolbarButton, GUILayout.Width(15))) {
                                selected.tab.OnDisable();
                                tabs.Remove(selected);
                                i = Math.Max(0, i-1);
                                if (tabs.Count > 0) {
                                    selected = tabs[i];
                                } else {
                                    selected = null;
                                }
                            }
                        } else {
                            GUI.backgroundColor = Color.gray;
                            GUI.contentColor = Color.white;
                            if (GUILayout.Button(t.tab.id.ToString(), EditorStyles.toolbarButton)) {
                                selected.tab.OnSelected(false);
                                selected = t;
                                selected.tab.OnSelected(true);
                                sel = true;
                                EditorPrefs.SetString(GetWindowId(), t.tab.id.ToString());
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUI.backgroundColor = bgColor;
                    GUI.contentColor = contentColor;
					if (selected != null) {
						selected.tab.OnHeaderGUI();
						selected.tab.ShowResult();
						selected.scrollPos = EditorGUILayout.BeginScrollView(selected.scrollPos);
						selected.tab.OnInspectorGUI();
						EditorGUILayout.EndScrollView();
						selected.tab.OnFooterGUI();
					}
				}

                CheckContextMenu();
			} catch (Exception ex) {
				Exception e = ex;
				if (e.InnerException != null) {
					e = e.InnerException;
				}
				Debug.LogError(e.Message+"\n"+e.StackTrace);
				EditorUtility.DisplayDialog("Error", "Show error details in Editor log", "OK");
			}
		}

        protected virtual void ShowContextMenu() { }

        private void CheckContextMenu()
        {
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.ContextClick)
            {
                ShowContextMenu();
            }
        }
		
		protected void OnFocus() {
			sceneName = SceneName;
			foreach (TabData tab in tabs) {
				tab.tab.OnFocus(true);
			}
		}
		
		protected void OnLostFocus() {
			foreach (TabData tab in tabs) {
				tab.tab.OnFocus(false);
			}
		}
		
		protected void OnInspectorUpdate() {
			string newName = SceneName;
			if (sceneName != newName) {
				sceneName = newName;
				foreach (TabData tab in tabs) {
					tab.tab.OnChangeScene(sceneName);
				}
				Repaint();
			}
			foreach (TabData tab in tabs) {
				tab.tab.OnInspectorUpdate();
			}
		}

		private string sceneName = string.Empty;
		public string SceneName {
			get {
				if (Application.isPlaying) {
					return SceneBridge.loadedLevelName;
				}
				return Path.GetFileNameWithoutExtension(EditorSceneBridge.currentScene);
			}
		}
		
		protected void OnDestroy() {
			OnLostFocus();
			foreach (TabData t in tabs) {
				t.tab.OnDisable();
			}
            #if UNITY_2017
            EditorApplication.playModeStateChanged += ChangePlayMode;
            #else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
            #endif
		}

        protected void ChangePlayMode(PlayModeStateChange change) {
            foreach (TabData t in tabs) {
                t.tab.OnChangePlayMode();
            }
        }

		protected void ChangePlaymode() {
			foreach (TabData t in tabs) {
				t.tab.OnChangePlayMode();
			}
		}
		
		public void AddTab(params EditorTab[] tab) {
			foreach (EditorTab t in tab) {
				tabs.Add(new TabData(t));
                try {
                    t.OnEnable();
                    if (showAllTab)
                    {
                        t.OnSelected(true);
                    }
                } catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
			}
			if (selected == null) {
				selected = tabs[0];
			}
		}
		
		public EditorTab GetSelected() {
			return selected.tab;
		}
		
		public void SetClosable(bool closable) {
			this.closable = closable;
		}
		
		private class TabData {
			public EditorTab tab;
			public Vector3 scrollPos;
			
			public TabData(EditorTab t) {
				this.tab = t;
			}
		}
	}
}
