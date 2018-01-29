using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using commons;
using System.Text;
using comunity;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;

namespace comunity
{
    public static class EditorTraversal
    {
        public static readonly Loggerx log = LogManager.GetLogger(typeof(EditorTraversal));
        private static bool sceneProcessing;

        private static Regex ignoreRegex;
        public static string ignorePattern = ".meta$|.fbx$|.FBX$|/Editor/|Assets/Plugins/";

        private static Regex ignorePath
        {
            get
            {
                if (ignoreRegex == null)
                {
                    ignoreRegex = new Regex(ignorePattern);
                }
                return ignoreRegex;
            }
        }

        public static void AddIgnorePattern(string regexPattern)
        {
            ignorePattern = string.Format("{0}|{1}", ignorePattern, regexPattern);
            ignoreRegex = null;
        }

        /// <summary>
        /// Fors the each scene.
        /// </summary>
        /// <returns>The each scene.</returns>
        /// <param name="func">[param] scene roots,  [return] error string</param>
        public static string ForEachScene(Func<IEnumerable<Transform>, string> func)
        {
            //      string current = EditorSceneBridge.currentScene;
            List<string> errors = new List<string>();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i=0; i<scenes.Length; ++i)
            {
                errors.AddRange(ProcessScene(scenes[i], func));
            }
            return StringUtil.Join("\n", errors);
        }

        public static List<string> ProcessScene(EditorBuildSettingsScene s, Func<IEnumerable<Transform>, string> func)
        {
            List<string> errors = new List<string>(); 
            sceneProcessing = true;

            try
            {
                EditorSceneBridge.OpenScene(s.path);
                log.Debug("Processing Scene '{0}'", s.path);
                string error = func(EditorSceneManager.GetActiveScene().GetRootGameObjects().Convert(o=>o.transform));
                if (error.IsNotEmpty())
                {
                    errors.Add(error);
                }
                EditorUtil.SaveScene();
            } catch (Exception ex)
            {
                log.Error(ex);
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Error", "See editor log for details", "OK");
                throw ex;
            }

            sceneProcessing = false;
            return errors;
        }

        /// <summary>
        /// Fors the each prefab.
        /// </summary>
        /// <returns>The each prefab.</returns>
        /// <param name="func">error_string Func(assetPath, asset)</param>
        public static string ForEachPrefab(Func<string, GameObject, string> func)
        {
            return ForEachAsset<GameObject>(func, FileType.Prefab);
        }

        public static string ForEachAssetPath(Func<string, string> func, FileType fileType)
        {
            try
            {
                StringBuilder err = new StringBuilder();
                string[] paths = EditorAssetUtil.ListAssetPaths("Assets", fileType, true);
                log.Info("Prebuild {0:D0} assets", paths.Length);
                for (int i = 0; i < paths.Length; ++i)
                {
                    string p = "Assets/"+paths[i];
                    if (ignorePath.IsMatch(p))
                    {
                        continue;
                    }
                    if (EditorUtil.DisplayProgressBar("Assets", p, i / (float)paths.Length))
                    {
                        return "canceled by user";
                    }
                    try {
                        string error = func(p);
                        if (error.IsNotEmpty())
                        {
                            err.Append(error).Append("\n");
                        }
                    } catch (Exception ex)
                    {
                        log.Error(ex);
                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("Error", "See editor log for details", "OK");
                        throw ex;
                    }
                }
                EditorUtility.ClearProgressBar();
                if (err.Length > 0)
                {
                    return err.ToString();
                } else
                {
                    AssetDatabase.SaveAssets();
                    return null;
                }
            } catch (Exception ex)
            {
                return ex.Message+"\n"+ex.StackTrace;
            }
        }

        public static void PreprocessAsset(string[] allPaths, string progressTitle, Action<Object, string> preprocess)
        {
            for (int i = 0; i < allPaths.Length; ++i)
            {
                string path = allPaths[i];
                if (ignorePath.IsMatch(path))
                {
                    continue;
                }
                if (EditorUtil.DisplayProgressBar(progressTitle, path, i / (float)allPaths.Length))
                {
                    EditorUtility.ClearProgressBar();
                    throw new Exception("canceled by user");
                }
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                preprocess(asset, path);
            }
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Fors the each resource.
        /// </summary>
        /// <returns>The each resource.</returns>
        /// <param name="func">Func returns error message if occurs.</param>
        public static string ForEachAsset<T>(Func<string, T, string> func, FileType fileType)  where T:Object
        {
            return ForEachAssetPath(p =>
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(p);
                return func(p, asset);
            }, fileType);
        }

        public static void SetDirty(UnityEngine.Object o)
        {
            CompatibilityEditor.SetDirty(o);
            if (sceneProcessing)
            {
                EditorSceneBridge.MarkSceneDirty();
            }
        }
    }
}

