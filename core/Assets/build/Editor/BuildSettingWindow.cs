using UnityEngine;
using UnityEditor;
using System.Collections;
using comunity;

namespace build
{
    public class BuildSettingWindow : TabbedEditorWindow
    {
        
        [MenuItem("Tools/Build Setting")]
        public static void Init()
        {
            BuildSettingWindow win = EditorWindow.GetWindow<BuildSettingWindow>();
            win.titleContent = new GUIContent("Build");
        }
        
        protected override void CreateTabs()
        {
            //      AddTab(new BuildSettingTab(this));
            AddTab(new BuildSettingTab(this), new AssetBuilderTab(new string[] {"DEV"}, this));
        }
    }
}
