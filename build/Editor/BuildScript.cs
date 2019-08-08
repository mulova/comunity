using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using Object = UnityEngine.Object;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using mulova.commons;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using comunity;
using System.Collections.Generic.Ex;
using System.Text.Ex;
using System.Ex;

namespace build
{
	public static class BuildScript
	{
		public const string VERIFY_ONLY = "verify_only";
		public const string EXCLUDE_CDN = "exclude_cdn";
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

		public static readonly Loggerx log = LogManager.GetLogger(typeof(BuildScript));

		private static bool DisplayProgressBar(string title, string info, float progress)
		{
			if (SystemInfo.graphicsDeviceID != 0)
			{
				return EditorUtility.DisplayCancelableProgressBar(title, info, progress);
			}
			log.Debug("{0} ({1:P2})", info, progress);
			return false;
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
			return errors.Join("\n");
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
				SaveScene();
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
				string[] paths = EditorAssetUtil.ListAssetPaths("Assets", fileType, false);
				log.Info("Prebuild {0:D0} assets", paths.Length);
                EditorGUIUtil.DisplayProgressBar(paths, "Asset Processing", false, p=> {
                    if (ignorePath.IsMatch(p))
                    {
                        return;
                    }
                    string error = func(p);
                    if (error.IsNotEmpty())
                    {
                        err.Append(error).Append("\n");
                    }
                });
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

		public static void Configure()
		{
			BuildConfig.Reset();
            string zone = CommandLineReader.GetCustomArgument(nameof(BuildConfig.ZONE), BuildConfig.ZONE);
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
            buildConfig[nameof(BuildConfig.RUNTIME)] = runtime.ToString();
			buildConfig[nameof(BuildConfig.PLATFORM)] = runtime.GetPlatformName();
			buildConfig[nameof(BuildConfig.TARGET)] = runtime.GetTargetName();
			buildConfig[nameof(BuildConfig.ZONE)] = zone;
            buildConfig[nameof(BuildConfig.UNITY_VER)] = Application.unityVersion;
            buildConfig[nameof(BuildConfig.BUILD_TIME)] = System.DateTime.UtcNow.Ticks.ToString();

			ExecOutput rev = EditorUtil.ExecuteCommand("sh", "-c \"git rev-parse HEAD\"");
			if (!rev.IsError())
			{
                buildConfig[nameof(BuildConfig.REVISION)] = rev.stdout.Trim();
			} else
			{
				throw new Exception(rev.stderr);
			}
			ExecOutput branch = EditorUtil.ExecuteCommand("sh", "-c \"git rev-parse --abbrev-ref HEAD\"");
			if (!branch.IsError())
			{
				string branchStr = branch.stdout.Trim();
				buildConfig[nameof(BuildConfig.DETAIL)] = branchStr;
				if (branchStr.StartsWith("release/"))
				{
                    buildConfig[nameof(BuildConfig.VERSION)] = branchStr.Substring("release/".Length);
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
			//            AssetBuilder.log.level = LogLevel.DEBUG;
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

		private static bool sceneProcessing;

		public static void SetDirty(Object o)
		{
			EditorUtil.SetDirty(o);
			if (sceneProcessing)
			{
				EditorSceneBridge.MarkSceneDirty();
			}
		}

		public static void SaveScene()
		{
			if (!EditorSceneBridge.currentScene.IsEmpty()&&EditorSceneBridge.currentScene != "Untitled")
			{
				if (EditorSceneBridge.isSceneDirty)
				{
					EditorSceneBridge.SaveScene();
				}
			}
		}

		public static void ConvertAssetBundleToText(string path)
		{
			AssetBundle bundle = AssetBundle.LoadFromFile(path);
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
			//            LibManager.CopyLibs();
			BuildScript.LoadEditorDll();
			BuildScript.InitEditorLog();
			ResetPrebuilder();
			if (options != null)
			{
				log.Info("Prebuild options: ", options.Join(", "));
			}
			bool withoutCdn = ArrayUtility.Contains(options, EXCLUDE_CDN);
			ForEachAssetPath(path =>
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
			ForEachAssetPath(path =>
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
			ForEachAssetPath(path =>
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

			ForEachScene(roots=> PreprocessScene(roots, options));
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

		private static void PreprocessAsset(string[] allPaths, string progressTitle, Action<Object, string> preprocess)
		{
            EditorGUIUtil.DisplayProgressBar(allPaths, progressTitle, true, path => {
                if (ignorePath.IsMatch(path))
                {
                    return;
                }
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                preprocess(asset, path);
            });
		}

		/// <summary>
		/// Verification is ignored
		/// </summary>
		/// <param name="assetPaths">Asset paths.</param>
		/// <param name="options">Options.</param>
		public static void PrebuildAssets(string[] assetPaths, params object[] options)
		{
			string[] allPaths = AssetDatabase.GetDependencies(assetPaths);
			PreprocessAsset(allPaths, "Verify Asset (1/3)", (a, path)=>
				{
					ComponentBuildProcess.VerifyComponents(a, options);
				});
			PreprocessAsset(allPaths, "Prebuild (2/3)", (a, path)=> {
				ComponentBuildProcess.PreprocessComponents(a, options);
				AssetBuildProcess.PreprocessAssets(path, a, options);
			});
			PreprocessAsset(allPaths, "Prebuild Over (3/3)", (a, path)=> {
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
			return string.Join("\n", assetErrors, sceneErrors, compErrors);
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
