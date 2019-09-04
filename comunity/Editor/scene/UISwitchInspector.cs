using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic.Ex;
using System.Text.Ex;

namespace comunity
{
    [CustomEditor(typeof(UISwitch))]
    public class UISwitchInspector : Editor
    {
        private UISwitch uiSwitch;
        internal static bool exclusive = true;


        void OnEnable()
        {
            uiSwitch = (UISwitch)target;
        }

        public override void OnInspectorGUI()
        {
#if AUTO_REMOVE
            RemoveUnused();
#endif
            DrawDefaultInspector();
        }

        public static bool[] GetVisibilityUnion(UISwitch uiSwitch)
        {
            bool[] union = new bool[uiSwitch.objs.Count];
            HashSet<List<bool>> set = new HashSet<List<bool>>();
            for (int i = 0; i < uiSwitch.switches.Count; ++i)
            {
                var s = uiSwitch.switches[i];
                if (set.Contains(s.visibility))
                {
                    uiSwitch.switches[i] = new UISwitchSect();
                    EditorUtil.SetDirty(uiSwitch);
                }
                set.Add(uiSwitch.switches[i].visibility);
                for (int j = 0; j < s.visibility.Count; ++j)
                {
                    union[j] |= s.visibility[j];
                }
            }
            return union;
        }

        private void RemoveUnused()
        {
            bool[] union = GetVisibilityUnion(uiSwitch);
            for (int i = union.Length - 1; i >= 0; --i)
            {
                if (!union[i])
                {
                    uiSwitch.objs.RemoveAt(i);
                    for (int j = 0; j < uiSwitch.switches.Count; ++j)
                    {
                        uiSwitch.switches[j].visibility.RemoveAt(i);
                    }
                    EditorUtil.SetDirty(uiSwitch);
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(UISwitchSect))]
    public class UISwitchSectDrawer : PropertyDrawerBase
    {
        private Dictionary<string, ReorderSerialized<bool>> vPool = new Dictionary<string, ReorderSerialized<bool>>();

        private ReorderSerialized<bool> GetVisibility(SerializedProperty p)
        {
            var v = vPool.Get(p.propertyPath);
            if (v == null)
            {
                var prop = p.FindPropertyRelative("visibility");
                v = new ReorderSerialized<bool>(prop);
                v.drawItem = OnDrawVisibility;
                
                v.onAdd = i => OnAddObject(prop, i);
                v.onRemove = i => OnRemoveObject(prop, i);
                v.onReorder = (i1, i2) => OnReorderObject(prop, i1, i2);
                v.canAdd = () =>
                {
                    var objs = p.serializedObject.FindProperty("objs");
                    return !IsDuplicate(objs, Selection.activeObject);
                };
                vPool[p.propertyPath] = v;
            }
            return v;
        }

        private ReorderSerialized<Transform> trans;
        private ReorderSerialized<Vector3> pos;

        protected override void DrawProperty(SerializedProperty p, Rect bound)
        {
            // Draw Name
            var bounds = bound.SplitByHeights(lineHeight);
            var n = p.FindPropertyRelative("name");
            var nameBounds = bounds[0].SplitByWidths(50);
            Color c = GUI.contentColor;
            if (n.stringValue.IsEmpty())
            {
                GUI.color = Color.red;
            }
            if (GUI.Button(nameBounds[0], new GUIContent("ID")))
            {
                var script = p.serializedObject.targetObject as UISwitch;
                script.Set(n.stringValue);
            }
            EditorGUI.PropertyField(nameBounds[1], n, new GUIContent(""));
            GUI.color = c;

            // Draw Visibility
            var visibility = GetVisibility(p);
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
            var separator = 10;
            trans = new ReorderSerialized<Transform>(p.FindPropertyRelative("trans"));
            pos = new ReorderSerialized<Vector3>(p.FindPropertyRelative("pos"));
            return visibility.drawer.GetHeight() + trans.drawer.GetHeight() + pos.drawer.GetHeight()+ lineHeight + separator;
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
            var union = UISwitchInspector.GetVisibilityUnion(item.serializedObject.targetObject as UISwitch);
            Color color = GUI.contentColor;
            if (!union[index])
            {
                GUI.contentColor = Color.gray;
            }
            var bounds = rect.SplitByWidths(25, 25);
            // draw index
            EditorGUI.PrefixLabel(bounds[0], new GUIContent(index.ToString()));
            // draw bool
            EditorGUI.PropertyField(bounds[1], item, new GUIContent(""));
            var objs = item.serializedObject.FindProperty("objs");
            var obj = objs.GetArrayElementAtIndex(index);
            var oldObj = obj.objectReferenceValue;
            EditorGUI.PropertyField(bounds[2], obj, new GUIContent(""));
            if (obj.objectReferenceValue == null || IsDuplicate(objs, obj.objectReferenceValue))
            {
                obj.objectReferenceValue = oldObj;
            }
            GUI.contentColor = color;
        }

        private void OnReorderObject(SerializedProperty p, int i1, int i2)
        {
            // reorder objs
            var objs = p.serializedObject.FindProperty("objs");
            objs.MoveArrayElement(i1, i2);
            p.serializedObject.ApplyModifiedProperties();

            // reorder visibility element
            var switches = p.serializedObject.FindProperty("switches");
            for (int i = 0; i < switches.arraySize; ++i)
            {
                var s = switches.GetArrayElementAtIndex(i);
                var visibility = s.FindPropertyRelative("visibility");
                if (p.propertyPath != visibility.propertyPath)
                {
                    visibility.MoveArrayElement(i1, i2); 
                }
            }
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
                if (p.propertyPath != visibility.propertyPath)
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

            // Remove visibility element
            var switches = p.serializedObject.FindProperty("switches");
            for (int i = 0; i < switches.arraySize; ++i)
            {
                var s = switches.GetArrayElementAtIndex(i);
                var visibility = s.FindPropertyRelative("visibility");
                if (p.propertyPath != visibility.propertyPath)
                {
                    visibility.DeleteArrayElementAtIndex(index);
                }
            }
            p.serializedObject.ApplyModifiedProperties();
        }
    }
}