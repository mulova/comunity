#if FULL
using System;
using UnityEngine;

public class TransformInspector : InspectorPlugin
{
	public override string GetTitle() {
		return "Transform";
	}
	
	public override string Inspect() {
		return transform.ToString();
	}
}

#endif