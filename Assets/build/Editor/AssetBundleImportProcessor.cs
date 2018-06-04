//#define ASSET_BUNDLE_IMPORTER
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using comunity;
using commons;

public class AssetBundleImportProcessor
#if ASSET_BUNDLE_IMPORTER
: AssetPostprocessor
#endif
{
    [MenuItem("Tools/unilova/Asset/Reassign AssetBundles")]
    public static void ReassignAssetBundleNames()
    {
        // Clear All AssetBundles
        List<string> assets = new List<string>();
        foreach (UnityObjId dir in AssetBundlePath.inst.dirs)
        {
            string[] paths = EditorAssetUtil.ListAssetPaths(dir.path, FileType.All);
            foreach (var p in paths)
            {
                if (AssetBundleDep.IsAssetBundle(p))
                {
                    assets.Add(p);
                }
            }
        }
        AssetBundleDep.SetCommonAssetAsBundles(assets);
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // changed AssetBundle names for moved assets
        foreach (var p in movedFromAssetPaths)
        {
            AssetImporter im = AssetImporter.GetAtPath(p);
            if (im != null)
            {
                var oldName = im.assetBundleName;
                if (oldName.IsNotEmpty() && p != "Assets/"+oldName)
                {
                    AssetBundleDep.SetAssetBundleName(p);
                    Debug.LogFormat("Change AssetBundle name {0} => {1}", oldName, im.assetBundleName);
                }
            }
        }

        AssetBundleDep.SetCommonAssetAsBundles(importedAssets);
    }

    [MenuItem("Tools/unilova/Asset/Clear AssetBundles")]
    public static void ClearAllAssetBundleNames()
    {
        foreach (var n in AssetDatabase.GetAllAssetBundleNames())
        {
            AssetDatabase.RemoveAssetBundleName(n, true);
        }
        AssetBundleDep.Clear();
    }
}

