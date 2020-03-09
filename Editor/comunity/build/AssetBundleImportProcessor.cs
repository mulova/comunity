//#define ASSET_BUNDLE_IMPORTER
using System.Collections.Generic;
using System.Text.Ex;
using mulova.commons;
using mulova.comunity;
using mulova.unicore;
using UnityEditor;
using UnityEngine;

namespace mulova.build
{
    public class AssetBundleImportProcessor
	#if ASSET_BUNDLE_IMPORTER
	: AssetPostprocessor
	#endif
	{
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			// changed AssetBundle names for moved assets
			foreach (var p in movedFromAssetPaths)
			{
				AssetImporter im = AssetImporter.GetAtPath(p);
				if (im != null)
				{
					var oldName = im.assetBundleName;
					if (!oldName.IsEmpty() && p != "Assets/"+oldName)
					{
						//AssetBundler.inst.SetAssetBundleName(p);
						Debug.LogFormat("Change AssetBundle name {0} => {1}", oldName, im.assetBundleName);
					}
				}
			}
			
			//AssetBundler.inst.SetCommonAssetAsBundles(importedAssets);
		}
		
		[MenuItem("Tools/unilova/Asset/Clear AssetBundles")]
		public static void ClearAllAssetBundleNames()
		{
			foreach (var n in AssetDatabase.GetAllAssetBundleNames())
			{
				AssetDatabase.RemoveAssetBundleName(n, true);
			}
			//AssetBundler.inst.Clear();
		}
	}
}

