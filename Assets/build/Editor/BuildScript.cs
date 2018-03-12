using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using Object = UnityEngine.Object;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using commons;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using comunity;
using UnityEditor.Build;

namespace build
{
    public class BuildScript : IPreprocessBuild, IPostprocessBuild
    {
        public const string VERIFY_ONLY = "verify_only";
        public const string EXCLUDE_CDN = "exclude_cdn";
        
        public static readonly Loggerx log = LogManager.GetLogger(typeof(BuildScript));

        public void OnPreprocessBuild(UnityEditor.BuildTarget target, string path)
        {
            throw new NotImplementedException();
        }

        public int callbackOrder
        {
            get
            {
                return 1;
            }
        }

        public void OnPostprocessBuild(UnityEditor.BuildTarget target, string path)
        {
            throw new NotImplementedException();
        }


        public static void Configure()
        {
            BuildConfig.Reset();
            string zone = CommandLineReader.GetCustomArgument(BuildConfig.KEY_ZONE, BuildConfig.ZONE);
            string market = CommandLineReader.GetCustomArgument("MARKET");
            string buildTarget = CommandLineReader.GetCustomArgument("BUILD_TARGET", BuildConfig.TARGET_ANDROID);
            var runtime = RuntimePlatform.Android;
            if (buildTarget == BuildConfig.TARGET_ANDROID)
            {
                runtime = RuntimePlatform.Android;
            } else if (buildTarget == BuildConfig.TARGET_IOS)
            {
                runtime = RuntimePlatform.IPhonePlayer;
            } else if (buildTarget == BuildConfig.TARGET_OSX)
            {
                runtime = RuntimePlatform.OSXPlayer;
            } else if (buildTarget == BuildConfig.TARGET_WIN)
            {
                runtime = RuntimePlatform.WindowsPlayer;
            } else if (buildTarget == BuildConfig.TARGET_WEBGL)
            {
                runtime = RuntimePlatform.WebGLPlayer;
            } else
            {
                runtime = EditorUserBuildSettings.activeBuildTarget.ToRuntimePlatform();
            }
            
            string buildConfigPath = string.Format("Assets/Resources/{0}.bytes", BuildConfig.FILE_NAME);
            PropertiesReader buildConfig = new PropertiesReader(buildConfigPath);
            buildConfig[BuildConfig.KEY_RUNTIME] = runtime.ToString();
            buildConfig[BuildConfig.KEY_PLATFORM] = runtime.GetPlatformName();
            buildConfig[BuildConfig.KEY_TARGET] = runtime.GetTargetName();
            buildConfig[BuildConfig.KEY_ZONE] = zone;
            buildConfig[BuildConfig.KEY_UNITY_VER] = Application.unityVersion;
            buildConfig[BuildConfig.KEY_BUILD_TIME] = System.DateTime.UtcNow.Ticks.ToString();
            
            ExecOutput rev = EditorUtil.ExecuteCommand("sh", "-c \"git rev-parse HEAD\"");
            if (!rev.IsError())
            {
                buildConfig[BuildConfig.KEY_REVISION] = rev.stdout.Trim();
            } else
            {
                throw new Exception(rev.stderr);
            }
            ExecOutput branch = EditorUtil.ExecuteCommand("sh", "-c \"git rev-parse --abbrev-ref HEAD\"");
            if (!branch.IsError())
            {
                string branchStr = branch.stdout.Trim();
                buildConfig[BuildConfig.KEY_DETAIL] = branchStr;
                if (branchStr.StartsWith("release/"))
                {
                    buildConfig[BuildConfig.KEY_VERSION] = branchStr.Substring("release/".Length);
                }
            } else
            {
                throw new Exception(rev.stderr);
            }
            
            File.WriteAllText(buildConfigPath, buildConfig.ToString());
            AssetDatabase.ImportAsset(buildConfigPath, ImportAssetOptions.ForceUpdate);
            BuildConfig.Reset();
            
            string platformConfigPath = "Assets/Resources/platform_config.bytes";
            PropertiesReader platformConfig = new PropertiesReader();
            platformConfig["market"] = market;
            // merge platform_config files
            platformConfig.LoadFile("Assets/platform/platform_config.bytes");
            platformConfig.LoadFile(string.Format("Assets/platform/platform_config_{0}.bytes", zone));
            platformConfig.LoadFile(string.Format("Assets/platform/platform_config_{0}.bytes", market));
            var file = string.Format("Assets/platform/platform_config_{0}_{1}.bytes", zone, market);
            if (File.Exists(file))
            {
                platformConfig.LoadFile(file);
            }
            if (File.Exists("Assets/platform/platform_config_test.bytes"))
            { 
                platformConfig.LoadFile("Assets/platform/platform_config_test.bytes");
            }
            File.WriteAllText(platformConfigPath, platformConfig.ToString());
            AssetDatabase.ImportAsset(platformConfigPath, ImportAssetOptions.ForceUpdate);

            BuildConfig.Reset();
            Platform.Reset();

            PlayerSettings.bundleVersion = BuildConfig.VERSION;
            if (runtime == RuntimePlatform.Android)
            {
                PlayerSettings.Android.bundleVersionCode = BuildConfig.VERSION_CODE;
            }
#if UNITY_5_6_OR_NEWER
            PlayerSettings.applicationIdentifier = 
#else
            PlayerSettings.bundleIdentifier = 
#endif
                Platform.conf.GetString("package_name", PlayerSettings.applicationIdentifier);
            AssetDatabase.SaveAssets();
        }

        public static void InitEditorLog()
        {
            BuildScript.log.level = LogLevel.DEBUG;
            AssetBuilder.log.level = LogLevel.DEBUG;
            TexFormatGroupEx.log.level = LogLevel.DEBUG;
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.Full);
            PlayerSettings.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
            PlayerSettings.SetStackTraceLogType(LogType.Assert, StackTraceLogType.Full);
            PlayerSettings.SetStackTraceLogType(LogType.Error, StackTraceLogType.Full);
            PlayerSettings.SetStackTraceLogType(LogType.Exception, StackTraceLogType.Full);
            PlayerSettings.SetStackTraceLogType(LogType.Warning, StackTraceLogType.Full);
        }

        public static void ConfigureAndroid()
        {
            string path = Platform.conf.GetString("KEYSTORE_FILE", null);
            if (path.IsEmpty())
            {
                return;
            }
            PlayerSettings.Android.keystoreName = Path.Combine(EditorAssetUtil.GetProjPath(), path);
            PlayerSettings.Android.keystorePass = Platform.conf.GetString("KEYSTORE_PW", string.Empty);
            PlayerSettings.Android.keyaliasName = Platform.conf.GetString("KEYSTORE_ALIAS_NAME", string.Empty);
            PlayerSettings.Android.keyaliasPass = Platform.conf.GetString("KEYSTORE_ALIAS_PW", string.Empty);
        }
        
        public static void LoadEditorDll()
        {
            foreach (string d in Directory.GetDirectories(Application.dataPath, "*Editor*", SearchOption.AllDirectories))
            {
                string dir = d.ToUnixPath();
                if (dir == "Editor"||dir.StartsWithIgnoreCase("Editor/")||dir.Contains("/Editor/")||dir.EndsWithIgnoreCase("/Editor"))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    FileInfo[] files = dirInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
                    foreach (FileInfo f in files)
                    {
                        if (f.Name.StartsWith("FreeType"))
                        {
                            continue;
                        }
                        Assembly.LoadFile(f.FullName);
                    }
                }
            }
        }
        
        public static void UpdateProjectSettings(bool versionUp)
        {
            string prjSettingPath = PathUtil.Combine(Application.dataPath, "../ProjectSettings/ProjectSettings.asset");
            if (versionUp)
            {
                string text = File.ReadAllText(prjSettingPath);
                Regex regex = new Regex(@"^(?<vc>\s*bundleVersion:\s*[0-9\.]+)", RegexOptions.Multiline);
                Match m = regex.Match(text);
                string ver = m.Groups["vc"].Value;
                int sep = ver.LastIndexOf('.');
                int minorVersion = int.Parse(ver.Substring(sep+1));
                string newVer = ver.Substring(0, sep+1)+(minorVersion+1);
                text = regex.Replace(text, newVer);
                log.Debug(string.Format("{0} -> {1}", ver, newVer));
                
                regex = new Regex(@"^(?<ver>\s*AndroidBundleVersionCode:\s*[0-9]+)", RegexOptions.Multiline);
                m = regex.Match(text);
                ver = m.Groups["ver"].Value;
                sep = ver.LastIndexOf(' ');
                if (sep < 0)
                {
                    sep = ver.LastIndexOf(':');
                }
                int verCode = int.Parse(ver.Substring(sep+1).Trim());
                newVer = ver.Substring(0, sep+1)+(verCode+1);
                text = regex.Replace(text, newVer);
                
                log.Debug(string.Format("{0} -> {1}", ver, newVer));
                File.WriteAllText(prjSettingPath, text);
            }
        }
        
        /*
        * done in configure.sh
    public static void UpdateBuildConfig()
    {
        string file = PathUtil.Combine(Application.dataPath, "Resources/build_config.bytes");
        string text = File.ReadAllText(file);
        text = SetVariable(text, "VERSION", PlayerSettings.bundleVersion);
        text = SetVariable(text, "VERSION_CODE", PlayerSettings.Android.bundleVersionCode);
        text = SetVariable(text, "BUILD_TIME", System.DateTime.Now.Ticks);
        ExecOutput rev = EditorUtil.ExecuteCommand("git", "rev-parse HEAD");
        text = SetVariable(text, "REPO_HASH", rev.stdout.Trim());
        File.WriteAllText(file, text);
        AssetDatabase.ImportAsset("Assets/Resources/build_config.bytes", ImportAssetOptions.ForceUpdate);
    }
        */
        
        private static string SetVariable(string input, string varName, object value)
        {
            Regex regex = new Regex(@"^\s*"+varName+@"\s*=.*$", RegexOptions.Multiline);
            if (regex.IsMatch(input))
            {
                return regex.Replace(input, varName+"="+value.ToString());
            } else
            {
                return string.Format("{0}\n{1}={2}", input.Trim(), varName, value.ToString());
            }
        }
        
        public static void DoNothing()
        {
        }
        
        public static void ConvertAssetBundleToText(string path)
        {
            #if UNITY_5_3
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            #else
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            #endif
            TextAsset[] csv = bundle.LoadAllAssets<TextAsset>();
            if (csv.IsNotEmpty())
            {
                string dst = PathUtil.ReplaceExtension(path, ".txt");           
                File.WriteAllBytes(dst, csv[0].bytes);
            }
        }
        
        /// <summary>
        /// Prebuild with specified AssetBuildProcess instances and all defined ComponentBuildProcess classes
        /// </summary>
        /// <param name="assetProcesses">Asset processes.</param>
        /// <param name="options">Options.</param>
        public static string PrebuildAll(params object[] options)
        {
            #if !UNITY_2017_3_OR_NEWER
            LibManager.CopyLibs();
            #endif
            LoadEditorDll();
            InitEditorLog();
            ResetPrebuilder();
            if (options != null)
            {
                log.Info("Prebuild options: ", StringUtil.Join(", ", options));
            }
            bool withoutCdn = ArrayUtility.Contains(options, EXCLUDE_CDN);
            EditorTraversal.ForEachAssetPath(path =>
            {
                // Cdn assets are pre-processed in asset build time 
                if (withoutCdn&&AssetBundlePath.inst.IsCdnPath(path))
                {
                    return null;
                }
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                ComponentBuildProcess.VerifyComponents(obj, options);
                return null;
            }, FileTypeEx.UNITY_SUPPORTED);
            EditorTraversal.ForEachAssetPath(path =>
            {
                // Cdn assets are pre-processed in asset build time 
                if (withoutCdn&&AssetBundlePath.inst.IsCdnPath(path))
                {
                    return null;
                }
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                ComponentBuildProcess.PreprocessComponents(obj, options);
                AssetBuildProcess.PreprocessAssets(path, obj, options);
                return null;
            }, FileTypeEx.UNITY_SUPPORTED);
            EditorTraversal.ForEachAssetPath(path =>
            {
                // Cdn assets are pre-processed in asset build time 
                if (withoutCdn&&AssetBundlePath.inst.IsCdnPath(path))
                {
                    return null;
                }
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                ComponentBuildProcess.PreprocessOver(obj, options);
                return null;
            }, FileTypeEx.UNITY_SUPPORTED);
            
            EditorTraversal.ForEachScene(roots=> PreprocessScene(roots, options));
            AssetDatabase.SaveAssets();
            
            return GetPrebuildMessage();
        }

        public static void PreprocessCurrentScene()
        {
            string err = PreprocessScene(EditorSceneManager.GetActiveScene().GetRootGameObjects().Convert(o => o.transform));
            if (err.IsNotEmpty())
            {
                throw new Exception(err);
            }
            EditorSceneManager.SaveOpenScenes();
        }

        public static string PreprocessScene(IEnumerable<Transform> roots, params object[] options)
        {
            foreach (Transform root in roots)
            {
                var transforms = root.GetComponentsInChildren<Transform>(true);
                foreach (Transform r in transforms)
                {
                    ComponentBuildProcess.VerifyComponents(r.gameObject, options);
                }
                SceneBuildProcess.PreprocessScenes(roots, options);
                foreach (Transform r in transforms)
                {
                    ComponentBuildProcess.PreprocessComponents(r.gameObject, options);
                }
                foreach (Transform r in transforms)
                {
                    ComponentBuildProcess.PreprocessOver(r.gameObject, options);
                }
            }
            return null;
        }

        /// <summary>
        /// Verification is ignored
        /// </summary>
        /// <param name="assetPaths">Asset paths.</param>
        /// <param name="options">Options.</param>
        public static void PrebuildAssets(string[] assetPaths, params object[] options)
        {
            string[] allPaths = AssetDatabase.GetDependencies(assetPaths);
            EditorTraversal.PreprocessAsset(allPaths, "Verify Asset (1/3)", (a, path)=>
            {
                ComponentBuildProcess.VerifyComponents(a, options);
            });
            EditorTraversal.PreprocessAsset(allPaths, "Prebuild (2/3)", (a, path)=> {
                ComponentBuildProcess.PreprocessComponents(a, options);
                AssetBuildProcess.PreprocessAssets(path, a, options);
            });
            EditorTraversal.PreprocessAsset(allPaths, "Prebuild Over (3/3)", (a, path)=> {
                ComponentBuildProcess.PreprocessOver(a, options);
            });
           
            AssetDatabase.SaveAssets();
        }
        
        public static void ResetPrebuilder()
        {
            ComponentBuildProcess.Reset();
            AssetBuildProcess.Reset();
            SceneBuildProcess.Reset();
        }
        
        public static string GetPrebuildMessage()
        {
            string assetErrors = AssetBuildProcess.GetErrorMessages();
            string sceneErrors = SceneBuildProcess.GetErrorMessages();
            string compErrors = ComponentBuildProcess.GetErrorMessages();
            return StringUtil.Join("\n", assetErrors, sceneErrors, compErrors);
        }

        public static void TestPostProcessBuild()
        {
            string path = EditorAssetUtil.GetProjPath();
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                path = PathUtil.Combine(path, "test.apk");
            } else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                path = PathUtil.Combine(path, "Build/iOS");
            }
            OnPostProcessBuild(EditorUserBuildSettings.activeBuildTarget, path);
        }

        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            Platform.Reset();
#if UNITY_ANDROID
            string manifestPath = PathUtil.Combine(path, Application.productName, "/unity-android-resources/AndroidManifest.xml");
            if (File.Exists(manifestPath))
            {
                AndroidManifest manifest = new AndroidManifest(manifestPath);
                manifest.Read();
                manifest.packageName = manifest.packageName+"_reslib";
                manifest.Write();
            }
#elif UNITY_IOS && PBX_PROJECT
            IosPostprocessor proc = new IosPostprocessor(path);
            proc.SetBitCode(Platform.conf.GetBool("XCODE_BITCODE", false));
            var frameworks = Platform.conf.GetString("XCODE_FRAMEWORKS", string.Empty).SplitCSV();
            foreach (var f in frameworks)
            {
                proc.AddSystemFramework(f);
            }
            var frameworkSearchPaths = Platform.conf.GetString("XCODE_FRAMEWORKSEARCHPATHS", string.Empty).SplitCSV();
            foreach (var p in frameworkSearchPaths)
            {
                proc.AddFrameworkSearchPath(p);
            }
            var headerPaths = Platform.conf.GetString("XCODE_HEADERPATHS", string.Empty).SplitCSV();
            foreach (var p in headerPaths)
            {
                proc.AddHeaderPath(p);
            }
            // XCODE_FILES items are start with 'Assets/'
            var files = Platform.conf.GetString("XCODE_FILES", string.Empty).SplitCSV();
            foreach (var f in files)
            {
                string projPath = PathUtil.Combine("Assets/", f);
                if (Directory.Exists(projPath))
                {
                    foreach (var file in EditorAssetUtil.ListAssetPaths(projPath, FileType.All, true))
                    {
                        if (!file.EndsWithIgnoreCase(".meta"))
                        {
                            proc.AddFile(file);
                        }
                    }
                    proc.AddHeaderPath(f.Substring("Assets/".Length));
                } else
                {
                    proc.AddFile("Assets/"+f);
                }
            }
            proc.Save();
#endif
        }
    }
}
