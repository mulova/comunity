using System.Text.Ex;
using mulova.commons;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using UnityEngine.Ex;
using Object = UnityEngine.Object;

namespace mulova.comunity
{
    [CustomPropertyDrawer(typeof(AssetRef))]
	public class AssetRefDrawer : PropertyDrawerBase
	{
		private static readonly bool DIGEST = false;

		protected override int GetLineCount(SerializedProperty p)
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
            AssetPropertyAttribute attr = attribute as AssetPropertyAttribute;
            if (attr != null && attr.exclusive)
            {
                return 3;
            } else
            {
                return 2;
            }
		}

        protected override void OnGUI(SerializedProperty p, Rect bound)
        {
            int lineCount = GetLineCount(p);
            var bounds = bound.SplitByHeights(lineHeight);
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
			if (!editorPath.IsEmpty())
			{
				assetPath = EditorAssetUtil.GetAssetRelativePath(editorPath);
			}
			EditorGUI.indentLevel = p.depth;

            string refType = null;
            if (assetPath.IsResourcesPath()) {
                refType = "R";
            } else
            {
                refType = reference.objectReferenceValue != null? "O" : "X";
            }
			string title = string.Format("{0} ({1})", p.name, refType);

			Color oldColor = GUI.backgroundColor;
			if (DIGEST&&EditorAssetUtil.IsModified(path.stringValue, md5.stringValue))
			{
				GUI.backgroundColor = Color.red;
			}
			Object newRef = EditorGUI.ObjectField(bounds[lineNo], title, obj, typeof(Object), true);
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
                    Rect pathRect = bounds[lineNo];
                    EditorGUI.SelectableLabel(pathRect, path.stringValue);
                    lineNo++;
                }
            }
			GUI.backgroundColor = oldColor;

			if (lineNo < lineCount && cdn.boolValue == true)
			{
				// draw alias
				Rect aliasRect = bounds[lineNo];
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
