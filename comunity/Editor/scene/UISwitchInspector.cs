using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;

namespace comunity
{
    [CustomEditor(typeof(UISwitch))]
    public class UISwitchInspector : Editor
    {
        private UISwitch objSwitch;
        internal static bool exclusive = true;


        void OnEnable()
        {
            objSwitch = (UISwitch)target;
        }

        public override void OnInspectorGUI()
        {
            CheckDuplicate();
            DrawDefaultInspector();
        }

        private void CheckDuplicate()
        {
            bool[] union = new bool[objSwitch.objs.Count];
            HashSet<List<bool>> set = new HashSet<List<bool>>();
            for (int i = 0; i < objSwitch.switches.Count; ++i)
            {
                var s = objSwitch.switches[i];
                if (set.Contains(s.visibility))
                {
                    objSwitch.switches[i] = new UISwitchSect();
                    EditorUtil.SetDirty(objSwitch);
                }
                set.Add(objSwitch.switches[i].visibility);
                for (int j=0; j<s.visibility.Count; ++j)
                {
                    union[j] |= s.visibility[j];
                }
                Debug.Log($"{i} hash: {objSwitch.switches[i].visibility.GetHashCode()} {objSwitch.switches[i].visibility.Count}");
            }
            for (int i = union.Length - 1; i >= 0; --i)
            {
                if (!union[i])
                {
                    objSwitch.objs.RemoveAt(i);
                    for (int j = 0; j < objSwitch.switches.Count; ++j)
                    {
                        objSwitch.switches[j].visibility.RemoveAt(i);
                    }
                    EditorUtil.SetDirty(objSwitch);
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(UISwitchSect))]
    public class UISwitchSectDrawer : PropertyDrawerBase
    {
        private ReorderSerialized<bool> GetVisibility(SerializedProperty p)
        {
            var prop = p.FindPropertyRelative("visibility");
            var visibility = new ReorderSerialized<bool>(prop);
            visibility.drawItem = OnDrawVisibility;

            visibility.onAdd = i => OnAddObject(prop, i);
            visibility.onRemove = i => OnRemoveObject(prop, i);
            visibility.onReorder = (i1, i2) => OnReorderObject(prop, i1, i2);
            visibility.canAdd = () =>
            {
                var objs = p.serializedObject.FindProperty("objs");
                return !IsDuplicate(objs, Selection.activeObject);
            };
            return visibility;
        }

        private ReorderSerialized<Transform> trans;
        private ReorderSerialized<Vector3> pos;

        protected override void DrawProperty(SerializedProperty p, Rect bound)
        {
            var visibility = GetVisibility(p);
            var bounds = bound.SplitByHeights(lineHeight);
            var n = p.FindPropertyRelative("name");
            var nameBounds = bounds[0].SplitByWidths(50);
            if (GUI.Button(nameBounds[0], new GUIContent("ID")))
            {
                var script = p.serializedObject.targetObject as UISwitch;
                script.Set(n.stringValue);
            }
            EditorGUI.PropertyField(nameBounds[1], n, new GUIContent(""));
            bounds[1].x += 30;
            bounds[1].width -= 30;
            var objBound = bounds[1];
            objBound.height = visibility.drawer.GetHeight();
            visibility.Draw(objBound);

            // Draw Visibility
            var tBound = objBound;
            tBound.height = trans.drawer.GetHeight();
            tBound.y += objBound.height;
            trans.Draw(tBound);
            var pBound = tBound;
            pBound.height = pos.drawer.GetHeight();
            pBound.y += tBound.height;
            pos.Draw(pBound);
        }

        //public override bool CanCacheInspectorGUI(SerializedProperty property)
        //{
        //    return false;
        //}

        public override float GetPropertyHeight(SerializedProperty p, GUIContent label)
        {
            var visibility = GetVisibility(p);
            trans = new ReorderSerialized<Transform>(p.FindPropertyRelative("trans"));
            pos = new ReorderSerialized<Vector3>(p.FindPropertyRelative("pos"));
            return visibility.drawer.GetHeight() + trans.drawer.GetHeight() + pos.drawer.GetHeight()+ lineHeight;
        }

        private bool IsDuplicate(SerializedProperty arr, Object o)
        {
            for (int i = 0; i < arr.arraySize; ++i)
            {
                var a = arr.GetArrayElementAtIndex(i);
                if (a.objectReferenceValue == Selection.activeObject)
                {
                    return true;
                }
            }
            return false;
        }

        private void OnDrawVisibility(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
        {
            var bounds = rect.SplitByWidths(25, 25);
            EditorGUI.PrefixLabel(bounds[0], new GUIContent(index.ToString()));
            EditorGUI.PropertyField(bounds[1], item, new GUIContent(""));
            var objs = item.serializedObject.FindProperty("objs");
            var obj = objs.GetArrayElementAtIndex(index);
            var oldObj = obj.objectReferenceValue;
            EditorGUI.PropertyField(bounds[2], obj, new GUIContent(""));
            if (obj.objectReferenceValue == null || IsDuplicate(objs, obj.objectReferenceValue))
            {
                obj.objectReferenceValue = oldObj;
            }
        }

        private void OnReorderObject(SerializedProperty p, int i1, int i2)
        {
            var objs = p.serializedObject.FindProperty("objs");
            objs.MoveArrayElement(i1, i2);
            p.serializedObject.ApplyModifiedProperties();
        }

        private void OnAddObject(SerializedProperty p, int index)
        {
            p.GetArrayElementAtIndex(index).boolValue = true;
            // Add object to "objs"
            var objs = p.serializedObject.FindProperty("objs");
            objs.InsertArrayElementAtIndex(index);
            var item = objs.GetArrayElementAtIndex(index);
            item.objectReferenceValue = Selection.activeObject;

            // Add visibility element
            var switches = p.serializedObject.FindProperty("switches");
            for (int i=0; i<switches.arraySize; ++i)
            {
                var s = switches.GetArrayElementAtIndex(i);
                var visibility = s.FindPropertyRelative("visibility");
                if (visibility.arraySize == objs.arraySize-1)
                {
                    visibility.InsertArrayElementAtIndex(index);
                    visibility.GetArrayElementAtIndex(index).boolValue = false;
                }
            }
            p.serializedObject.ApplyModifiedProperties();
        }

        private void OnRemoveObject(SerializedProperty p, int index)
        {
            var objs = p.serializedObject.FindProperty("objs");
            objs.DeleteArrayElementAtIndex(index); // clear reference
            objs.DeleteArrayElementAtIndex(index); // remove index
            p.serializedObject.ApplyModifiedProperties();
        }
    }
}