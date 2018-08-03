
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
using System.Runtime.CompilerServices;

namespace scenehistorian
{

    public class SceneCamWindow : EditorWindow
    {
        private const string PATH = "Library/scenehistorian/cam";
        private SceneCamHistory history;

        public static SceneCamWindow instance
        {
            get
            {
                return EditorWindow.GetWindow<SceneCamWindow>("Scene Camera");
            }
        }

        [MenuItem("Tools/SceneHistorian/Scene Cam")]
        private static void OpenWindow()
        {
            instance.Show();
        }

        void OnEnable()
        {
            history = SceneCamHistory.Load(PATH);
            #if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
            #else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
            #endif
            SceneViewMenu.AddContextMenu(menu=> {
                foreach (var h in instance.history.items)
                {
                    menu.AddItem(new GUIContent("cam/"+h.id), false, OnCamMenu, h);
                }
            }, 1);
        }

        void OnDisable()
        {
            #if UNITY_2017_1_OR_NEWER
            EditorApplication.playModeStateChanged += ChangePlayMode;
            #else
            EditorApplication.playmodeStateChanged += ChangePlaymode;
            #endif
            // Enter play mode
            if (!Application.isPlaying)
            {
                history.Save(PATH);
            }
        }

        private void OnCamMenu(object h)
        {
            SceneCamProperty p = h as SceneCamProperty;
            p.Apply();
        }

        private void ChangePlayMode(PlayModeStateChange stateChange)
        {
			if (BuildPipeline.isBuildingPlayer)
			{
				return;
			}
			if (stateChange == PlayModeStateChange.EnteredEditMode)
			{
				if (history.Count > 0)
				{
					history[0].Apply();
				}
			}
        }

        void OnHeaderGUI()
        {
        }

        void OnFooterGUI()
        {
        }

        private Vector3 scrollPos;
        void OnGUI()
        {
            OnHeaderGUI();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            var listDrawer = new ListDrawer<SceneCamProperty>(history.items, new SceneCamPropertyDrawer());
            try
            {
				if (listDrawer.Draw())
				{
					history.Save(PATH);
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

    }
}
