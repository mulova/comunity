using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using comunity;
using commons;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Rotorz.Games.Collections;
using System.Text.Ex;

namespace scenehistorian
{
    public class SceneHistoryWindow : EditorWindow
    {
        private SceneHistory sceneHistory;
        private const string PATH = "Library/scenehistorian/history";
        private Object currentScene;
        private bool changed;
        private string nameFilter;
        private SceneHistoryDrawer listDrawer;
        private const bool SHOW_SIZE = false;
        private GUIContent sortIcon;

        private static readonly Color SORT_COLOR = Color.green;

        private bool valid
        {
            get
            {
                return !BuildPipeline.isBuildingPlayer;
            }
        }

        public static SceneHistoryWindow instance
        {
            get
            {
                return EditorWindow.GetWindow<SceneHistoryWindow>("SceneHistorian");
            }
        }


        void OnEnable()
        {
            sortIcon = new GUIContent(EditorGUIUtility.FindTexture("AlphabeticalSorting"), "Sort");
			var dir = Path.GetDirectoryName(PATH);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
            sceneHistory = SceneHistory.Load(PATH);
			OnSceneOpened(SceneManager.GetActiveScene(), OpenSceneMode.Single);
            #if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged += OnSceneObjChange;
            #else
            EditorApplication.hierarchyWindowChanged += OnSceneObjChange;
            #endif
			if (!BuildPipeline.isBuildingPlayer)
			{
				EditorApplication.pauseStateChanged += OnPauseStateChanged;
				EditorSceneManager.sceneOpening += OnSceneOpening;
				EditorSceneManager.sceneOpened += OnSceneOpened;
				EditorSceneManager.sceneClosing += OnSceneClosing;
				SceneManager.activeSceneChanged += OnActiveScene;
				SceneManager.sceneLoaded += OnSceneLoaded;
			}

            #if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
            #else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
            #endif
			var history = sceneHistory.items;
			if (history.Count > 0)
			{
				SceneViewContextMenu.AddContextMenu(menu=> {
					if (history.Count >= 2)
					{
						menu.AddItem(new GUIContent("Previous: " + history[1].name), false, GoBack);
					}
					for (int i=2; i<history.Count; ++i)
					{
						if (history[i].starred)
						{
							menu.AddItem(new GUIContent("scenes/"+ history[i].name), false, OnSceneMenu, history[i]);
						}
					}
				}, 1);
			}
        }

        void OnDisable()
        {
			// Enter play mode
			if (!Application.isPlaying)
			{
				SaveCam();
				sceneHistory.Save(PATH);
			}
            #if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged -= OnSceneObjChange;
            #else
            EditorApplication.hierarchyWindowChanged -= OnSceneObjChange;
            #endif
			EditorApplication.pauseStateChanged -= OnPauseStateChanged;
			EditorSceneManager.sceneOpening -= OnSceneOpening;
			EditorSceneManager.sceneOpened -= OnSceneOpened;
			EditorSceneManager.sceneClosing -= OnSceneClosing;
			SceneManager.activeSceneChanged -= OnActiveScene;
			SceneManager.sceneLoaded -= OnSceneLoaded;

            #if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
            #else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
            #endif
        }

		private void OnActiveScene(Scene s1, Scene s2)
		{
			if (Application.isPlaying) {
				return;
			}
			if (sceneHistory.Count == 0)
			{
				return;
			}
			if (!sceneHistory[0].activeScene.path.EqualsIgnoreSeparator(s2.path)) {
				sceneHistory[0].SetActiveScene(s2.path);
				sceneHistory.Save(PATH);
			}
		}

		void OnPauseStateChanged(PauseState state)
		{
//			if (sceneHistory.Count >= 0)
//			{
//				sceneHistory[0].ApplyCam();
//			}
		}

        private void OnSceneObjChange()
        {
            changed = true;
        }

        private void ChangePlayMode(PlayModeStateChange stateChange)
        {
			if (BuildPipeline.isBuildingPlayer)
			{
				return;
			}
			if (stateChange == PlayModeStateChange.EnteredEditMode)
			{
				if (sceneHistory.Count >= 0 && sceneHistory.useCam)
				{
					sceneHistory[0].ApplyCam();
				}
			}
        }

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
//			if (mode != LoadSceneMode.Single)
//			{
//				return;
//			}
			if (BuildPipeline.isBuildingPlayer)
			{
				return;
			}
            if (sceneHistory.useCam)
            {
			    int index = sceneHistory.IndexOf(scene.path);
			    if (index >= 0)
			    {
				    sceneHistory[index].ApplyCam();
			    }
            }
		}

        private void SaveCam()
        {
            if (sceneHistory.Count > 0)
            {
                var item = sceneHistory[0];
                item.SaveCam();
            }
        }

        private void OnSceneOpening(string path,OpenSceneMode mode)
        {
			if (BuildPipeline.isBuildingPlayer)
			{
				return;
			}
			if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
			if (mode == OpenSceneMode.Single)
			{
				SaveCam();
			}
			sceneHistory.Save(PATH);
        }

        private void OnSceneOpened(Scene s, OpenSceneMode mode)
        {
			if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            currentScene = AssetDatabase.LoadAssetAtPath<Object>(EditorSceneBridge.currentScene);
            if (currentScene == null)
            {
                return;
            }
            SceneHistoryItem item = null;
            int index = sceneHistory.IndexOf(currentScene);
            
            if (index >= 0)
            {
                item = sceneHistory[index];
                if (mode == OpenSceneMode.Single)
                {
                    sceneHistory.RemoveAt(index);
                    sceneHistory.Insert(0, item);
                    item.LoadAdditiveScenes();
                    if (sceneHistory.useCam)
                    {
                        item.ApplyCam();
                    }
                } else
                {
                    var sceneObj = AssetDatabase.LoadAssetAtPath<Object>(s.path);
                    if (!item.Contains(sceneObj))
                    {
                        item.AddScene(sceneObj);
                    }
                }
            } else
            {
                item = new SceneHistoryItem(currentScene);
                item.SaveCam();
                sceneHistory.Insert(0, item);
            }
            sceneHistory.Save(PATH);
            changed = false;
        }

        private void OnSceneClosing(Scene s, bool removing)
        {
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}
            if (EditorSceneBridge.currentScene == s.path)
            {
				SaveCam();
			} else
			{
				currentScene = AssetDatabase.LoadAssetAtPath<Object>(EditorSceneBridge.currentScene);
				if (currentScene == null)
				{
					return;
				}
				int index = sceneHistory.IndexOf(currentScene);
				SceneHistoryItem item = null;
				if (index >= 0)
				{
					item = sceneHistory[index];
					var sceneObj = AssetDatabase.LoadAssetAtPath<Object>(s.path);
					if (item.Contains(sceneObj))
					{
						item.RemoveScene(sceneObj);
					}
				} else
				{
					item = new SceneHistoryItem(currentScene);
					sceneHistory.Insert(0, item);
				}
			}
            sceneHistory.Save(PATH);
            changed = false;
        }

        public void GoBack()
        {
            if ((!changed||EditorSceneBridge.SaveCurrentSceneIfUserWantsTo()) && sceneHistory.Count > 1)
            {
                EditorSceneBridge.OpenScene(sceneHistory[1].first.path);
            }
        }

		void OnInspectorUpdate() {
			if (!Application.isPlaying)
			{
				// activeSceneChanged event is not available in Editor mode
				var s = SceneManager.GetActiveScene();
				OnActiveScene(s, s);
			}
		}

        private  void OnSceneMenu(object h)
        {
            SceneHistoryItem hist = h as SceneHistoryItem;
            EditorSceneManager.OpenScene(hist.first.path);
        }

        private void ShowMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Sort"), sceneHistory.sort, ()=> {
                sceneHistory.sort = !sceneHistory.sort;
                sceneHistory.Save(PATH);
            });
            menu.AddItem(new GUIContent("Cam"), sceneHistory.useCam, () => {
                sceneHistory.useCam = !sceneHistory.useCam;
                sceneHistory.Save(PATH);
            });

            menu.AddItem(new GUIContent("Clear"), false, () =>
            {
                if (EditorUtility.DisplayDialog("Warning", "Clear history?", "Ok", "Cancel"))
                {
                    sceneHistory.Clear();
                    File.Delete(PATH);
                }
            });
            menu.ShowAsContext();
        }

        private List<UnityObjId> allScenes = new List<UnityObjId>();
        public void OnHeaderGUI()
        {
            //GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUIUtil.SearchField("", ref nameFilter);
            if (nameFilter.IsEmpty())
            {
                allScenes.Clear();
            }
            GUI.enabled = sceneHistory.Count >= 2;
            if (GUILayout.Button("Back", EditorStyles.toolbarButton, GUILayout.Width(50), GUILayout.Height(20)))
            {
                GoBack();
            }
            var color = GUI.contentColor;
            if (sceneHistory.sort)
            {
                GUI.contentColor = SORT_COLOR;
            }
            if (GUILayout.Button(sortIcon, EditorStyles.toolbarButton, GUILayout.Width(30), GUILayout.Height(20)))
            {
                ShowMenu();
            }
            GUI.contentColor = color;
            GUILayout.EndHorizontal();
        }

        private Vector3 scrollPos;
        void OnGUI()
        {
            listDrawer = new SceneHistoryDrawer(sceneHistory);
            OnHeaderGUI();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			#if INTERNAL_REORDER
            var listDrawer = new SceneHistoryReorderList(sceneHistory);
            #else
            listDrawer.allowSceneObject = false;
            #endif
            if (nameFilter.IsNotEmpty())
            {
                string[] filters = nameFilter.SplitEx(' ');
                Predicate<SceneHistoryItem> filter = h =>
                {
                    if (h.first == null || h.first.path.IsEmpty())
                    {
                        return false;
                    }
                    string itemName = h.name;
                    foreach (var n in filters)
                    {
                        if (itemName.IndexOfIgnoreCase(n) < 0)
                        {
                            return false;
                        }
                    }
                    return true;
                };
                listDrawer.Filter(filter);
            }
            try
            {
#if INTERNAL_REORDER
                if (listDrawer.Draw())
#else
                try
                {
                    listDrawer.Draw(ReorderableListFlags.ShowIndices | ReorderableListFlags.HideAddButton | ReorderableListFlags.DisableContextMenu);
                } catch (Exception ex)
                {
                    Debug.LogException(ex);
                    sceneHistory.Clear();
                }
                if (listDrawer.changed)
#endif
                {
                    sceneHistory.Save(PATH);
                    changed = false;
                }
                if (allScenes.IsEmpty())
                {
                    var guids = UnityEditor.AssetDatabase.FindAssets("t:Scene");
                    foreach (var id in guids)
                    {
                        allScenes.Add(new UnityObjId(id));
                    }
                }

                if (nameFilter.IsNotEmpty())
                {
					EditorGUILayout.LabelField("Not in history", EditorStyles.miniBoldLabel);
                    string[] filters = nameFilter.SplitEx(' ');
                    var filteredScenes = new SceneHistory();
                    foreach (var s in allScenes)
                    {
                        string filename = Path.GetFileNameWithoutExtension(s.path);
                        bool match = true;
                        foreach (var f in filters)
                        {
                            if (filename.IndexOfIgnoreCase(f) < 0)
                            {
                                match = false;
                                break;
                            }
                        }
                        if (match && !sceneHistory.Contains(s.path))
                        {
                            filteredScenes.Add(s.reference);
                        }
                    }
                    listDrawer = new SceneHistoryDrawer(filteredScenes);
                    #if INTERNAL_REORDER
                    if (listDrawer.Draw())
                    #else
                    listDrawer.Draw(ReorderableListFlags.HideAddButton | ReorderableListFlags.DisableContextMenu | ReorderableListFlags.DisableReordering | ReorderableListFlags.DisableDuplicateCommand);
                    #endif
                }
            } catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            EditorGUILayout.EndScrollView();
            OnFooterGUI();
        }

        void OnFooterGUI()
        {
            GUI.enabled = true;
            if (SHOW_SIZE)
            {
                EditorGUILayout.BeginHorizontal();
                if (EditorGUIUtil.IntField("Size", ref sceneHistory.maxSize))
                {
                    if (sceneHistory.maxSize < 2)
                    {
                        sceneHistory.maxSize = 2;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
