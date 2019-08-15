using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;
using mulova.commons;
using System;
using Object = UnityEngine.Object;
using System.Text.Ex;
using UnityEngine.Ex;

namespace comunity
{
    [System.Serializable]
    public class UnityObjId
    {
        [SerializeField] public string _id;
        [SerializeField] private bool _asset;
        [SerializeField] private Type _type;

        // guid for asset, scene path for scene object
        public string id { get { return _id; } private set { this._id = value; } }
        public bool asset { get { return _asset; } private set { _asset = value; } }
        public Type type { get { return _type; } private set { _type = value; } }

        public Object reference
        {
            get
            {
                if (asset)
                {
                    string uid = AssetDatabase.GUIDToAssetPath(id);
                    if (!uid.IsEmpty())
                    {
                        return AssetDatabase.LoadAssetAtPath(uid, _type);
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
                        } else
                        {
                            return t.GetComponent(type);
                        }
                    }
                    return t;
                }
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
                this.type = value.GetType();
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

        public UnityObjId(Object o)
        {
            if (o != null)
            {
                reference = o;
                type = o.GetType();
            }
        }

        public override bool Equals(object obj)
        {
            UnityObjId that = obj as UnityObjId;
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
            return id;
        }
    }
}
