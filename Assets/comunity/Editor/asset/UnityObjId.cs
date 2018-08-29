using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;
using commons;
using System;
using Object = UnityEngine.Object;

namespace comunity
{
    [System.Serializable]
    public class UnityObjId
    {
        [SerializeField] private string _id;
        [SerializeField] private bool _asset;

        // guid for asset, scene path for scene object
        public string id
        {
            get
            {
                return _id;
            }
            private set
            {
                this._id = value;
            }
        }

        public bool asset { get { return _asset; } private set { _asset = value; } }

        public Object reference
        {
            get
            {
                if (asset)
                {
                    string uid = AssetDatabase.GUIDToAssetPath(id);
                    if (uid.IsNotEmpty())
                    {
                        return AssetDatabase.LoadAssetAtPath<Object>(uid);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return TransformUtil.Search(id);
                }
            }

            set
            {
                string assetPath = AssetDatabase.GetAssetPath(value);
                if (assetPath.IsNotEmpty())
                {
                    this.id = AssetDatabase.AssetPathToGUID(assetPath);
                    this.asset = true;
                }
                else
                {
                    if (value is GameObject)
                    {
                        this.id = (value as GameObject).transform.GetScenePath();
                    }
                    else if (value is Component)
                    {
                        this.id = (value as Component).transform.GetScenePath();
                    }
                    this.asset = false;
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

        public UnityObjId(Object o)
        {
            reference = o;
        }

        public UnityObjId(string guid)
        {
            id = guid;
            asset = true;
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
