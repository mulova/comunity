using System.Collections.Generic;
using System.Collections.Generic.Ex;
using mulova.commons;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using System.Ex;
using System.Text.Ex;
using UnityEngine.Ex;

namespace mulova.build
{
    public static class AssetBundleMenu
    {

        /// <summary>
        /// Extracts the nested prefab dependencies and non prefab assets
        /// </summary>
        /// <returns>The nested dependencies.</returns>
        public static HashSet<string> ExtractNestedDependencies(string path)
        {
            var dep = new HashSet<string>(AssetDatabase.GetDependencies(path, true));

            var nested = GetOutermostNestedPrefabs(path).ToArray();
            dep.RemoveAll(AssetDatabase.GetDependencies(nested, true));
            dep.AddAll(nested);
            return dep;
        }

        public static List<string> GetOutermostNestedPrefabs(string path)
        {
            var o = PrefabUtility.LoadPrefabContents(path);
            var nested = new List<string>();
            o.transform.BreadthFirstTraversal(t =>
            {
                var obj = t.gameObject;
                if (obj != o && PrefabUtility.IsAnyPrefabInstanceRoot(obj))
                {
                    var origin = PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj);
                    var originPath = AssetDatabase.GetAssetPath(origin);
                    if (!originPath.IsEmpty())
                    {
                        nested.Add(originPath);
                    }
                    return false;
                }
                return true;
            }
            );
            return nested;
        }

        [MenuItem("Assets/AssetBundles/List Outermost Nested Prefabs")]
        public static void ListRootNestedPrefabs()
        {
            Selection.assetGUIDs.ForEach(guid =>
            {
                var rootPath = AssetDatabase.GUIDToAssetPath(guid);
                EditorTraversal.ForEachAssetPath(rootPath, FileType.Prefab, p =>
                {
                    var list = GetOutermostNestedPrefabs(p);
                    Debug.Log(list.Join(","));
                });
            });
        }

        [MenuItem("Assets/AssetBundles/Assign AssetBundle Labels From Selection")]
        public static void AssignAssetBundleNames()
        {
            // Clear All AssetBundles
            var dep = new AssetBundler();
            var assets = EditorAssetUtil.ListAssets(Selection.objects);
            var paths = new List<string>();
            foreach (var a in assets)
            {
                paths.Add(AssetDatabase.GetAssetPath(a));
            }
            //dep.SetPathFilter(true, @"\.jpg$", @"\.png$", @"\.tga$");
            dep.SetCommonAssetAsBundles(paths);
        }

        [MenuItem("Assets/AssetBundles/Extract Common AssetBundle Labels For Current Bundles", false, 1)]
        public static void ExtractComonForCurrentBundles()
        {
            // Clear All AssetBundles
            var dep = new AssetBundler();
            foreach (var n in AssetDatabase.GetAllAssetBundleNames())
            {
                var p = AssetDatabase.GetAssetPathsFromAssetBundle(n);
                dep.SetCommonAssetAsBundles(p);
            }
        }
    }
}
