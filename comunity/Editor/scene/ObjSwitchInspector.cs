using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace comunity
{
    [CustomEditor(typeof(ObjSwitch))]
	public class ObjSwitchInspector : Editor
	{
		private ObjSwitch objSwitch;
        private ReorderProperty<ObjSwitch> elementInspector;
		private ReorderProperty<ObjSwitch> presetInspector;
		internal static bool exclusive = true;
        private SerializedObject ser;


        void OnEnable()
		{
            ser = new SerializedObject(target);
			objSwitch = (ObjSwitch)target;
            elementInspector = new ReorderProperty<ObjSwitch>(ser, nameof(objSwitch.switches));
            presetInspector = new ReorderProperty<ObjSwitch>(ser, nameof(objSwitch.preset));
		}
	
		public override void OnInspectorGUI()
		{
            ser.Update();
            elementInspector.Draw();
            presetInspector.Draw();
            ser.ApplyModifiedProperties();

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
        protected override void DrawGUI(SerializedProperty p)
        {
            var n = p.FindPropertyRelative("name");
            var objs = p.FindPropertyRelative("objs");
            var trans = p.FindPropertyRelative("trans");
            var pos = p.FindPropertyRelative("pos");

            EditorGUILayout.PropertyField(n);
            EditorGUILayout.PropertyField(objs);
            EditorGUILayout.PropertyField(trans);
            EditorGUILayout.PropertyField(pos);
        }

        protected override int GetLineCount(SerializedProperty p)
        {
            var objs = GetProperty("objs");
            var trans = GetProperty("trans");
            var pos = GetProperty("pos");

            var size = objs.arraySize + trans.arraySize + pos.arraySize;
            return size;
        }
    }
}