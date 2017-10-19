using System;
using UnityEditor;
using UnityEngine;


using System.IO;
using UnityEditor.Build;

namespace core
{
    [InitializeOnLoad]
    public class LibManager
    #if UNITY_2017
        : IActiveBuildTargetChanged
    #endif
    {
        static LibManager()
        {
            new LibManager();
        }

        public LibManager()
        {
            #if !UNITY_2017
            EditorUserBuildSettings.activeBuildTargetChanged += CopyLibs;
            #endif
            CopyLibs();
        }

        #if UNITY_2017
//        [ActiveBuildTargetChanged]
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            CopyLibs();
        }
        #endif

        #region IOrderedCallback implementation

        public int callbackOrder
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        [MenuItem("Tools/unilova/Copy Platform Libs")]
        public static void CopyLibs()
        {
            // only works on OSX for now
            if (!SystemInfo.operatingSystem.Contains("Mac"))
            {
                return;
            }
            if (!Directory.Exists("platform_libs"))
            {
                Debug.Log("no platform_libs folder");
                return;
            }

            string target = EditorUserBuildSettings.activeBuildTarget.ToRuntimePlatform().GetPlatformName();
            if (File.Exists("platform_libs/copy_dll.sh"))
            {
                if (!Directory.Exists("platform_libs/"+target))
                {
                    target = BuildConfig.TARGET_ANDROID;
                }
                var output = EditorUtil.ExecuteCommand("platform_libs/copy_dll.sh", target);

                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate); // NullRefereceException is thrown. why?
                if (!output.IsError())
                {
                    //                  AssetDatabase.ImportAsset("Assets/Plugins/lib", ImportAssetOptions.ImportRecursive);
                    //                  AssetDatabase.ImportAsset("Assets/Plugins/Editor/lib", ImportAssetOptions.ImportRecursive);
                    Debug.Log("Copy Libs: "+output.GetResult());
                } else
                {
                    Debug.LogError("Copy Libs: "+output.GetResult());
                }
            }
        }
    }
}

