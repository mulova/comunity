#if FULL
using System;
using UnityEngine;

/**
 * Used to show the complex data of the game object
 * Add this to the game object containing Inspector
 */
[RequireComponent(typeof(NameInspector))]
public abstract class InspectorPlugin : MonoBehaviour
{
	public abstract string GetTitle();
	public abstract string Inspect();
}

#endif