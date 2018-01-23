using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using editor.ex;
using UnityEngine.Assertions;
using commons;

namespace core
{
	[CustomPropertyDrawer(typeof(AssetRef))]
	public class AssetRefDrawer : PropertyDrawerBase
	{
		private static readonly bool DIGEST = false;

		protected override int GetLineCount()
		{
            SerializedProperty guid = GetProperty("guid");
            string id = guid.stringValue;
            if (id.IsEmpty())
            {
                return 1;
            }
            SerializedProperty path = GetProperty("path");
            if (path.stringValue.IsResourcesPath())
            {
                return 1;
            }
            AssetPropertyAttribute attr = GetAttribute<AssetPropertyAttribute>();
            if (attr != null && attr.exclusive)
            {
                return 3;
            } else
            {
                return 2;
            }
		}

		protected override void DrawGUI(GUIContent label)
		{
			int lineCount = GetLineCount();
			int lineNo = 0;
			SerializedProperty cdn = GetProperty("cdn");
			SerializedProperty path = GetProperty("path");
			SerializedProperty exclusiveId = GetProperty("exclusiveAssetId");
			SerializedProperty reference = GetProperty("reference");
			SerializedProperty md5 = GetProperty("md5");
			SerializedProperty guid = GetProperty("guid");

			if (reference == null)
			{
				return;
			}
			string editorPath = AssetDatabase.GUIDToAssetPath(guid.stringValue);
			if (editorPath.IsEmpty())
			{
				editorPath = path.stringValue;
			}
			Object obj = AssetDatabase.LoadAssetAtPath(editorPath, typeof(Object));

			if (obj == null)
			{
				obj = reference.objectReferenceValue;
				if (obj != null)
				{
					editorPath = AssetDatabase.GetAssetPath(obj);
				}
			}
			string assetPath = string.Empty;
			if (editorPath.IsNotEmpty())
			{
				assetPath = EditorAssetUtil.GetAssetRelativePath(editorPath);
			}
			EditorGUI.indentLevel = prop.depth;

			Rect line1Rect = GetLineRect(lineNo);
            string refType = null;
            if (assetPath.IsResourcesPath()) {
                refType = "R";
            } else
            {
                refType = reference.objectReferenceValue != null? "O" : "X";
            }
			string title = string.Format("{0} ({1})", label.text, refType);

			Color oldColor = GUI.backgroundColor;
			if (DIGEST&&EditorAssetUtil.IsModified(path.stringValue, md5.stringValue))
			{
				GUI.backgroundColor = Color.red;
			}
			Object newRef = EditorGUI.ObjectField(line1Rect, title, obj, typeof(Object), true);
			if (newRef != obj||path.stringValue != assetPath||(cdn.boolValue^reference.objectReferenceValue == null))
			{
				Set(cdn, path, reference, guid, newRef);
			}
			lineNo++;
			EditorGUI.indentLevel++;
            if (lineNo < lineCount)
            {
                if (newRef != null)
                {
                    Rect pathRect = GetLineRect(lineNo);
                    EditorGUI.SelectableLabel(pathRect, path.stringValue);
                    lineNo++;
                }
            }
			GUI.backgroundColor = oldColor;

			if (lineNo < lineCount && cdn.boolValue == true)
			{
				// draw alias
				Rect aliasRect = GetLineRect(lineNo);
				string newAlias = EditorGUI.TextField(aliasRect, exclusiveId.name, exclusiveId.stringValue);
				if (newAlias != exclusiveId.stringValue)
				{
					exclusiveId.stringValue = newAlias;
				}
				lineNo++;
			}
			EditorGUI.indentLevel--;
		}

		public static void Set(SerializedObject obj, string varName, Object val)
		{
			obj.Update();
			SerializedProperty assetRef = obj.FindProperty(varName);
			Set(assetRef.FindPropertyRelative("cdn"), assetRef.FindPropertyRelative("path"), assetRef.FindPropertyRelative("reference"), assetRef.FindPropertyRelative("guid"), val); 
			obj.ApplyModifiedProperties();
		}

		public static void Set(SerializedProperty cdn, SerializedProperty path, SerializedProperty reference, SerializedProperty uuid, Object val)
		{
			if (val != null)
			{
				string pathVal = AssetDatabase.GetAssetPath(val);
				uuid.stringValue = AssetDatabase.AssetPathToGUID(pathVal);
				path.stringValue = AssetRefEditorEx.GetPath(val);
                cdn.boolValue = AssetBundlePath.inst.IsCdnPath(pathVal);
            } else
            {
                path.stringValue = string.Empty;
                uuid.stringValue = string.Empty;
                cdn.boolValue = false;
            }
			if (cdn.boolValue)
			{
				reference.objectReferenceValue = null;
			} else
			{
				reference.objectReferenceValue = val;
			}
		}
	}
}
