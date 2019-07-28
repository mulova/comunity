using System;
using comunity;
using UnityEditor;
using UnityEngine;

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
            SceneViewContextMenu.AddContextMenu(menu=> {
                foreach (var h in instance.history.items)
                {
                    menu.AddItem(new GUIContent("cam/"+h.id), false, OnCamMenu, h);
                }
            }, 10);
        } 

        void OnDisable()
        {
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
            var listDrawer = new SceneCamReorderList(history.items);
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
