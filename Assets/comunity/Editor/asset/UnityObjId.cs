using UnityEngine;
using System.Collections;
using comunity;
using UnityEditor;
using commons;

namespace comunity
{
    [System.Serializable]
    public class UnityObjId
    {
        public string id { get; private set; } // guid for asset, scene path for scene object
        public bool asset { get; private set; }
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
    }
}
