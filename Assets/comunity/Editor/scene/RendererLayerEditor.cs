//  RendererLayerEditor.cs
//   Author:
//       Yves J. Albuquerque <yves.albuquerque@gmail.com>
//  Last Update:
//       28-12-2013
//Put this file into a folder named Editor.
//Based on Nick`s code at https://gist.github.com/nickgravelyn/7460288 and Ivan Murashko solution at http://forum.unity3d.com/threads/210683-List-of-sorting-layers?p=1432958&viewfull=1#post1432958 aput by Guavaman at http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System.Collections.Generic;

namespace comunity
{
	
	
	[CanEditMultipleObjects()]
	[CustomEditor(typeof(RendererLayer))]
	public class RendererLayerEditor : Editor
	{
		ParticleSystem[] l_particleSystems; //reference to our particle systems
		List<Renderer> l_renderers = new List<Renderer>();//reference to our renderers
		
		string[] sortingLayerNames;//we load here our Layer names to be displayed at the popup GUI
		int popupMenuIndex;//The selected GUI popup Index
		bool applyToChild = false;//Turn on/off if the effect will be extended to all renderers in child transforms
		bool applyToChildOldValue = false;//Used this old value to detect changes in applyToChild boolean
		
		/// <summary>
		/// Raises the enable event. We use it to set some references and do some initialization. I don`t figured out how to make a variable persistent in Unity Editor yet so most of the codes here can useless
		/// </summary>
		void OnEnable()
		{
			sortingLayerNames = GetSortingLayerNames(); //First we load the name of our layers
			l_particleSystems = (target as RendererLayer).gameObject.GetComponentsInChildren<ParticleSystem>();//Then we load our ParticleSystems
			for (int i = 0; i<l_particleSystems.Length;i++) //here we loads all renderers to our renderersarray
			{
				l_renderers.Add(l_particleSystems[i].GetComponent<Renderer>());
			}
			l_renderers.AddRange((target as RendererLayer).GetComponentsInChildren<Renderer>(true));
			
			for (int i = 0; i<sortingLayerNames.Length;i++) //here we initialize our popupMenuIndex with the current Sort Layer Name
			{
				if (sortingLayerNames[i] == l_particleSystems[0].GetComponent<Renderer>().sortingLayerName)
					popupMenuIndex = i;
			}
		}
		
		/// <summary>
		/// OnInspectorGUI is where the magic happens. Here we draw and change all the stuff
		/// </summary>
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector(); //first we draw our DefaultInspector
			
			if (l_renderers.Count == 0) //if there`s no Renderer at this object
			{
				return; //returns
			}
			
			popupMenuIndex = EditorGUILayout.Popup("Sorting Layer", popupMenuIndex, sortingLayerNames);//The popup menu is displayed simple as that
			int newSortingLayerOrder = EditorGUILayout.IntField("Order in Layer", l_renderers[0].sortingOrder); //Specifies the order to be drawed in this particular SortLayer
			applyToChild = EditorGUILayout.ToggleLeft("Apply to Childs", applyToChild);//If this change will be applyed to every renderer or just this one
			
			if (sortingLayerNames[popupMenuIndex] != l_renderers[0].sortingLayerName ||
				newSortingLayerOrder != l_renderers[0].sortingOrder ||
				applyToChild != applyToChildOldValue) //if there`s some change
			{
				Undo.RecordObject(l_renderers[0], "Change Particle System Renderer Order"); //first let record this change into Undo class so if the user did a mess, he can use ctrl+z to undo
				
				if (applyToChild) //change sortingLayerName and sortingOrder in each Renderer
				{
					for (int i = 0; i<l_renderers.Count;i++)
					{
						l_renderers[i].sortingLayerName = sortingLayerNames[popupMenuIndex];
						l_renderers[i].sortingOrder = newSortingLayerOrder;
					}
				}
				else //or at least at this one
				{
					l_renderers[0].sortingLayerName = sortingLayerNames[popupMenuIndex];
					l_renderers[0].sortingOrder = newSortingLayerOrder;
				}
				
				EditorUtil.SetDirty(l_renderers[0]); //saves
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