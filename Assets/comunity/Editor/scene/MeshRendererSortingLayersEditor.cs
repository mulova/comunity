using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

namespace comunity
{
	
	[CanEditMultipleObjects()]
	[CustomEditor(typeof(MeshRenderer))]
	public class MeshRendererSortingLayersEditor : Editor
	{
		Renderer renderer;
		string[] sortingLayerNames;
		int selectedOption;
		
		void OnEnable()
		{
			sortingLayerNames = GetSortingLayerNames();
			renderer = target as Renderer;
			
			for (int i = 0; i<sortingLayerNames.Length;i++)
			{
				if (sortingLayerNames[i] == renderer.sortingLayerName)
					selectedOption = i;
			}
		}
		
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			if (!renderer) return;
			
			EditorGUILayout.BeginHorizontal();
			selectedOption = EditorGUILayout.Popup("Sorting Layer", selectedOption, sortingLayerNames);
			if (sortingLayerNames[selectedOption] != renderer.sortingLayerName)
			{
				Undo.RecordObject(renderer, "Sorting Layer");
				renderer.sortingLayerName = sortingLayerNames[selectedOption];
				CompatibilityEditor.SetDirty(renderer);
			}
			EditorGUILayout.LabelField("(Id:" + renderer.sortingLayerID.ToString() + ")", GUILayout.MaxWidth(40));
			EditorGUILayout.EndHorizontal();
			
			int newSortingLayerOrder = EditorGUILayout.IntField("Order in Layer", renderer.sortingOrder);
			if (newSortingLayerOrder != renderer.sortingOrder)
			{
				Undo.RecordObject(renderer, "Edit Sorting Order");
				renderer.sortingOrder = newSortingLayerOrder;
				CompatibilityEditor.SetDirty(renderer);
			}
			
			bool rq = false;
			if (rq)
			{
				if (renderer.sharedMaterial != null) {
					int newRQ = EditorGUILayout.IntField("RenderQueue", renderer.sharedMaterial.renderQueue);
					if (newRQ != renderer.sharedMaterial.renderQueue)
					{
						Undo.RecordObject(renderer.sharedMaterial, "Edit RenderQueue");
						foreach (var m in renderer.sharedMaterials)
						{
							m.renderQueue = newRQ;
						}
						CompatibilityEditor.SetDirty(renderer.sharedMaterial);
					}
				}
			}
		}
		
		// Get the sorting layer names
		public string[] GetSortingLayerNames()
		{
			Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			return (string[])sortingLayersProperty.GetValue(null, new object[0]);
		}	
	}
}
