using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using mulova.comunity;

namespace convinity
{
    public class SceneAnalyzerWindow : TabbedEditorWindow {
        
        [MenuItem("Tools/unilova/Scene Analyzer")]
        public static void ShowWindow() {
            EditorWindow win = EditorWindow.GetWindow(typeof(SceneAnalyzerWindow));
            win.titleContent = new GUIContent("Scene Analyzer");
        }
        
        protected override void CreateTabs() {
            AddTab(new RefSearchTab(this));
            AddTab(new ComponentSearchTab(this));
            AddTab(new SetterTab(this));
            AddTab(new LayerSearchTab(this));
            AddTab(new NullRefTab(this));
        }
        
    }
}

