using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

using UnityEditor.SceneManagement;

namespace comunity
{
    [InitializeOnLoad]
    public class DiMenu : UnityEditor.AssetModificationProcessor
    {
        private static string scene;

        public static string FILE_PATH
        {
            get
            {
                return string.Format("Assets/Resources/{0}.bytes", DiService.PREF_PATH);     
            }
        }

        public static MonoBehaviour[] allScripts
        {
            get
            {
                List<MonoBehaviour> all = new List<MonoBehaviour>();
                foreach (var m in Resources.FindObjectsOfTypeAll<MonoBehaviour>())
                {
                    var assetPath = AssetDatabase.GetAssetPath(m.gameObject);
                    if (assetPath.IsEmpty())
                    {
                        all.Add(m);
                    }
                }
                return all.ToArray();
            }
        }

        static DiMenu()
        {
            EditorApplication.hierarchyWindowChanged += OnHierarchyChange;
            #if UNITY_2017
            EditorApplication.playModeStateChanged += OnPlayModeChange;
            #else
            EditorApplication.playmodeStateChanged += OnPlaymodeChange;
            #endif
        }

        private static void OnPlayModeChange(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredPlayMode)
            {
                InjectDependency();
            }
        }

        private static void OnPlaymodeChange()
        {
            if (Application.isPlaying)
            {
                InjectDependency();
            }
        }

        private static void OnHierarchyChange()
        {
            if (Application.isPlaying)
            {
                return;
            }
            if (scene != EditorSceneManager.GetActiveScene().name)
            {
                scene = EditorSceneManager.GetActiveScene().name;
                InjectDependency();
            }
        }

        [MenuItem("Tools/Inject", false)]
        public static void InjectDependency()
        {
            if (!Application.isEditor || !DiService.enableDi)
            {
                return;
            }
             DiService di = UnityEngine.Object.FindObjectOfType<DiService>();
            if (di == null)
            {
                di = DiService.Create();
            }
            var changeList = di.ResolveScene(allScripts);
            foreach (var m in changeList)
            {
                CompatibilityEditor.SetDirty(m);
            }
            if (changeList.IsNotEmpty())
            {
                CompatibilityEditor.SetDirty(di);
            }
        }

        [MenuItem("Tools/Inject", true)]
        public static bool IsInjectDependency()
        {
            DiService.LoadPreference();
            return DiService.enableDi;
        }

        [PreferenceItem("DI")]
        public static void PreferenceMenu()
        {
            bool di = DiService.enableDi;
            if (EditorGUIUtil.Toggle("enable DI", ref di))
            {
                DiService.enableDi = di;
                Save();
            }
        }

        public static void Save()
        {
            reader.Put(DiService.ENABLED, DiService.enableDi);
            reader.Save(FILE_PATH);
            AssetDatabase.ImportAsset(FILE_PATH, ImportAssetOptions.ForceSynchronousImport|ImportAssetOptions.ForceUpdate);
            DiService.LoadPreference();
        }

        public static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                if (path.Contains(".unity"))
                {
                    InjectDependency();
                    break;
                }
            }
            return paths;
        }

        private static PropertiesReader _reader;

        private static PropertiesReader reader
        {
            get
            {
                if (_reader == null)
                {
                    _reader = new PropertiesReader();
                    _reader.LoadFile(FILE_PATH, true);
                }
                return _reader;
            }
        }
    }
}
