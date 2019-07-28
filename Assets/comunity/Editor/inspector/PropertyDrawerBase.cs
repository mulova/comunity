using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using commons;

namespace comunity
{
    public abstract class PropertyDrawerBase : PropertyDrawer
    {
        private const int lineHeight = 16;
        protected SerializedProperty prop;
        protected Rect bound;

        protected T GetAttribute<T>() where T:Attribute
        {
            return ReflectionUtil.GetAttribute<T>(prop.serializedObject.targetObject.GetType());
        }

        protected SerializedProperty GetProperty(string name)
        {
            return prop.FindPropertyRelative(name);
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            this.prop = prop;
            float height = GetLineCount() * lineHeight;
            this.prop = null;
            return height;
        }

        protected abstract int GetLineCount();

        protected abstract void DrawGUI(GUIContent label);

        protected Rect GetLineRect(int lineNo)
        {
            Rect lineRect = bound;
            lineRect.y += lineHeight * lineNo;
            lineRect.height = lineHeight;
            return lineRect;
        }

        protected Rect[] HorizontalSplitRect(Rect src, float leftWidth)
        {
            Rect left = src;
            Rect right = src;
            left.width = (src.width+EditorGUIUtility.currentViewWidth)*leftWidth;
            right.x = left.x+left.width;
            right.width = src.width+EditorGUIUtility.currentViewWidth-left.width;
            return new Rect[] { left, right};
        }

        protected Rect[] HorizontalSplit(Rect src, int count)
        {
            Rect[] rects = new Rect[count];
            float width = src.width / count;
            for (int i=0; i<count; ++i)
            {
                rects[i] = src;
                rects[i].x = src.x+i * width;
                rects[i].width = width;
            }
            return rects;
        }

        protected Rect[] HorizontalSplit(int line, float leftWidth)
        {
            return HorizontalSplitRect(GetLineRect(line), leftWidth);
        }

        protected bool DrawObjectField<T>(Rect r, GUIContent label, ref T o, bool allowSceneObj = true) where T:Object
        {
            Rect controlRect = EditorGUI.PrefixLabel(r, label);
            Rect refRect = controlRect;
            refRect.width = controlRect.width-15;

            T old = o as T;
            o = EditorGUI.ObjectField(refRect, o, typeof(T), allowSceneObj) as T;
            return o != old;
        }
        
        public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
        {
            this.prop = prop;
            this.bound = rect;
            Undo.RecordObject(prop.serializedObject.targetObject, GetType().FullName);
            EditorGUI.indentLevel = prop.depth;
            DrawGUI(label);
            this.prop = null;
        }

        protected void DrawArray(Rect pos, SerializedProperty p, params string[] propNames)
        {
            int length = p.arraySize;
            for (int i=0; i<length; i++)
            {
                float height = 1F / propNames.Length;
                Rect rect = GetRect(pos, 0F, i * height, 1F, height);
                SerializedProperty prop = p.GetArrayElementAtIndex(i);
                DrawRow(rect, prop, propNames);
            }
        }
        
        protected void DrawTitles(Rect pos, params string[] propNames)
        {
            for (int i=0; i<propNames.Length; i++)
            {
                float width = 1F / propNames.Length;
                DrawLabel(propNames[i], GetRect(pos, i * width, 0F, width, 1F));
            }
        }
        
        /**
     * Draw properties in a row without property names
     */
        protected void DrawRow(Rect pos, SerializedProperty p, params string[] propNames)
        {
            for (int i=0; i<propNames.Length; i++)
            {
                SerializedProperty prop = p.FindPropertyRelative(propNames[i]);
                float width = 1F / propNames.Length;
                Rect rect = GetRect(pos, i * width, 0F, width, 1F);
                DrawProperty(rect, prop, false);
            }
        }
        
        /**
     * Draw each properties line by line
     */
        protected void DrawProperties(Rect pos, SerializedProperty p, params string[] propNames)
        {
            for (int i=0; i<propNames.Length; i++)
            {
                SerializedProperty prop = p.FindPropertyRelative(propNames[i]);
                float height = 1F / propNames.Length;
                Rect rect = GetRect(pos, 0F, i * height, 1F, height);
                DrawProperty(rect, prop, true);
            }
        }
        
        protected void DrawProperty(Rect pos, SerializedProperty prop, bool title)
        {
            GUIContent label = GUIContent.none;
            if (title)
            {
                label = new GUIContent(prop.displayName);
            }
            if (prop.propertyType == SerializedPropertyType.AnimationCurve)
            {
                prop.animationCurveValue = EditorGUI.CurveField(pos, label, prop.animationCurveValue);
            } else if (prop.propertyType == SerializedPropertyType.ArraySize)
            {
            } else if (prop.propertyType == SerializedPropertyType.Boolean)
            {
                prop.boolValue = EditorGUI.Toggle(pos, label, prop.boolValue);
            } else if (prop.propertyType == SerializedPropertyType.Bounds)
            {
                prop.boundsValue = EditorGUI.BoundsField(pos, label, prop.boundsValue);
            } else if (prop.propertyType == SerializedPropertyType.Character)
            {
                prop.intValue = EditorGUI.IntField(pos, label, prop.intValue);
            } else if (prop.propertyType == SerializedPropertyType.Color)
            {
                prop.colorValue = EditorGUI.ColorField(pos, label, prop.colorValue);
            } else if (prop.propertyType == SerializedPropertyType.Enum)
            {
                string[] optionNames = prop.enumNames;
                GUIContent[] options = new GUIContent[optionNames.Length];
                for (int i=0; i<optionNames.Length; i++)
                {
                    options[i] = new GUIContent(optionNames[i]);
                }
                prop.enumValueIndex = EditorGUI.Popup(pos, label, prop.intValue, options);
            } else if (prop.propertyType == SerializedPropertyType.Float)
            {
                prop.floatValue = EditorGUI.FloatField(pos, label, prop.floatValue);
                //      } else if (prop.propertyType == SerializedPropertyType.Generic) {
                //          DrawGeneric(prop, pos);
            } else if (prop.propertyType == SerializedPropertyType.Gradient)
            {
            } else if (prop.propertyType == SerializedPropertyType.Integer)
            {
                prop.intValue = EditorGUI.IntField(pos, label, prop.intValue);
            } else if (prop.propertyType == SerializedPropertyType.LayerMask)
            {
                prop.intValue = EditorGUI.LayerField(pos, label, prop.intValue);
            } else if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                prop.objectReferenceValue = EditorGUI.ObjectField(pos, label, prop.objectReferenceValue, Type.GetType(prop.type), true);
            } else if (prop.propertyType == SerializedPropertyType.Rect)
            {
                prop.rectValue = EditorGUI.RectField(pos, label, prop.rectValue);
            } else if (prop.propertyType == SerializedPropertyType.String)
            {
                prop.stringValue = EditorGUI.TextField(pos, label, prop.stringValue);
            } else if (prop.propertyType == SerializedPropertyType.Vector2)
            {
                prop.vector2Value = EditorGUI.Vector2Field(pos, label.text, prop.vector2Value);
            } else if (prop.propertyType == SerializedPropertyType.Vector3)
            {
                prop.vector3Value = EditorGUI.Vector2Field(pos, label.text, prop.vector3Value);
            } else if (prop.propertyType == SerializedPropertyType.Generic)
            {
                if (prop.type == typeof(Quaternion).ToString())
                {
                    DrawVec4(prop, label, pos);
                }
            }
        }
        
        protected void DrawLabel(string title, Rect pos)
        {
            EditorGUI.SelectableLabel(pos, title);
        }
        
        protected void DrawVec4(SerializedProperty prop, GUIContent label, Rect pos)
        {
            Quaternion q = prop.quaternionValue;
            Vector4 vec4 = new Vector4(q.x, q.y, q.z, q.w);
            vec4 = EditorGUI.Vector4Field(pos, label.text, vec4);
            prop.quaternionValue = new Quaternion(vec4.x, vec4.y, vec4.z, vec4.w);
        }

        private Rect GetRect(Rect src, float x0, float y0, float width, float height)
        {
            return new Rect(MathUtil.Interpolate(x0, src.x, src.x+src.width), 
                            MathUtil.Interpolate(y0, src.y, src.y+src.height),
                            src.width * width,
                            src.height * height);
        }

        public static bool Popup<T>(Rect rect, ref T selection, IList<T> items)
        {
            bool ret = Popup<T>(rect, null, ref selection, items, ObjToString.DefaultToString);
            return ret;
        }

        public static bool Popup<T>(Rect rect, string label, ref T selection, IList<T> items)
        {
            return Popup<T>(rect, label, ref selection, items, ObjToString.DefaultToString);
        }

        public static bool Popup<T>(Rect rect, string label, ref T selection, IList<T> items, ToString toString)
        {
            return Popup<T>(rect, label, ref selection, items, toString, null);
        }

        public static bool Popup<T>(Rect rect, string label, ref T selection, IList<T> items, ToString toString, GUIStyle style)
        {
            if (items.Count == 0)
            {
                return false;
            }
            int index = -1;
            string[] str = new string[items.Count];
            for (int i=0; i<items.Count; i++)
            {
                str[i] = toString(items[i]);
                if (object.ReferenceEquals(items[i], selection)||object.Equals(items[i], selection))
                {
                    index = i;
                }
            }
            int newIndex = 0;

            Rect controlRect = label != null? EditorGUI.PrefixLabel(rect, new GUIContent(label)) : rect;

            // Show if current value is not the member of popup list
            Color c = GUI.contentColor;
            if (index < 0&&selection != null)
            {
                GUI.contentColor = Color.red;
                index = 0;
            }
            GUI.contentColor = c;
            if (style != null)
            {
                newIndex = EditorGUI.Popup(controlRect, index, str, style);
            } else
            {
                newIndex = EditorGUI.Popup(controlRect, index, str);
            }
            if (index != newIndex)
            {
                selection = items[newIndex];
                return true;
            }
            return false;
        }

        public static bool PopupNullable<T>(Rect rect, string label, ref T selection, IList<T> items)
        {
            T sel = selection;
            bool changed = PopupNullable<T>(rect, label, ref sel, items, ObjToString.ScenePathToString);
            selection = sel;
            return changed;
        }

        public static bool PopupNullable<T>(Rect rect, string label, ref T selection, IList<T> items, ToString toString)
        {
            Rect controlRect = label != null? EditorGUI.PrefixLabel(rect, new GUIContent(label)) : rect;
            if (items.Count == 0)
            {
                bool changed = false;
                if (selection != null&&!string.Empty.Equals(selection)&&!selection.Equals(default(T)))
                {
                    selection = default(T);
                    changed = true;
                }
                EditorGUI.Popup(controlRect, 0, new string[] {"-"});
                return changed;
            }


            int index = 0;
            string[] str = new string[items.Count+1];
            str[0] = "-";
            for (int i=1; i<= items.Count; i++)
            {
                str[i] = toString(items[i-1]);
                if (object.Equals(items[i-1], selection))
                {
                    index = i;
                }
            }
            int newIndex = EditorGUI.Popup(controlRect, index, str);
            if (newIndex == 0)
            {
                selection = default(T);
            } else
            {
                selection = items[newIndex-1];
            }
            return newIndex != index;
        }

        public static bool TextField(Rect r, string label, SerializedProperty p)
        {
            return TextField(r, label, p, EditorStyles.textField);
        }

        public static bool TextField(Rect r, string label, SerializedProperty p, GUIStyle style)
        {
            string str = p.stringValue;
            string newStr = label == null?
                EditorGUI.TextField(r, str, style) :
                EditorGUI.TextField(r, label, str, style);
            if (newStr != str)
            {
                p.stringValue = newStr;
                return true;
            }
            return false;
        }

        public static void LabelField(Rect r, SerializedProperty p)
        {
            LabelField(r, p, EditorStyles.label);
        }

        public static void LabelField(Rect r, SerializedProperty p, GUIStyle style)
        {
            string str = p.stringValue;
            EditorGUI.LabelField(r, str, style);
        }
    }
}

