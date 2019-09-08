using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using mulova.comunity;

namespace convinity {
	class LayerSearchTab : SearchTab<GameObject>
	{
		List<GameObject> layerList = new List<GameObject> ();
		private int layer;
		
		public LayerSearchTab(TabbedEditorWindow window) : base("Layer", window)
		{
		}
		
		public override void OnHeaderGUI(List<GameObject> found) {
			EditorGUILayout.BeginHorizontal ();
			int newLayer = EditorGUILayout.LayerField (layer);
			if (layer != newLayer) {
				layer = newLayer;
				Search();
			}
			EditorGUILayout.EndHorizontal ();
		}
		
		protected override List<GameObject> SearchResource()
		{
			layerList.Clear ();
			Object[] objects = Object.FindObjectsOfType (typeof(GameObject));
			foreach (Object obj in objects) {
				GameObject gameObj = obj as GameObject;
				if (gameObj != null) {
					if (gameObj.layer == layer) {
						layerList.Add (gameObj);
					}
				}
			}
			return layerList;
		}
		
		protected override void OnInspectorGUI(List<GameObject> found)
		{
			foreach (GameObject o in found) {
				EditorGUILayout.ObjectField (o, o.GetType (), true);
			}
		}
		
		public override void OnFooterGUI(List<GameObject> found) {}

		public override void OnChangePlayMode(PlayModeStateChange stateChange) {}
		public override void OnChangeScene(string sceneName) {}

		public override void OnFocus(bool focus) {}
		public override void OnSelected(bool sel) {}
	}
}