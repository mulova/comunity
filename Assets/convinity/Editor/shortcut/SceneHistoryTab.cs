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

    public class SceneHistoryTab : EditorTab
    {
        private SceneHistory sceneHistory;
        private const string PATH = "Library/Shortcut/history";
        private Object currentScene;
        private bool changed;

        public SceneHistoryTab(object id, TabbedEditorWindow window) : base(id, window)
        {
        }

        public override void OnEnable()
        {
            sceneHistory = SceneHistory.Load(PATH);
			OnSceneOpened(EditorSceneManager.GetActiveScene(), OpenSceneMode.Single);
            #if UNITY_2018_1_OR_NEWER
            EditorApplication.hierarchyChanged += OnSceneObjChange;
            #else
            EditorApplication.hierarchyWindowChanged += OnSceneObjChange;
            #endif
			EditorApplication.pauseStateChanged += OnPauseStateChanged;
			EditorSceneManager.sceneOpening += OnSceneOpening;
			EditorSceneManager.sceneOpened += OnSceneOpened;
			EditorSceneManager.sceneClosing += OnSceneClosing;
			SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
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
        }

		void OnPauseStateChanged(PauseState state)
		{
			if (sceneHistory.Count >= 0)
			{
				sceneHistory[0].ApplyCam();
			}
		}

        private void OnSceneObjChange()
        {
            changed = true;
        }

        public override void OnSelected(bool sel)
        {
        }

        public override void OnFocus(bool focus)
        {
        }

        public override void OnChangePlayMode()
        {
			if (!Application.isPlaying)
			{
				if (sceneHistory.Count >= 0)
				{
					sceneHistory[0].ApplyCam();
				}
			}
        }

        public override void OnChangeScene(string sceneName)
        {
        }

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
//			if (mode != LoadSceneMode.Single)
//			{
//				return;
//			}
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
			if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            SaveCam();
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

        public override void OnInspectorUpdate()
        {
        }

        private void GoBack()
        {
            if (!changed||EditorSceneBridge.SaveCurrentSceneIfUserWantsTo())
            {
                EditorSceneBridge.OpenScene(sceneHistory[1].first.path);
            }
        }

        public override void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = sceneHistory.Count >= 2;
            if (GUILayout.Button("Back", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
            {
                GoBack();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
			#if REORDERABLE_LIST
			var listDrawer = new SceneHistoryDrawer(sceneHistory);
			listDrawer.allowSceneObject = false;
			#else
			var listDrawer = new SceneHistoryReorderList(sceneHistory);
			#endif
            try
            {
				#if REORDERABLE_LIST
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
            if ((Event.current.command || Event.current.control) && Event.current.shift && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Backspace)
            {
                GoBack();
            }
        }

        public override void OnFooterGUI()
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
