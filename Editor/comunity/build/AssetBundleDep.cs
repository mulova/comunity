using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Text.Ex;
using System.Text.RegularExpressions;
using mulova.commons;
using mulova.comunity;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace mulova.build
{
    public class AssetBundleDep
    {
        public delegate string Labeler(string path);
        public bool collectCurrentDeps = true;
        public Labeler generateLabel = path =>
        {
            var l = EditorAssetUtil.GetAssetRelativePath(path).ToLower() + "_ab";
            return l.Replace('.', '_');
        };
        public Labeler generateVariant = path=> {
            return AssetDatabase.GetImplicitAssetBundleVariantName(path);
        };
        private bool includePath;
        private HashSet<Regex> pathFilter = new HashSet<Regex>();

        private HashSet<string> _curDeps;
        private HashSet<string> currentDeps
        {
            get
            {
                CollectDeps();
                return _curDeps;
            }
        }

        private static AssetBundleDep _inst;
        public static AssetBundleDep inst
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new AssetBundleDep();
                }
                return _inst;
            }
        }

        public AssetBundleDep(bool collectCurrentDeps = true)
        {
            this.collectCurrentDeps = collectCurrentDeps;
        }

        public void SetPathFilter(bool include, params string[] regexp)
        {
            this.includePath = include;
            if (regexp != null) {
                regexp.ForEach(ex => pathFilter.Add(new Regex(ex)));
            }
        }

        public void CollectDeps()
        {
            if (_curDeps != null)
            {
                return;
            }
            _curDeps = new HashSet<string>();
            if (collectCurrentDeps)
            {
                var names = AssetDatabase.GetAllAssetBundleNames();
                try
                {
                    if (names != null)
                    {
                        for (int i = 0; i < names.Length; ++i)
                        {
                            EditorUtility.DisplayProgressBar("Collecting Dependencies", names[i], i / (float)names.Length);
                            var p = AssetDatabase.GetAssetPathsFromAssetBundle(names[i]);
                            var dep = AssetDatabase.GetDependencies(p, true);
                            _curDeps.AddAll(dep);
                        }
                    }
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }
        }

        public bool IsAssetBundle(string path)
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
                case FileType.Asset:
                    return true;
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
        public void SetCommonAssetAsBundles(IList<string> assets)
        {
            CollectDeps();
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

        private void SetDependenciesRecursive(string[] deps)
        {
            // find duplicate dependencies add asset bundle name for them
            foreach (string d in deps)
            {
                bool include = false;
                if (!pathFilter.IsEmpty())
                {
                    foreach (var r in pathFilter)
                    {
                        bool match = r.IsMatch(d);
                        if (includePath && match)
                        {
                            include = true;
                            break;
                        }
                        if (!includePath && !match)
                        {
                            include = false;
                            break;
                        }
                    }
                }
                if (currentDeps.Contains(d) && include)
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
                }
                else
                {
                    var deps2 = AssetDatabase.GetDependencies(d, false);
                    if (!deps2.IsEmpty())
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
        public void SetAssetBundleName(string path)
        {
            var bundleName = generateLabel(path);
            var variantName = generateVariant(path);
            AssetImporter im = AssetImporter.GetAtPath(path);
            im.SetAssetBundleNameAndVariant(bundleName, variantName);
            im.SaveAndReimport();
            currentDeps.Add(path);
            Debug.LogFormat("Assigning AssetBundle '{0}'", path);
        }

        public void Clear()
        {
            _curDeps = null;
        }

        public List<AssetBundleDup> FindDuplicateAssetBundles()
        {
            var names = AssetDatabase.GetAllAssetBundleNames();
            var duplicates = new Dictionary<string, AssetBundleDup>();
            EditorGUIUtil.DisplayProgressBar(names, "Find", true, n =>
            {
                var paths = AssetDatabase.GetAssetPathsFromAssetBundle(n);
                var newAssets = new Dictionary<string, AssetBundleDup>();
                foreach (var p in paths)
                {
                    var depPaths = AssetDatabase.GetDependencies(p, false);
                    foreach (var d in depPaths)
                    {
                        if (!string.IsNullOrEmpty(AssetDatabase.GetImplicitAssetBundleName(d))
                            || d.EndsWithIgnoreCase(".cs")
                            || d.EndsWithIgnoreCase(".dll"))
                        {
                            continue;
                        }
                        var item = duplicates.Get(d);
                        if (item == null)
                        {
                            var item0 = newAssets.Get(d);
                            if (item0 == null)
                            {
                                item0 = new AssetBundleDup(AssetDatabase.LoadAssetAtPath<Object>(d));
                                newAssets.Add(d, item0);
                            }
                            item0.AddRef(AssetDatabase.LoadAssetAtPath<Object>(p));
                        }
                        else
                        {
                            item.duplicate = true;
                            item.AddRef(AssetDatabase.LoadAssetAtPath<Object>(p));
                        }
                    }
                }
                duplicates.AddAll(newAssets);
            });
            return duplicates.Values.Filter(d => d.duplicate);
        }
    }
}


//public class AssetBundleModificationProcessor : AssetModificationProcessor
//{
//    public void OnWillDeleteAsset(string,RemoveAssetOptions)
//    {
//    }
//}