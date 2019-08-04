using UnityEditor;

namespace scenehistorian
{
	public static class SceneHistoryMenu
	{
		[MenuItem("Tools/SceneHistorian/Scene History")]
		public static void OpenWindow()
		{
			SceneHistoryWindow.instance.Show();
		}
		
		[MenuItem("Tools/SceneHistorian/Previous Scene %#r")]
		public static void GoBackMenu()
		{
			SceneHistoryWindow.instance.GoBack();
		}
	}
}
