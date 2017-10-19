using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

using editor.ex;
using System.Reflection;
using System.Linq;
using System.Collections;
using UnityEditor.SceneManagement;
using commons;

namespace core
{
//	[CustomPropertyDrawer(typeof(EnumClass))]
	public abstract class EnumClassDrawer<T> : PropertyDrawerBase where T: class
	{
		protected override int GetLineCount()
		{
			return 1;
		}

		protected override void DrawGUI(GUIContent label)
		{
            prop.serializedObject.Update();
            IList<T> values = GetValues();
            object target = GetParent(prop);
            if (target == null)
            {
                return;
            }
            T o = ReflectionUtil.GetFieldValue<T>(target, prop.name);
            if (PopupNullable(GetLineRect(0), prop.displayName, ref o, values)) {
                ReflectionUtil.SetFieldValue(target, prop.name, o);
                prop.serializedObject.ApplyModifiedProperties();
                CompatibilityEditor.SetDirty(prop.serializedObject.targetObject);
                EditorSceneManager.SaveOpenScenes();
			}
		}

		protected abstract IList<T> GetValues();

        public object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach(var element in elements.Take(elements.Length-1))
            {
                if(element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[","").Replace("]",""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        public object GetValue(object source, string name)
        {
            if(source == null)
                return null;
            Type type = source.GetType();
            FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if(f == null)
            {
                PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if(p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        public object GetValue(object source, string name, int index)
        {
            IEnumerable enumerable = GetValue(source, name) as IEnumerable;
            IEnumerator enm = enumerable.GetEnumerator();
            while(index-- >= 0)
            {
                if (!enm.MoveNext())
                {
                    return null;
                }
            }
            return enm.Current;
        }

	}
}

