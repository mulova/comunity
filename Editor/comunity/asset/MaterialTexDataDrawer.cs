using System;
using UnityEngine;
using UnityEditor;

namespace mulova.comunity
{
	[CustomPropertyDrawer(typeof(MaterialTexData))]
	public class MaterialTexDataDrawer : PropertyDrawerBase
	{
		protected override int GetLineCount(SerializedProperty p)
		{
			return 3;
		}

		protected override void OnGUI(SerializedProperty p, Rect bound)
        {
			SerializedProperty mat = p.FindPropertyRelative("material");
            SerializedProperty tex1 = p.FindPropertyRelative("tex1");
			SerializedProperty tex2 = p.FindPropertyRelative("tex2");

			Material m = mat.objectReferenceValue as Material;
			if (DrawObjectField<Material>(bound, new GUIContent(p.name), ref m, false)) {
				mat.objectReferenceValue = m;
				if (m != null) {
					tex1.stringValue = EditorAssetUtil.GetAssetRelativePath(m.mainTexture);
				} else {
					tex1.stringValue = null;
				}
			}
		}
	}
}

