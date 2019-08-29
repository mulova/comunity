using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace comunity
{
    [CustomEditor(typeof(ObjSwitch))]
	public class ObjSwitchInspector : Editor
	{
		private ObjSwitch objSwitch;
		internal static bool exclusive = true;


        void OnEnable()
		{
			objSwitch = (ObjSwitch)target;
		}
	
		public override void OnInspectorGUI()
		{
            ListDrawer<GameObject> allDrawer = new ListDrawer<GameObject>(objSwitch.GetAllObjects());
            allDrawer.flags = Rotorz.Games.Collections.ReorderableListFlags.HideAddButton;
            allDrawer.onRemove += OnRemoveObject;
            allDrawer.Draw();
            DrawDefaultInspector();
		}

        private void OnRemoveObject(int i, GameObject o)
        {
            if (objSwitch.Remove(o))
            {
                EditorUtil.SetDirty(objSwitch);
            }
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
            var bounds = bound.SplitByHeights(lineHeight);
            var n = p.FindPropertyRelative("name");
            EditorGUI.PropertyField(bounds[0], n);
            var objBound = bounds[1];
            objBound.height = objs.drawer.GetHeight();
            objs.Draw(objBound);
            var tBound = objBound;
            tBound.height = trans.drawer.GetHeight();
            tBound.y += objBound.height;
            trans.Draw(tBound);
            var pBound = tBound;
            pBound.height = pos.drawer.GetHeight();
            pBound.y += tBound.height;
            pos.Draw(pBound);
        }

        public override float GetPropertyHeight(SerializedProperty p, GUIContent label)
        {
            if (objs == null)
            {
                objs = new ReorderSerialized<GameObject>(p.FindPropertyRelative("objs"));
                trans = new ReorderSerialized<Transform>(p.FindPropertyRelative("trans"));
                pos = new ReorderSerialized<Vector3>(p.FindPropertyRelative("pos"));
            }
            var nameHeight = PropertyDrawerBase.lineHeight;
            return objs.drawer.GetHeight() + trans.drawer.GetHeight() + pos.drawer.GetHeight()+nameHeight;
         }
    }
}