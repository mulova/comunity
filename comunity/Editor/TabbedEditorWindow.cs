using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using mulova.commons;
using System.Text.Ex;

namespace comunity {
	public abstract class TabbedEditorWindow : EditorWindow {
		
		private TabData selected;
		private List<TabData> tabs = new List<TabData>();
		private bool closable;

        public Color activeTabBgColor = Color.red;
        public Color activeTabContentColor = Color.green;
        public Color inactiveTabBgColor = Color.black;
        public Color inactiveTabContentColor = Color.gray;

        protected EditorTab activeTab
        {
            get
            {
                return selected?.tab;
            }
        }

        private bool isContextMenu
        {
            get
            {
                Event currentEvent = Event.current;
                return currentEvent.type == EventType.ContextClick;
            }
        }

        public int tabCount
        {
            get
            {
                return tabs.Count;
            }
        }

        protected virtual void OnEnable() {
			CreateTabs();
			string tabName = EditorPrefs.GetString(GetWindowId());
			if (!tabName.IsEmpty()) {
				foreach (TabData t in tabs) {
					if (tabName == t.tab.ToString()) {
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

        protected virtual void OnDisable()
		{
			OnLostFocus();
			foreach (TabData t in tabs) {
				t.tab.OnDisable();
			}
		}

		protected void OnDestroy() {
			#if UNITY_2017_1_OR_NEWER
			EditorApplication.playModeStateChanged += ChangePlayMode;
			#else
			EditorApplication.playmodeStateChanged += ChangePlaymode;
			#endif
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

        private GenericMenu _contextMenu;
        public GenericMenu contextMenu
        {
            get
            {
                if (_contextMenu == null)
                {
                    _contextMenu = new GenericMenu();
                }
                return _contextMenu;
            }
        }

        protected void OnGUI() {
			try {
                _contextMenu = null;
				if (showAllTab) {
					EditorGUILayout.BeginHorizontal();
					for (int i=0; i<tabs.Count; ++i) {
						EditorGUILayout.BeginVertical();
						TabData t = tabs[i];
						GUILayout.Label(t.tab.ToString(), EditorStyles.boldLabel);
						t.tab.OnHeaderGUI();
						t.tab.ShowResult();
						t.scrollPos = EditorGUILayout.BeginScrollView(t.scrollPos);
						t.tab.OnInspectorGUI();
						EditorGUILayout.EndScrollView();
						t.tab.OnFooterGUI();
						EditorGUILayout.EndVertical();
                        if (isContextMenu)
                        {
                            selected.tab.AddContextMenu();
                        }
                    }
					EditorGUILayout.EndHorizontal();
				} else {
                    if (tabs.Count > 1)
                    {
                        Color bgColor = GUI.backgroundColor;
                        Color contentColor = GUI.contentColor;
                        EditorGUILayout.BeginHorizontal();
                        for (int i=0; i<tabs.Count; ++i) {
                            TabData t = tabs[i];
                            bool sel = t == selected;
                            if (sel) {
                                GUI.backgroundColor = activeTabBgColor;
                                GUI.contentColor = activeTabContentColor;
                                GUILayout.Button(t.tab.ToString(), EditorStyles.toolbarButton);
                                if (closable && GUILayout.Button("x", EditorStyles.toolbarButton, GUILayout.Width(15))) {
                                    selected.tab.OnDisable();
                                    selected.tab.Remove();
                                    tabs.Remove(selected);
                                    i = Math.Max(0, i-1);
                                    if (tabs.Count > 0) {
                                        selected = tabs[i];
                                    } else {
                                        selected = null;
                                    }
                                }
                            } else {
                                GUI.backgroundColor = inactiveTabBgColor;
                                GUI.contentColor = inactiveTabContentColor;
                                if (GUILayout.Button(t.tab.ToString(), EditorStyles.toolbarButton)) {
                                    selected.tab.OnSelected(false);
                                    selected = t;
                                    selected.tab.OnSelected(true);
                                    sel = true;
                                    EditorPrefs.SetString(GetWindowId(), t.tab.ToString());
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        GUI.backgroundColor = bgColor;
                        GUI.contentColor = contentColor;
                    }
					if (selected != null) {
						selected.tab.OnHeaderGUI();
						selected.tab.ShowResult();
						selected.scrollPos = EditorGUILayout.BeginScrollView(selected.scrollPos);
						selected.tab.OnInspectorGUI();
						EditorGUILayout.EndScrollView();
						selected.tab.OnFooterGUI();
                        if (isContextMenu)
                        {
                            selected.tab.AddContextMenu();
                        }
					}
				}

                if (isContextMenu)
                {
                    AddContextMenu();
                    _contextMenu.ShowAsContext();
                }
			} catch (Exception ex) {
				Exception e = ex;
				if (e.InnerException != null) {
					e = e.InnerException;
				}
                Debug.LogError(e.ToString());
//				EditorUtility.DisplayDialog("Error", "Show error details in Editor log", "OK");
			}
		}

        protected virtual void AddContextMenu() { }

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
		
        protected void ChangePlayMode(PlayModeStateChange change) {
            foreach (TabData t in tabs) {
                t.tab.OnChangePlayMode(change);
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

        public void RemoveTab(EditorTab tab)
        {
            int index = tabs.FindIndex(t => t.tab == tab);
            tabs.RemoveAt(index);
            if (selected != null && selected.tab == tab)
            {
                tab.OnSelected(false);
                if (index > 0)
                {
                    selected = tabs[index - 1];
                } else if (tabs.Count > 0)
                {
                    selected = tabs[0];
                }
                selected.tab.OnSelected(true);
            }
            tab.OnDisable();
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
