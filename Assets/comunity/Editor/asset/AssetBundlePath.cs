using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System.Text;
using System;
using commons;

namespace comunity
{
    [System.Serializable]
    public class AssetBundlePath
    {
        public const string PATH = "asset_builder/ab_path";
        [SerializeField]
        private List<UnityObjId> _dirs = new List<UnityObjId>();
        public List<UnityObjId> dirs
        {
            get
            {
                return _dirs;
            }
        }

        private List<UnityObjId> _rawDirs = new List<UnityObjId>();
        public List<UnityObjId> rawDirs
        {
            get
            {
                return _rawDirs;
            }
        }

        private bool auto = false;

        private static AssetBundlePath _inst;
        public static AssetBundlePath inst
        {
            get
            {
                if (_inst == null)
                {
                    _inst = Load(PATH);
                }
                return _inst;
            }
        }

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
            return IsCdnPath(dirs, path)||IsCdnPath(rawDirs, path);
        }
        
        public bool IsRawCdnPath(string path)
        {
            if (path.IsEmpty())
            {
                return false;
            }
            return IsCdnPath(rawDirs, path);
        }
        
        public bool IsRawCdnAsset(Object o)
        {
            if (o == null)
            {
                return false;
            }
            return IsRawCdnPath(AssetDatabase.GetAssetPath(o));
        }
        
        private bool IsCdnPath(List<UnityObjId> dirs, string path)
        {
            foreach (UnityObjId dir in dirs)
            {
                if (path.StartsWith(dir.path))
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


        public void DrawInspectorGUI()
        {
            DrawAssetDir("AssetDir", dirs, false);
            DrawAssetDir("Raw AssetDir", rawDirs, true);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save", GUILayout.Height(50)))
            {
                Save(PATH);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddObjects(List<UnityObjId> list, params Object[] objs)
        {
            foreach (Object o in objs)
            {
                UnityObjId obj = new UnityObjId(o);
                if (o == null)
                {
                    continue;
                }
                if (EditorAssetUtil.IsFolder(o)&&!list.Contains(obj))
                {
                    list.Add(obj);
                }
            }
        }

        private void DrawAssetDir(string title, List<UnityObjId> list, bool raw)
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
                var drawer = new UnityObjIdReorderList(null, list);
                if (drawer.Draw())
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
            if (_dirs.IsEmpty()||_rawDirs.IsEmpty())
            {
                // TODOM find
                Debug.LogError("Why remove files?");
                return;
            }
#pragma warning restore 0162
            Save(PATH);
        }

        public static AssetBundlePath Load(string path)
        {
            try{
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    var p = JsonUtility.FromJson<AssetBundlePath>(json);
                    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
                    {
                        p._dirs.AddRange(p._rawDirs);
                        p._rawDirs.Clear();
                    }
                    return p;
                }
            } catch (Exception ex) {
                Debug.LogError(ex.ToString());
            }
            return new AssetBundlePath();

        }

        public void Save(string path)
        {
            StringBuilder err = new StringBuilder();
            for (int i=0; i<_dirs.Count; ++i)
            {
                string d1 = _dirs[i].path+"/";
                for (int j=i; j<_rawDirs.Count; ++j)
                {
                    string d2 = _rawDirs[j].path+"/";
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
                    var json = JsonUtility.ToJson(this, true);
                    File.WriteAllText(PATH, json);
                } else
                {
                    Debug.LogWarning("Won't save paths");
                }
            }
        }
    }
}
