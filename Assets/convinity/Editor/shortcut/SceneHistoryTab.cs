using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using UnityEditorInternal;
using Rotorz.ReorderableList;
using comunity;

namespace convinity
{

    public class SceneHistoryTab : EditorTab
    {
        private UnityObjList sceneHistory;
        private const string PATH = "Library/Shortcut/history";
        private int size = 10;
        private Object currentScene;
        private bool changed;

        public SceneHistoryTab(object id, TabbedEditorWindow window) : base(id, window)
        {
        }

        public override void OnEnable()
        {
            BinarySerializer ser = new BinarySerializer(PATH, FileAccess.Read);
            sceneHistory = ser.Deserialize<UnityObjList>();
            ser.Close();
            if (sceneHistory == null)
            {
                sceneHistory = new UnityObjList();
            }
            OnChangeScene("");
            size = EditorPrefs.GetInt("SceneHistory", 10);
            EditorApplication.hierarchyWindowChanged += OnSceneObjChange;
        }

        public override void OnDisable()
        {
            SaveHistory();
            EditorApplication.hierarchyWindowChanged -= OnSceneObjChange;
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
            currentScene = null;
        }

        public override void OnChangeScene(string sceneName)
        {
            if (Application.isPlaying||EditorApplication.isPlaying)
            {
                return;
            }
            currentScene = AssetDatabase.LoadAssetAtPath(EditorSceneBridge.currentScene, typeof(Object));
            if (currentScene == null)
            {
                return;
            }
            sceneHistory.Remove(currentScene);
            sceneHistory.Insert(0, currentScene);
            if (sceneHistory.Count > size)
            {
                sceneHistory.RemoveAt(sceneHistory.Count-1);
            }
            SaveHistory();
            changed = false;
        }

        public override void OnInspectorUpdate()
        {
        }

        public override void OnHeaderGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = sceneHistory.Count > 1;
            if (GUILayout.Button("Back", EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
            {
                if (!changed||EditorSceneBridge.SaveCurrentSceneIfUserWantsTo())
                {
                    EditorSceneBridge.OpenScene(sceneHistory[1].path);
                }
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            ListDrawer<UnityObjId> listDrawer = new ListDrawer<UnityObjId>(sceneHistory, new UnityObjIdDrawer());
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

        private void SaveHistory()
        {
            BinarySerializer w = new BinarySerializer(PATH, FileAccess.Write);
            w.Serialize(sceneHistory);
        }
    }
}
