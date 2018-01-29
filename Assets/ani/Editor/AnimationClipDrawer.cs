using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using editor.ex;
using commons;

namespace ani
{
	[CustomPropertyDrawer(typeof(AnimationClipListingAttribute))]
	public class AnimationClipDrawer : PropertyDrawerBase
	{
		protected override int GetLineCount()
		{
			return 1;
		}

		protected override void DrawGUI(GUIContent label)
		{
			Component c = prop.serializedObject.targetObject as Component;
			Animation anim = c.GetComponent<Animation>();
			AnimationClipListingAttribute attr = attribute as AnimationClipListingAttribute;
			if (attr.varName.IsNotEmpty()) {
				Object o = ReflectionUtil.GetFieldValue<Object>(c, attr.varName);
				if (o != null) {
					if (o is Animation) {
						anim = o as Animation;
					} else if (o is Component) {
						anim = (o as Component).GetComponent<Animation>();
					} else if (o is GameObject) {
						anim = (o as GameObject).GetComponent<Animation>();
					}
				}
			}
			if (anim != null) {
				AnimationClip[] clips = anim.GetAllClips().ToArray();
				AnimationClip a = prop.objectReferenceValue as AnimationClip;
				if (PopupNullable(GetLineRect(0), prop.name, ref a, clips)) {
					prop.objectReferenceValue = a;
				}
			} else {
				EditorGUI.PropertyField(GetLineRect(0), prop, new GUIContent(prop.name));
			}
		}
	}
}

