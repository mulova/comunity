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

namespace convinity
{
    public class SceneHistoryWindow : EditorWindow
    {
        private SceneHistory sceneHistory;
        private const string PATH = "Library/Shortcut/history";
        private Object currentScene;
        private bool changed;

        void OnEnable()
        {
			var dir = Path.GetDirectoryName(PATH);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
            sceneHistory = SceneHistory.Load(PATH);
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
				SceneManager.sceneLoaded += OnSceneLoaded;
			}

            #if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
            #else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
            #endif
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
			SceneManager.sceneLoaded -= OnSceneLoaded;

            #if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
            #else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
            #endif
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

        [MenuItem("Tools/SceneView/Scene HIstory")]
        private static void OpenWindow()
        {
            var win = EditorWindow.GetWindow<SceneHistoryWindow>("Scene History");
            win.Show();
        }

        [MenuItem("Tools/SceneView/Previous Scene %#&z")]
        private static void GoBackMenu()
        {
            var win = EditorWindow.GetWindow<SceneHistoryWindow>();
            win.GoBack();
        }

        public void GoBack()
        {
            if (!changed||EditorSceneBridge.SaveCurrentSceneIfUserWantsTo())
            {
                EditorSceneBridge.OpenScene(sceneHistory[1].first.path);
            }
        }

        public void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = sceneHistory.Count >= 2;
			if (GUILayout.Button("Back", GUILayout.Height(30)))
            {
                GoBack();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        void OnGUI()
        {
            OnHeaderGUI();
			#if !INTERNAL_REORDER
			var listDrawer = new SceneHistoryDrawer(sceneHistory);
			listDrawer.allowSceneObject = false;
			#else
			var listDrawer = new SceneHistoryReorderList(sceneHistory);
			#endif
            try
            {
				#if !INTERNAL_REORDER
				listDrawer.Draw(ReorderableListFlags.ShowIndices|ReorderableListFlags.HideAddButton|ReorderableListFlags.DisableContextMenu);
				if (listDrawer.changed)
				#else
				if (listDrawer.Draw())
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
            OnFooterGUI();
        }

        void OnFooterGUI()
        {
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
