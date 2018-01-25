using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System.Text;
using System;

namespace core
{
    public class AssetBundlePath
    {
        public const string ASSET_DIR_LIST = "asset_builder/asset_dir.txt";
        public const string RAW_ASSET_DIR_LIST = "asset_builder/asset_dir_raw.txt";
        private List<Object> rawDirRefs = new List<Object>();
        private List<Object> dirRefs = new List<Object>();
        public static AssetBundlePath inst = new AssetBundlePath();

        [PreferenceItem("Asset Builder")]
        public static void PreferenceMenu()
        {
            inst.DrawInspectorGUI();
        }

        public bool IsCdnAsset(Object o)
        {
            if (o == null)
            {
                return false;
            }
            string path = AssetDatabase.GetAssetPath(o);
            return IsCdnPath(path);
        }

        public bool IsCdnPath(string path)
        {
            if (path.IsEmpty())
            {
                return false;
            }
            return IsCdnPath(cdnDirs, path)||IsCdnPath(rawCdnDirs, path);
        }
        
        public bool IsRawCdnPath(string path)
        {
            if (path.IsEmpty())
            {
                return false;
            }
            return IsCdnPath(rawCdnDirs, path);
        }
        
        public bool IsRawCdnAsset(Object o)
        {
            if (o == null)
            {
                return false;
            }
            return IsRawCdnPath(AssetDatabase.GetAssetPath(o));
        }
        
        private bool IsCdnPath(List<string> dirs, string path)
        {
            foreach (string dir in dirs)
            {
                if (path.StartsWith(dir))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsModified(string filePath, ref DateTime time)
        {
            DateTime modTime = File.GetLastWriteTime(filePath);
            if (modTime != time)
            {
                time = modTime;
                return true;
            } else
            {
                return false;
            }
        }


        private DateTime assetDirTime;
        private DateTime rawAssetDirTime;
        private List<string> _cdnDirs;
        private List<string> _rawCdnDirs;
        private List<Object> _cdnRefs;
        private List<Object> _rawCdnRefs;
        
        private void UpdatePaths()
        {
            if (_cdnDirs == null||IsModified(ASSET_DIR_LIST, ref assetDirTime))
            {
                _cdnDirs = LoadCdnPaths(ASSET_DIR_LIST);
            }
            if (_rawCdnDirs == null||IsModified(RAW_ASSET_DIR_LIST, ref rawAssetDirTime))
            {
                _rawCdnDirs = LoadCdnPaths(RAW_ASSET_DIR_LIST);
            }
            
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
            {
                _cdnDirs.AddRange(_rawCdnDirs);
                _rawCdnDirs.Clear();
            }
            _cdnRefs = _cdnDirs.ConvertAll(d => AssetDatabase.LoadAssetAtPath<Object>(d));
            _rawCdnRefs = _rawCdnDirs.ConvertAll(d => AssetDatabase.LoadAssetAtPath<Object>(d));
        }
        
        public List<string> cdnDirs
        {
            get
            {
                UpdatePaths();
                return _cdnDirs;
            }
        }
        
        public List<string> rawCdnDirs
        {
            get
            {
                UpdatePaths();
                return _rawCdnDirs;
            }
        }
        
        public List<Object> cdnRefs
        {
            get
            {
                UpdatePaths();
                return _cdnRefs;
            }
        }
        
        public List<Object> rawCdnRefs
        {
            get
            {
                UpdatePaths();
                return _rawCdnRefs;
            }
        }
        
        private List<string> LoadCdnPaths(string filePath)
        {
            List<string> paths = new List<string>();
            if (File.Exists(filePath))
            {
                string[] guidList = File.ReadAllLines(filePath);
                if (guidList != null)
                {
                    foreach (string guid in guidList)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        if (assetPath.IsNotEmpty())
                        {
                            paths.Add(assetPath);
                        }
                    }
                }
            }
            return paths;
        }

        public AssetBundlePath()
        {
            dirRefs = cdnRefs;
            rawDirRefs = rawCdnRefs;
        }
       
        public void DrawInspectorGUI()
        {
            DrawAssetDir("AssetDir", dirRefs, false);
            DrawAssetDir("Raw AssetDir", rawDirRefs, true);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                Save();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddObjects(List<Object> list, params Object[] objs)
        {
            foreach (Object o in objs)
            {
                if (o == null)
                {
                    continue;
                }
                if (EditorAssetUtil.IsFolder(o)&&!list.Contains(o))
                {
                    list.Add(o);
                }
            }
        }

        private void DrawAssetDir(string title, List<Object> list, bool raw)
        {
            bool foldout = EditorUI.DrawHeader(title);
            if (foldout)
            {
                EditorUI.BeginContents();
                Object[] drag = EditorGUIUtil.DnD(EditorGUILayout.GetControlRect());
                if (drag != null)
                {
                    AddObjects(list, drag);
                    SaveAuto();
                }
                if (EditorGUIUtil.ObjectFieldList(list))
                {
                    SaveAuto();
                }
                EditorUI.EndContents();
            }
        }

        private void SaveAuto()
        {
            if (true)
            {
                return;
            }
#pragma warning disable 0162
            if (dirRefs.IsEmpty()||rawDirRefs.IsEmpty())
            {
                // TODOM find
                Debug.LogError("Why remove files?");
                return;
            }
#pragma warning restore 0162
            Save();
        }

        public void Save()
        {
            StringBuilder err = new StringBuilder();
            for (int i=0; i<dirRefs.Count; ++i)
            {
                string d1 = AssetDatabase.GetAssetPath(dirRefs[i])+"/";
                for (int j=i; j<rawDirRefs.Count; ++j)
                {
                    string d2 = AssetDatabase.GetAssetPath(rawDirRefs[j])+"/";
                    if (d1.StartsWith(d2)||d2.StartsWith(d1))
                    {
                        err.AppendFormat("the path '{0}' overlaps with '{1}'", d1, d2).AppendLine();
                    }
                }
            }
            if (err.Length > 0)
            {
                EditorUtility.DisplayDialog("Path overlap", err.ToString(), "OK");
            } else
            {
                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
                {
                    EditorAssetUtil.SaveReferences(AssetBundlePath.ASSET_DIR_LIST, dirRefs);
                    EditorAssetUtil.SaveReferences(AssetBundlePath.RAW_ASSET_DIR_LIST, rawDirRefs);
                } else
                {
                    Debug.LogWarning("Won't save paths");
                }
            }
        }
    }
}
