using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using commons;
using System;
using comunity;
using UnityEngine.UI;
using System.IO;

namespace build
{
    public static class AssetBundleDep
    {
        private static HashSet<string> excludes = new HashSet<string>();
        private static HashSet<string> _curDeps;
        private static HashSet<string> currentDeps
        {
            get
            {
                InitDeps();
                return _curDeps;
            }
        }
        
        private static void InitDeps()
        {
            if (_curDeps != null)
            {
                return;
            }
            _curDeps = new HashSet<string>();
            var names = AssetDatabase.GetAllAssetBundleNames();
            try {
                if (names != null)
                {
                    for (int i=0; i<names.Length; ++i)
                    {
                        EditorUtility.DisplayProgressBar("Collecting Dependencies", names[i], i / (float)names.Length);
                        var p = AssetDatabase.GetAssetPathsFromAssetBundle(names[i]);
                        var dep = AssetDatabase.GetDependencies(p, true);
                        _curDeps.AddAll(dep);
                    }
                }
            } finally {
                EditorUtility.ClearProgressBar();
            }
        }
        
        public static bool IsAssetBundle(string path)
        {
            var type = FileTypeEx.GetFileType(path);
            switch (type)
            {
                case FileType.Material:
                case FileType.Text:
                case FileType.Prefab:
                case FileType.Anim:
                case FileType.Model:
                case FileType.Image:
                case FileType.Audio:
                case FileType.Video:
                case FileType.Scene:
                case FileType.ScriptableObject:
                    return AssetBundlePath.inst.IsCdnPath(path);
                case FileType.Asset:
                case FileType.Zip:
                case FileType.Meta:
                case FileType.Script:
                    return false;
                default:
                    return false;
            }
        }
        
        
        /// <summary>
        /// Make the assets and it's dependencies which is the common asset of existing AssetBundles as AssetBundle
        /// </summary>
        /// <param name="assets">Assets.</param>
        public static void SetCommonAssetAsBundles(IList<string> assets)
        {
            InitDeps();
            List<string> assetPaths = new List<string>();
            // filter asset bundle
            foreach (var p in assets)
            {
                if (IsAssetBundle(p))
                {
                    assetPaths.Add(p);
                }
            }
            var deps = AssetDatabase.GetDependencies(assetPaths.ToArray(), false);
            SetDependenciesRecursive(deps);
            
            foreach (var p in assetPaths)
            {
                SetAssetBundleName(p);
            }
        }
        
        private static void SetDependenciesRecursive(string[] deps)
        {
            // find duplicate dependencies add asset bundle name for them
            foreach (string d in deps)
            {
                if (excludes.Contains(d))
                {
                    continue;
                }
                if (currentDeps.Contains(d))
                {
                    AssetImporter im = AssetImporter.GetAtPath(d);
                    if (im.assetBundleName.IsEmpty())
                    {
                        SetAssetBundleName(d);
                        if (!AssetBundlePath.inst.IsCdnPath(d))
                        {
                            Debug.LogWarningFormat("Common asset '{0}' is not in CDN ", d);
                        }
                    }
                } else
                {
                    var deps2 = AssetDatabase.GetDependencies(d, false);
                    if (deps2.IsNotEmpty())
                    {
                        SetDependenciesRecursive(deps2);
                    }
                }
                currentDeps.Add(d);
            }
        }
        
        /// <summary>
        /// Detach 'Assets/' from path and make it as asset bundle name 
        /// </summary>
        /// <param name="path">Path.</param>
        public static void SetAssetBundleName(string path)
        {
            AssetImporter im = AssetImporter.GetAtPath(path);
            string bundleName = EditorAssetUtil.GetAssetRelativePath(path).ToLower();
            im.SetAssetBundleNameAndVariant(bundleName, "");
            currentDeps.Add(path);
            Debug.LogFormat("Assigning AssetBundle '{0}'", path);
        }
        
        public static void Clear()
        {
            _curDeps = null;
        }
        
        public static List<string> FindDuplicateAssetBundles()
        {
            var names = AssetDatabase.GetAllAssetBundleNames();
            var assets = new HashSet<string>();
            var duplicates = new HashSet<string>();
            var subAssets = new HashSet<string>();
            EditorGUIUtil.DisplayProgressBar(names, "Find", true, n=> {
                var paths = AssetDatabase.GetAssetPathsFromAssetBundle(n);
                var newAssets = new HashSet<string>();
                foreach (var p in paths)
                {
                    var deps = AssetDatabase.GetDependencies(p, false);
                    foreach (var d in deps)
                    {
                        if (!string.IsNullOrEmpty(AssetDatabase.GetImplicitAssetBundleName(d))
                            || d.EndsWithIgnoreCase(".cs")
                            || d.EndsWithIgnoreCase(".dll"))
                        {
                            continue;
                        }
                        if (assets.Contains(d))
                        {
                            duplicates.Add(d);
                        } else
                        {
                            newAssets.Add(d);
                        }
                        var subDeps = AssetDatabase.GetDependencies(d, true);
                        subAssets.AddAll(subDeps);
                    }
                }
                assets.AddAll(newAssets);
            });
            return duplicates.ToList(d=>d);
        }
    }
}


//public class AssetBundleModificationProcessor : AssetModificationProcessor
//{
//    public void OnWillDeleteAsset(string,RemoveAssetOptions)
//    {
//    }
//}