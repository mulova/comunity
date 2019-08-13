using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace comunity
{
    //[CustomEditor(typeof(ObjSwitch))]
	public class ObjSwitchInspector : Editor
	{
		private ObjSwitch objSwitch;
        private ReorderSerialized<ObjSwitch> elementInspector;
		private ReorderSerialized<ObjSwitch> presetInspector;
		internal static bool exclusive = true;
        private SerializedObject ser;


        void OnEnable()
		{
            ser = new SerializedObject(target);
			objSwitch = (ObjSwitch)target;
            elementInspector = new ReorderSerialized<ObjSwitch>(ser, nameof(objSwitch.switches));
            presetInspector = new ReorderSerialized<ObjSwitch>(ser, nameof(objSwitch.preset));
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
        private ReorderSerialized<GameObject> objs;
        private ReorderSerialized<Transform> trans;
        private ReorderSerialized<Vector3> pos;

        protected override void DrawProperty(SerializedProperty p, Rect bound)
        {
            var n = p.FindPropertyRelative("name");
            if (objs == null)
            {
                objs = new ReorderSerialized<GameObject>(p.FindPropertyRelative("objs"));
                trans = new ReorderSerialized<Transform>(p.FindPropertyRelative("trans"));
                pos = new ReorderSerialized<Vector3>(p.FindPropertyRelative("pos"));
            }

            EditorGUI.PropertyField(GetLineRect(0), n);
            objs.Draw(bound);
            trans.Draw(bound);
            pos.Draw(bound);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 200;
        }
    }
}