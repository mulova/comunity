using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using commons;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditorInternal;

namespace comunity
{
    [CustomEditor(typeof(ObjSwitch))]
	public class ObjSwitchInspector : Editor
	{
		private ObjSwitch objSwitch;
        private ReorderableListEx elementInspector;
		private ReorderableListEx presetInspector;
		internal static bool exclusive = true;
        private SerializedObject obj;


        void OnEnable()
		{
            obj = new SerializedObject(target);
			objSwitch = (ObjSwitch)target;
            elementInspector = new ReorderableListEx(nameof(objSwitch.switches), obj);
            presetInspector = new ReorderableListEx(nameof(objSwitch.preset), obj);
		}
	
		public override void OnInspectorGUI()
		{
            obj.Update();
            elementInspector.DoLayoutList();
            presetInspector.DoLayoutList();
            obj.ApplyModifiedProperties();

			//EditorGUIUtil.Toggle("Exclusive(Editor Only)", ref exclusive);
			//changed |= elementInspector.OnInspectorGUI();
			//if (EditorUI.DrawHeader("Preset")) {
			//	EditorUI.BeginContents();
			//	changed |= presetInspector.OnInspectorGUI();
			//	if (changed) {
			//		presetInspector.SetPresetKeys(GetAllKeys());
   //                 EditorUtil.SetDirty(objSwitch);
			//	}
			//	EditorUI.EndContents();
			//}
		}

		private string[] GetAllKeys()
		{
			List<string> keys = new List<string>();
			foreach (ObjSwitchElement e in objSwitch.switches) {
				keys.Add(e.name);
			}
			return keys.ToArray();
		}
	}

    [CustomPropertyDrawer(typeof(ObjSwitchElement))]
    public class ObSwitchElementDrawer : PropertyDrawerBase
    {
        protected override void DrawGUI(GUIContent label)
        {
            var n = GetProperty("name");
            var objs = GetProperty("objs");
            var trans = GetProperty("trans");
            var pos = GetProperty("pos");

            EditorGUILayout.PropertyField(n);
            EditorGUILayout.PropertyField(objs);
            EditorGUILayout.PropertyField(trans);
            EditorGUILayout.PropertyField(pos);
        }

        protected override int GetLineCount()
        {
            var objs = GetProperty("objs");
            var trans = GetProperty("trans");
            var pos = GetProperty("pos");

            var size = objs.arraySize + trans.arraySize + pos.arraySize;
            return size;
        }
    }
}