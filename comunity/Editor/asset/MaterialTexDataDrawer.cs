using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using System.IO;

namespace comunity
{
	[CustomPropertyDrawer(typeof(MaterialTexData))]
	public class MaterialTexDataDrawer : PropertyDrawerBase
	{
		protected override int GetLineCount(SerializedProperty p)
		{
			return 1;
		}

		protected override void DrawGUI(SerializedProperty p)
        {
			SerializedProperty mat = p.FindPropertyRelative("material");
			SerializedProperty texPath = p.FindPropertyRelative("texPath");

			Material m = mat.objectReferenceValue as Material;
			if (DrawObjectField<Material>(GetLineRect(0), new GUIContent(p.name), ref m, false)) {
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

