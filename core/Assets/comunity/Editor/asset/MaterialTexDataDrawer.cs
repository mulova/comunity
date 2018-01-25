using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;


using System.IO;
using editor.ex;

namespace core
{
	[CustomPropertyDrawer(typeof(MaterialTexData))]
	public class MaterialTexDataDrawer : PropertyDrawerBase
	{
		protected override int GetLineCount()
		{
			return 1;
		}

		protected override void DrawGUI(GUIContent label)
		{
			SerializedProperty mat = GetProperty("material");
			SerializedProperty texPath = GetProperty("texPath");

			Material m = mat.objectReferenceValue as Material;
			if (DrawObjectField<Material>(GetLineRect(0), label, ref m, false)) {
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

