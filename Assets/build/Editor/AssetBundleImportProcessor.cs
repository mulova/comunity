//#define ASSET_BUNDLE_IMPORTER
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using comunity;
using commons;

namespace build
{
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
					if (AssetBundleDep.inst.IsAssetBundle(p))
					{
						assets.Add(p);
					}
				}
			}
            AssetBundleDep dep = new AssetBundleDep();
            dep.SetPathFilter(true, @"\.jpg$", @"\.png$", @"\.tga$");
            dep.SetCommonAssetAsBundles(assets);
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
						AssetBundleDep.inst.SetAssetBundleName(p);
						Debug.LogFormat("Change AssetBundle name {0} => {1}", oldName, im.assetBundleName);
					}
				}
			}
			
			AssetBundleDep.inst.SetCommonAssetAsBundles(importedAssets);
		}
		
		[MenuItem("Tools/unilova/Asset/Clear AssetBundles")]
		public static void ClearAllAssetBundleNames()
		{
			foreach (var n in AssetDatabase.GetAllAssetBundleNames())
			{
				AssetDatabase.RemoveAssetBundleName(n, true);
			}
			AssetBundleDep.inst.Clear();
		}
	}
}

