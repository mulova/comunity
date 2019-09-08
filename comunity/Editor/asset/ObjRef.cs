using UnityEngine;
using UnityEditor;
using mulova.commons;
using System;
using Object = UnityEngine.Object;
using System.Text.Ex;
using UnityEngine.Ex;

namespace mulova.comunity
{
    [Serializable]
    public class ObjRef
    {
        [SerializeField] private string _id;
        [SerializeField] private bool _asset;
        [SerializeField] private string _type;

        // guid for asset, scene path for scene object
        public string id { get { return _id; } private set { this._id = value; } }
        public bool asset { get { return _asset; } private set { _asset = value; } }
        public Type type { get { return !_type.IsEmpty()? ReflectionUtil.GetType(_type): null; } private set { _type = value.FullName; } }

        public Object reference
        {
            get
            {
                return GetReference(_id, _asset, _type);
            }

            set
            {
                string assetPath = AssetDatabase.GetAssetPath(value);
                if (!assetPath.IsEmpty())
                {
                    this.id = AssetDatabase.AssetPathToGUID(assetPath);
                    this.asset = true;
                }
                else
                {
                    var go = value.GetGameObject();
                    if (go != null)
                    {
                        this.id = go.transform.GetScenePath();
                    }
                    this.asset = false;
                }
                if (value != null)
                {
                    this.type = value.GetType();
                }
            }
        }

        public string path
        {
            get
            {
                if (asset)
                {
                    return AssetDatabase.GUIDToAssetPath(id);
                }
                else
                {
                    return id;
                }
            }
        }

        public ObjRef(Object o)
        {
            if (o != null)
            {
                reference = o;
            }
        }


        public static Object GetReference(string id, bool isAsset, string typeStr)
        {
            var type = ReflectionUtil.GetType(typeStr);
            if (isAsset)
            {
                string uid = AssetDatabase.GUIDToAssetPath(id);
                if (!uid.IsEmpty())
                {
                    return AssetDatabase.LoadAssetAtPath(uid, type);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var t = TransformUtil.Search(id);
                if (t != null)
                {
                    if (type == typeof(GameObject))
                    {
                        return t.gameObject;
                    }
                    else if (type != null)
                    {
                        return t.GetComponent(type);
                    }
                }
                return t;
            }
        }

        public override bool Equals(object obj)
        {
            ObjRef that = obj as ObjRef;
            if (that != null)
            {
                return this.id == that.id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (this.id == null)
            {
                return 0;
            }
            return this.id.GetHashCode();
        }

        public override string ToString()
        {
            return path;
        }
    }
}
