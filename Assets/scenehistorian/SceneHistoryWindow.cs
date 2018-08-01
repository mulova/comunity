using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using Rotorz.ReorderableList;
using comunity;
using commons;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace scenehistorian
{
    public class SceneHistoryWindow : EditorWindow
    {
        private SceneHistory sceneHistory;
        private const string PATH = "Library/scenehistorian/history";
        private Object currentScene;
        private bool changed;
        private string filterName;
        private SceneHistoryDrawer listDrawer;

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
			var dir = Path.GetDirectoryName(PATH);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
            sceneHistory = SceneHistory.Load(PATH);
            listDrawer = new SceneHistoryDrawer(sceneHistory);
			OnSceneOpened(EditorSceneManager.GetActiveScene(), OpenSceneMode.Single);
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
				EditorSceneManager.activeSceneChanged += OnActiveScene;
				SceneManager.sceneLoaded += OnSceneLoaded;
			}

            #if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
            #else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
            #endif
            SceneView.onSceneGUIDelegate += OnSceneGUI;
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
			EditorSceneManager.activeSceneChanged -= OnActiveScene;
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
				if (sceneHistory.Count >= 0)
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
			int index = sceneHistory.IndexOf(scene.path);
			if (index >= 0)
			{
				sceneHistory[index].ApplyCam();
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
                    item.ApplyCam();
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

        [MenuItem("Tools/SceneHistorian/Scene History")]
        private static void OpenWindow()
        {
			instance.Show();
        }

        [MenuItem("Tools/SceneHistorian/Previous Scene %#r")]
        private static void GoBackMenu()
        {
			instance.GoBack();
        }

        public void GoBack()
        {
            if (!changed||EditorSceneBridge.SaveCurrentSceneIfUserWantsTo())
            {
                EditorSceneBridge.OpenScene(sceneHistory[1].first.path);
            }
        }

		private static bool drag;
        static void OnSceneGUI (SceneView sceneview) {
            if (Event.current.button == 1)
            {
				if (Event.current.type == EventType.MouseDown) {
					drag = false;				
				} else if (Event.current.type == EventType.MouseDrag) {
					drag = true;				
				} else if (Event.current.type == EventType.MouseUp && !drag) {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Previous Scene"), false, GoBackMenu);
                    foreach (var h in instance.sceneHistory.items)
                    {
                        if (h.starred)
                        {
                            menu.AddItem(new GUIContent(Path.GetFileNameWithoutExtension(h.first.path)), false, OnSceneMenu, h);
                        }
                    }
                    menu.ShowAsContext();
					Event.current.Use();
                }
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

        private static void OnSceneMenu(object h)
        {
            SceneHistoryItem hist = h as SceneHistoryItem;
            EditorSceneManager.OpenScene(hist.first.path);
        }

        public void OnHeaderGUI()
        {
            EditorGUIUtil.SearchField("", ref filterName);
        }

        private Vector3 scrollPos;
        void OnGUI()
        {
            OnHeaderGUI();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			#if INTERNAL_REORDER
            var listDrawer = new SceneHistoryReorderList(sceneHistory);
            #else
            listDrawer.allowSceneObject = false;
            #endif
            Predicate<SceneHistoryItem> filter = h => h.first != null && h.first.path != null && h.first.path.IndexOfIgnoreCase(filterName)>=0;
            listDrawer.Filter(filter);
            try
            {
				#if INTERNAL_REORDER
                if (listDrawer.Draw())
                #else
                listDrawer.Draw(ReorderableListFlags.ShowIndices|ReorderableListFlags.HideAddButton|ReorderableListFlags.DisableContextMenu);
                if (listDrawer.changed)
				#endif
				{
					sceneHistory.Save(PATH);
					changed = false;
				}
            } catch (Exception ex)
            {
                if (!(ex.GetBaseException() is ExitGUIException))
                {
                    throw ex;
                }
            }
            EditorGUILayout.EndScrollView();
            OnFooterGUI();
        }

        void OnFooterGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = sceneHistory.Count >= 2;
            if (GUILayout.Button("Back", GUILayout.Height(30)))
            {
                GoBack();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
			if (EditorGUIUtil.IntField("Size", ref sceneHistory.maxSize))
			{
				if (sceneHistory.maxSize < 2)
				{
					sceneHistory.maxSize = 2;
				}
			}
            if (GUILayout.Button("Clear"))
            {
                sceneHistory.Clear();
                File.Delete(PATH);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
