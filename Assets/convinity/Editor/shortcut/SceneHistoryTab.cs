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
        private int size = 10;
        private Object currentScene;
        private bool changed;

        public SceneHistoryTab(object id, TabbedEditorWindow window) : base(id, window)
        {
        }

        public override void OnEnable()
        {
            sceneHistory = SceneHistory.Load(PATH);
            OnSceneOpened(EditorSceneManager.GetActiveScene(), OpenSceneMode.Single);
            size = EditorPrefs.GetInt("SceneHistory", 10);
            EditorApplication.hierarchyWindowChanged += OnSceneObjChange;
            EditorSceneManager.sceneOpening += OnSceneOpening;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosing += OnSceneClosing;
			EditorSceneManager.sceneSaving += OnSceneSaved;
        }

        public override void OnDisable()
        {
            sceneHistory.Save(PATH);
            EditorApplication.hierarchyWindowChanged -= OnSceneObjChange;
			EditorSceneManager.sceneOpening -= OnSceneOpening;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.sceneClosing -= OnSceneClosing;
			EditorSceneManager.sceneSaving -= OnSceneSaved;
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
        }

        public override void OnChangeScene(string sceneName)
        {
        }

        private void OnSceneSaved(Scene scene, string path)
        {
			SaveCam();
			sceneHistory.Save(PATH);
        }

		private void SaveCam()
		{
			var item = sceneHistory[0];
			item.SaveCam();
		}

		private void OnSceneOpening(string path,OpenSceneMode mode)
		{
			SaveCam();
			sceneHistory.Save(PATH);
		}
        private void OnSceneOpened(Scene s, OpenSceneMode mode)
        {
            if (Application.isPlaying||EditorApplication.isPlaying)
            {
                return;
            }
            currentScene = AssetDatabase.LoadAssetAtPath<Object>(EditorSceneBridge.currentScene);
            if (!currentScene == null)
            {
                return;
            }
            int index = sceneHistory.IndexOf(currentScene);
            SceneHistoryItem item = null;
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
            int i = sceneHistory.Count-1;
            while (sceneHistory.Count > size && i > 2)
            {
                if (!sceneHistory[i].first.starred)
                {
                    sceneHistory.RemoveAt(i);
                }
                i--;
            }
            sceneHistory.Save(PATH);
            changed = false;
        }

        private void OnSceneClosing(Scene s, bool removing)
        {
            if (!removing || Application.isPlaying||EditorApplication.isPlaying)
            {
                return;
            }
            if (EditorSceneBridge.currentScene == s.path)
            {
                return;
            }
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
            int i = sceneHistory.Count-1;
            while (sceneHistory.Count > size && i > 2)
            {
                if (!sceneHistory[i].first.starred)
                {
                    sceneHistory.RemoveAt(i);
                }
                i--;
            }
            sceneHistory.Save(PATH);
            changed = false;
        }

        public override void OnInspectorUpdate()
        {
        }

        public override void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = sceneHistory.Count >= 2;
            if (GUILayout.Button("Back", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
            {
                if (!changed||EditorSceneBridge.SaveCurrentSceneIfUserWantsTo())
                {
                    EditorSceneBridge.OpenScene(sceneHistory[1].first.path);
                }
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            var listDrawer = new SceneHistoryDrawer(sceneHistory);
            listDrawer.allowSceneObject = false;
            try
            {
                listDrawer.Draw(ReorderableListFlags.ShowIndices|ReorderableListFlags.HideAddButton|ReorderableListFlags.DisableContextMenu);
            } catch (Exception ex)
            {
                if (!(ex.GetBaseException() is ExitGUIException))
                {
                    throw ex;
                }
            }
        }

        public override void OnFooterGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (EditorGUIUtil.IntField("Size", ref size))
            {
                EditorPrefs.SetInt("SceneHistory", size);
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
