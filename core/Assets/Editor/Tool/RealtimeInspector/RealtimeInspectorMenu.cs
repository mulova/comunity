#if FULL
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

public class RealtimeInspectorMenu
{
	[MenuItem ("Tools/unilova/Inspector/By Attribute")]
	static void AddAttributeMenu() {
		Selection.activeGameObject.AddComponent<AttributeInspector>();
	}
	
	[MenuItem ("Tools/unilova/Inspector/Transform")]
	static void AddTransformMenu() {
		Selection.activeGameObject.AddComponent<TransformInspector>();
	}
	
}
#endif