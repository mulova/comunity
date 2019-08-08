//  SortLayerRendererExtension.cs
//   Author:
//       Yves J. Albuquerque <yves.albuquerque@gmail.com>
//  Last Update:
//       27-12-2013
//Put this file into a folder named Editor.
//Based on Nick`s code at https://gist.github.com/nickgravelyn/7460288 and Ivan Murashko solution at http://forum.unity3d.com/threads/210683-List-of-sorting-layers?p=1432958&viewfull=1#post1432958 aput by Guavaman at http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

namespace comunity
{
	
	[CanEditMultipleObjects()]
	[CustomEditor(typeof(SortingLayerExposed),true)]
	public class SortLayerRendererExtension : Editor
	{
		SortingLayerExposed layer;
		Renderer renderer;
		Renderer[] childsRenderer;
		string[] sortingLayerNames;
		
		int selectedOption;
		bool applyToChild = false;
		bool applyToChildOldValue = false;
		
		void OnEnable()
		{
			layer = target as SortingLayerExposed;
			sortingLayerNames = GetSortingLayerNames();
			renderer = layer.GetComponent<Renderer>();
			if (renderer.transform.childCount > 1)
				childsRenderer = layer.transform.GetComponentsInChildren<Renderer>();
			
			for (int i = 0; i<sortingLayerNames.Length;i++)
			{
				if (sortingLayerNames[i] == renderer.sortingLayerName)
					selectedOption = i;
			}
		}
		
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			if (!renderer)
			{
				return;
			}
			
			EditorGUILayout.LabelField("\n");
			
			selectedOption = EditorGUILayout.Popup("Sorting Layer", selectedOption, sortingLayerNames);
			if (sortingLayerNames[selectedOption] != renderer.sortingLayerName)
			{
				Undo.RecordObject(renderer, "Sorting Layer");
				if (!applyToChild)
					renderer.sortingLayerName = sortingLayerNames[selectedOption];
				else
				{
					for (int i = 0; i<childsRenderer.Length;i++)
					{
						childsRenderer[i].sortingLayerName = sortingLayerNames[selectedOption];
					}
				}
				EditorUtil.SetDirty(renderer);
			}
			
			int newSortingLayerOrder = EditorGUILayout.IntField("Order in Layer", renderer.sortingOrder);
			if (newSortingLayerOrder != renderer.sortingOrder)
			{
				Undo.RecordObject(renderer, "Edit Sorting Order");
				renderer.sortingOrder = newSortingLayerOrder;
				EditorUtil.SetDirty(renderer);
			}
			
			applyToChild = EditorGUILayout.ToggleLeft("Apply to Childs", applyToChild);
			if (applyToChild != applyToChildOldValue)
			{
				for (int i = 0; i<childsRenderer.Length;i++)
				{
					childsRenderer[i].sortingLayerName = sortingLayerNames[selectedOption];
				}
				Undo.RecordObject(renderer, "Apply Sort Mode To Child");
				applyToChildOldValue = applyToChild;
				EditorUtil.SetDirty(renderer);
			}
		}
		
		// Get the sorting layer names
		public string[] GetSortingLayerNames()
		{
			Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			return (string[])sortingLayersProperty.GetValue(null, new object[0]);
		}
		
		// Get the unique sorting layer IDs -- tossed this in for good measure
		public int[] GetSortingLayerUniqueIDs()
		{
			Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
			return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
		}
	}
}