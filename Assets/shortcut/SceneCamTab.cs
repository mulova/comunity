
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

namespace convinity
{

    public class SceneCamTab : EditorTab
    {
        private const string PATH = "Library/Shortcut/scene_cam";
        private SceneCamHistory history;

        public SceneCamTab(object id, TabbedEditorWindow window) : base(id, window)
        {
        }

        public override void OnEnable()
        {
            history = SceneCamHistory.Load(PATH);
        }

        public override void OnDisable()
        {
			// Enter play mode
			if (!Application.isPlaying)
			{
				history.Save(PATH);
			}
        }

        public override void OnSelected(bool sel)
        {
        }

        public override void OnFocus(bool focus)
        {
        }

        public override void OnChangePlayMode(PlayModeStateChange stateChange)
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

        public override void OnChangeScene(string sceneName)
        {
        }

        public override void OnInspectorUpdate()
        {
        }

        public override void OnHeaderGUI()
        {
        }

        public override void OnInspectorGUI()
        {
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
        }

        public override void OnFooterGUI()
        {
        }
    }
}
