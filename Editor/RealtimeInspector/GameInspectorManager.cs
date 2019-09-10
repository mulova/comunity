#if FULL
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


public class GameInspectorManager : MonoBehaviour
{
	public string windowTitle = "Inspector";
	public int nameWidth = 100;
	public bool followMouse = true;
	public GUISkin skin;
	public Rect bound = new Rect (10, 10, 300, 200);

	private Inspector hot = null;
	
	public void Show (Inspector node)
	{
		hot = node;
	}

	public void Hide (Inspector node)
	{
		if (node == hot)
			hot = null;
	}

	void OnGUI ()
	{
		if (hot == null)
			return;
		if (skin != null)
			GUI.skin = skin;
		if(followMouse) {
			//make the inspector follow the mouse.
			bound.x = (int)Input.mousePosition.x;
			bound.y = (int)Screen.height - Input.mousePosition.y;
			//try and keep the inspector window visible regardless of mouse position.
			bound.x -= (Input.mousePosition.x > Screen.width / 2)?bound.width + 5:-5;
			bound.y -= (Input.mousePosition.y < Screen.height / 2)?bound.height + 5:-5;
		}
		//This draws the inspector window.
		GUILayout.BeginArea (bound, windowTitle, "box");
		GUILayout.Space (16);
		foreach (Descriptor d in hot.Descriptors) {
			GUILayout.BeginHorizontal();
			GUILayout.Label (d.Title, GUILayout.Width (nameWidth));
			GUILayout.Label (d.Value.ToString ());
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea ();
	}
}
#endif