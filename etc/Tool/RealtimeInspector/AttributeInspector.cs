#if FULL
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/**
 * Show 'InspectableAttribute' members only
 */
public class AttributeInspector : Inspector
{
	protected override void AddDescriptors(List<Descriptor> list) {
		foreach (Component component in gameObject.GetComponents<Component>()) {
			FieldInfo[] fields = component.GetType().GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.inst);
			foreach (FieldInfo info in fields) {
				object[] attributes = info.GetCustomAttributes (true);
				foreach (object attr in attributes) {
					if (attr.GetType () == typeof(InspectableAttribute)) {
						list.Add (new Descriptor (component, info, attr as InspectableAttribute));
					}
				}
			}
		}
		//sort the Inspectable values using the rank value.
		list.Sort (delegate(Descriptor A, Descriptor B) {
			if (A.Attr.rank == B.Attr.rank)
				return 0;
			return A.Attr.rank < B.Attr.rank ? -1 : 1;
		});
	}
}
#endif