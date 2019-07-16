using System;
using UnityEditor;
using Object = UnityEngine.Object;
using commons;

namespace comunity
{
    public static class AssetRefEditorEx
    {
        public static string GetPath(Object o)
        {
            string assetPath = AssetDatabase.GetAssetPath(o);
            if (assetPath.IsNotEmpty())
            {
                Assert.IsTrue(assetPath.StartsWith("Assets/"));
                string path = assetPath.Substring("Assets/".Length);
                if (AssetBundlePath.inst.IsRawCdnAsset(o))
                {
                    return path;
                } else if (AssetBundlePath.inst.IsCdnAsset(o))
                {
                    return PathUtil.ReplaceExtension(path, FileTypeEx.ASSET_BUNDLE);
                } else
                {
                    return path;
                }
            } else
            {
                return string.Empty;
            }
        }
        
        public static void SetPath(this AssetRef r, Object o)
        {
            r.path = GetPath(o);
            r.cdn = AssetBundlePath.inst.IsRawCdnAsset(o);
            r.guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(o));
        }
        
        public static string GetEditorPath(this AssetRef r)
        {
            if (r.guid.IsNotEmpty())
            {
                string path = AssetDatabase.GUIDToAssetPath(r.guid);
                if (path.IsNotEmpty())
                {
                    return path;
                }
            }
            if (r.path.IsNotEmpty())
            {
                return "Assets/"+r.path;
            } else
            {
                return string.Empty;
            }
        }
    }
}


