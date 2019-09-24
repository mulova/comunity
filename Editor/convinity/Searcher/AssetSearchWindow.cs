using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace convinity
{
    public class AssetSearchWindow : TabbedEditorWindow
	{
		protected override void CreateTabs() {
			AddTab(new TexSearchTab(this));
			AddTab(new ShaderSearchTab(this));
			AddTab(new AudioSearchTab(this));
			AddTab(new ColliderSearchTab(this));
		}
		
		[MenuItem("Tools/unilova/Asset/Asset Searcher")]
		static void OpenWiddow()
		{
			AssetSearchWindow mWindow = ScriptableObject.CreateInstance<AssetSearchWindow>();
			mWindow.titleContent = new GUIContent("Asset Search");
			mWindow.Show();
		}
	}
}
