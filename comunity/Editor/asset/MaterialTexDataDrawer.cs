using System;
using UnityEngine;
using UnityEditor;

namespace comunity
{
	[CustomPropertyDrawer(typeof(MaterialTexData))]
	public class MaterialTexDataDrawer : PropertyDrawerBase
	{
		protected override int GetLineCount(SerializedProperty p)
		{
			return 1;
		}

		protected override void DrawProperty(SerializedProperty p, Rect bound)
        {
			SerializedProperty mat = p.FindPropertyRelative("material");
			SerializedProperty texPath = p.FindPropertyRelative("texPath");

			Material m = mat.objectReferenceValue as Material;
			if (DrawObjectField<Material>(bound, new GUIContent(p.name), ref m, false)) {
				mat.objectReferenceValue = m;
				if (m != null) {
					texPath.stringValue = EditorAssetUtil.GetAssetRelativePath(m.mainTexture);
				} else {
					texPath.stringValue = null;
				}
			}
		}
	}
}

