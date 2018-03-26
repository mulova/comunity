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
    public class UnityObjId : IComparable<UnityObjId>
    {
        [SerializeField]
        private string _id;
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

        [SerializeField]
        private bool _asset;
        public bool asset { get { return _asset; } private set { _asset = value; } }

        public bool starred;

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
                    } else
                    {
                        return null;
                    }
                } else
                {
                    return TransformUtil.Search(id);
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
                } else
                {
                    return id;
                }
            }
        }

        public UnityObjId(Object o)
        {
            string assetPath = AssetDatabase.GetAssetPath(o);
            if (assetPath.IsNotEmpty())
            {
                this.id = AssetDatabase.AssetPathToGUID(assetPath);
                this.asset = true;
            } else
            {
                if (o is GameObject)
                {
                    this.id = (o as GameObject).transform.GetScenePath();
                } else if (o is Component)
                {
                    this.id = (o as Component).transform.GetScenePath();
                }
                this.asset = false;
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

        public int CompareTo(UnityObjId other)
        {
            if (this.starred^other.starred)
            {
                return this.starred? -1 : 1;
            } else
            {
                return 0;
            }
        }
    }
}
