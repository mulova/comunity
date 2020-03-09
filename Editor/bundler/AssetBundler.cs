using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Text.Ex;
using System.Text.RegularExpressions;
using mulova.commons;
using mulova.unicore;
using UnityEditor;
using UnityEngine;
using System.Ex;

namespace mulova.build
{
    public class AssetBundler
    {
        public delegate string Labeler(string path);
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

        private HashSet<string> currentDeps = new HashSet<string>();

        public void SetPathFilter(bool include, params string[] regexp)
        {
            this.includePath = include;
            if (regexp != null) {
                regexp.ForEach(ex => pathFilter.Add(new Regex(ex)));
            }
        }

        public bool IsAsset(string path)
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
        /// <param name="rootPaths">Assets.</param>
        public void SetCommonAssetAsBundles(IList<string> rootPaths)
        {
            List<string> assetPaths = new List<string>();
            // filter asset bundle
            foreach (var p in rootPaths)
            {
                if (IsAsset(p))
                {
                    assetPaths.Add(p);
                }
            }

            try
            {
                for (int i = 0; i < rootPaths.Count; ++i)
                {
                    EditorUtility.DisplayProgressBar("Collecting Dependencies", rootPaths.Get(i), i / (float)rootPaths.Count);
                    //var deps = AssetBundleMenu.ExtractNestedDependencies(rootPaths[i]);
                    var deps = AssetDatabase.GetDependencies(rootPaths[i]);
                    foreach (var d in deps)
                    {
                        if (!currentDeps.Add(d))
                        {
                            SetAssetBundleName(d);
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// Detach 'Assets/' from path and make it as asset bundle name 
        /// </summary>
        /// <param name="path">Path.</param>
        public void SetAssetBundleName(string path)
        {
            AssetImporter im = AssetImporter.GetAtPath(path);
            if (im.assetBundleName.IsEmpty())
            {
                var bundleName = generateLabel(path);
                var variantName = generateVariant(path);
                im.SetAssetBundleNameAndVariant(bundleName, variantName);
                im.SaveAndReimport();
            }
            currentDeps.Add(path);
            Debug.LogFormat("Assigning AssetBundle '{0}'", path);
        }

        public void Clear()
        {
            currentDeps.Clear();
        }
    }
}
