using System.Reflection;
using Object = UnityEngine.Object;
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using mulova.commons;
using comunity;
using System.Ex;
using System.Text.Ex;

namespace convinity
{
    public class RefSearchData
    {
        public readonly string assetGuid;
        public readonly ScenePath scenePath;
        public readonly int compIndex;
        public FieldInfo field;
        public PropertyInfo prop;
        public readonly Object obj;
        public readonly string assetPath;

        internal Type declaringType
        {
            get
            {
                if (field != null)
                {
                    return field.DeclaringType;
                } else
                {
                    return prop.DeclaringType;
                }
            }
        }

        public string signature
        {
            get
            {
                if (field != null)
                {
                    return string.Concat(field.DeclaringType.Name, ".", field.Name);
                } else
                {
                    return string.Concat(prop.DeclaringType.Name, ".", prop.Name);
                }
            }
        }

        public RefSearchData(Object o)
        {
            this.obj = o;
            assetPath = AssetDatabase.GetAssetPath(o);
            if (o is Component)
            {
                compIndex = (o as Component).gameObject.GetComponents(o.GetType()).FindIndex(c => c == o);
            }
            if (assetPath.IsEmpty())
            {
                assetPath = EditorSceneManager.GetActiveScene().path;
                scenePath = new ScenePath(o);
            }
            assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
        }

        public RefSearchData(Object o, FieldInfo f) : this(o)
        {
            this.field = f;
        }

        public RefSearchData(Object o, PropertyInfo p) : this(o)
        {
            this.prop = p;
        }

        public void SetValue(Object val)
        {
            if (field != null)
            {
                field.SetValue(obj, val);
            } else
            {
                prop.SetValue(obj, val, null);
            }
            EditorUtil.SetDirty(obj);
        }

        public override bool Equals(object thatObj)
        {
            RefSearchData that = thatObj as RefSearchData;
            if (that == null)
            {
                return false;
            }
            return this.obj == that.obj;
        }

        public override int GetHashCode()
        {
            return obj.GetHashCode();
        }

        public override string ToString()
        {
            string path = scenePath == null? assetPath : string.Format("'{0}'{1}", assetPath, scenePath);
            return string.Format("{0} [{1}]", path, signature);
        }
    }
}