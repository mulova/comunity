using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace mulova.build
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
