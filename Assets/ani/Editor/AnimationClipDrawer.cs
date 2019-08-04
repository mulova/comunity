using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using commons;
using comunity;
using System.Text.Ex;
using UnityEngine.Ex;

namespace ani
{
	[CustomPropertyDrawer(typeof(AnimationClipListingAttribute))]
    public class AnimationClipDrawer : PropertyDrawerBase
	{
		protected override int GetLineCount(SerializedProperty p)
		{
			return 1;
		}

		protected override void DrawGUI(SerializedProperty p)
        {
            Component c = p.serializedObject.targetObject as Component;
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
				AnimationClip a = p.objectReferenceValue as AnimationClip;
				if (PopupNullable(GetLineRect(0), p.name, ref a, clips)) {
					p.objectReferenceValue = a;
				}
			} else {
				EditorGUI.PropertyField(GetLineRect(0), p, new GUIContent(p.name));
			}
		}
	}
}

