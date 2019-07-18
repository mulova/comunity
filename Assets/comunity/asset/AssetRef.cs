using System;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Serialization;
using commons;
using System.Text.Ex;

namespace comunity
{
    /// <summary>
    /// Assets in Resources/ directory is not strongly referenced.
    /// </summary>
    [System.Serializable]
    public class AssetRef
    {
        public bool cdn = true;
        public string path;
        public string guid;
        public Object reference;
        public string md5;
        public string exclusiveAssetId;
        [System.NonSerialized] 
        private Object tempRef;

        public AssetRef()
        {
        }

        public AssetRef(AssetRef src)
        {
            this.cdn = src.cdn;
            this.path = src.path;
            this.guid = src.guid;
            this.reference = src.reference;
            this.md5 = src.md5;
        }

        public Object GetReference()
        {
            if (reference != null)
            {
                return reference;
            }
            if (tempRef == null&&!cdn&&path.IsResourcesPath())
            {
                tempRef = Resources.Load(path.GetResourcesPath());
            }
            return tempRef;
        }

        public void LoadAsset<T>(Action<T> callback) where T: Object
        {
            T o = GetReference() as T;
            if (o != null)
            {
                callback(o);
            } else if (cdn)
            {
                Cdn.cache.GetAsset<T>(exclusiveAssetId, path, callback);
            } else
            {
                callback(null);
            }
        }

        public string actualPath
        {
            get
            {
                if (path.IsEmpty())
                {
                    return string.Empty;
                }
                if (cdn&&path.IsNotEmpty())
                {
                    return PathUtil.Combine(Cdn.Path, path);
                } else if (path.IsResourcesPath())
                {
                    return path.GetResourcesPath();
                }
                return path;
            }
        }

        public bool isStrong
        {
            get
            {
                return reference != null||IsStrong(cdn, path);
            }
        }

        public bool isEmpty
        {
            get
            {
                return path.IsEmpty()&&reference == null;
            }
        }

        public void Clear()
        {
            path = string.Empty;
            reference = null;
        }

        public static bool IsStrong(bool cdn, string path)
        {
            return !(cdn||path.IsResourcesPath());
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o is AssetRef)
            {
                AssetRef that = o as AssetRef;
                return this.cdn == that.cdn&&this.path == that.path;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = cdn.GetHashCode();
            if (path != null)
            {
                hash += path.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            return path;
        }
    }

}
