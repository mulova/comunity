/*
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;
using UnityEditor.DPLEditor;
using mulova.tool;
using mulova.util;
using mulova.asset;
using System.Text;
using System.Collections.Generic;
using DPL;
using asset.ex;
using Object = UnityEngine.Object;
using com.dryad.build;
using mulova.log;
using System.Xml.Serialization;
using UnityEditor.iOS.Xcode;

public static class Builder
{
	public const string APP_ID = "svwar";
	public const string NZ_BETA = "Beta";
	public const string NZ_LIVE = "Live";
	public const string IOS_BUILD_DIR = "Build/iOS";

	public static void DoNothing()
	{
		Debug.Log("Import...");
	}

	private static bool Is(BuildTarget target)
	{
		return EditorUserBuildSettings.activeBuildTarget == target;
	}

	private static string GetZone()
	{
		return BuildConfig.ZONE;
	}

	private static TexFormatGroup GetTexFormatGroup()
	{
		return TexFormatGroup.ParseIgnoreCase(CommandLineReader.GetCustomArgument("tex_format"));
	}

	public static void Build()
	{
		if (Is(BuildTarget.Android))
		{
			BuildWithArgs("Build/Android/svw.apk");
		} else if (Is(BuildTarget.iOS))
		{
			BuildWithArgs(IOS_BUILD_DIR);
		}
	}

	private static void SetBuildTarget(BuildTarget t)
	{
		if (EditorUserBuildSettings.activeBuildTarget != t)
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(t);
		}
	}

	private static void ConfigureAndroid()
	{
		//		PlayerSettings.Android.keystoreName = Path.Combine(Path.GetDirectoryName(Application.dataPath), "keystore/dryad.keystore");
		//		PlayerSettings.Android.keystorePass = "drydev10518";
		//		PlayerSettings.Android.keyaliasName = "yovillain";
		//		PlayerSettings.Android.keyaliasPass = "drydev10518";
		//		PlayerSettings.Android.keystorePass = "drydev10518";
		//
		if(PlatformConfig.PACKAGE_NAME.Contains("sway")){
			PlayerSettings.Android.keystoreName = Path.Combine(Path.GetDirectoryName(Application.dataPath), "keystore/sway_sv.keystore");
			PlayerSettings.Android.keystorePass = "swaysway";
			PlayerSettings.Android.keyaliasName = "official";
			PlayerSettings.Android.keyaliasPass = "swaysway";
			PlayerSettings.Android.keystorePass = "swaysway";
		}
		else if (PlatformConfig.PACKAGE_NAME.Contains("paranoid"))
		{
			PlayerSettings.Android.keystoreName = Path.Combine(Path.GetDirectoryName(Application.dataPath), "keystore/paranoid_yv.keystore");
			PlayerSettings.Android.keystorePass = "parapara";
			PlayerSettings.Android.keyaliasName = "paranoidyv";
			PlayerSettings.Android.keyaliasPass = "parapara";
			PlayerSettings.Android.keystorePass = "parapara";
		}
		else{
			PlayerSettings.Android.keystoreName = Path.Combine(Path.GetDirectoryName(Application.dataPath), "keystore/dryad.keystore");
			PlayerSettings.Android.keystorePass = "drydev10518";
			PlayerSettings.Android.keyaliasName = "yovillain";
			PlayerSettings.Android.keyaliasPass = "drydev10518";
			PlayerSettings.Android.keystorePass = "drydev10518";
		}
		PlayerSettings.allowedAutorotateToLandscapeLeft = true;
		PlayerSettings.allowedAutorotateToLandscapeRight = true;
		PlayerSettings.showUnitySplashScreen = false;
		SettingEditor.RegenerateManifest(SettingEditor.GetSetting());
	}

	public static BuildOptions buildOption
	{
		get
		{
			BuildOptions option = PlatformConfig.DEBUGGABLE? BuildOptions.Development: BuildOptions.None;
			if (BuildConfig.TEST)
			{
				option |= BuildOptions.AllowDebugging;
			}
			return option;
		}
	}

	public static void BuildWithArgs(string buildDir)
	{
		string result = Build(buildDir);
		if (result.IsNotEmpty())
		{
			throw new Exception(result);
		}
	}

	public static void InitEditorBuild()
	{
		BuildScript.LoadEditorDll();
		EditorAssetLoaderEx.SetEditorCdn();
		BuildScript.log.SetLevel(LogLevel.DEBUG);
		AssetBuilder.log.SetLevel(LogLevel.DEBUG);
		TexFormatGroupEx.log.SetLevel(LogLevel.DEBUG);
	}

	public static void Configure()
	{
		InitEditorBuild();
		SetDPLSetting();
	}

	public static void PrebuildTest()
	{
		//        Prebuild("Build/iOS", BuildOptions.Development, string.Empty);

	}

	public static void Prebuild()
	{
		string assetDir = CommandLineReader.GetCustomArgument("asset_dir");
		Prebuild(assetDir);
	}

	public static void Prebuild(string assetDir)
	{
		Configure();
		string err = BuildScript.PrebuildAll(buildOption, GetTexFormatGroup(), BuildScript.EXCLUDE_CDN);
		BuildScript.UpdateProjectSettings(false);
		CopyToStreamingAssets(assetDir);
		if (err.IsNotEmpty())
		{
			throw new Exception(err);
		}
	}

	private static void SetDPLSetting()
	{
		#if !UNITY_WEBPLAYER
		Setting s = AssetDatabase.LoadAssetAtPath<Setting>("Assets/DPL/Resources/DPLSetting.asset");
		s.Market = DPLEx.GetMarketStr();
		EditorUtility.SetDirty(s);
		EditorApplication.SaveAssets();
		#endif
	}

	public static string Build(string buildDir)
	{
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
		{
			ConfigureAndroid();
		}
		buildDir = Path.Combine(Path.GetDirectoryName(Application.dataPath), buildDir);

		Debug.Log("Build to: "+buildDir);

		string[] scenes = (from scene in EditorBuildSettings.scenes
			where scene.enabled == true
			select scene.path).ToArray();
		//#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && UNITY_ANDROID
		//		EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.DXT;
		//#endif

		string res = BuildPipeline.BuildPlayer(scenes, buildDir, EditorUserBuildSettings.activeBuildTarget, buildOption);

		if (string.IsNullOrEmpty(res))
		{
			Debug.Log("Build succeeded!");
		} else
		{
			Debug.LogError("An error occured while building : "+res);
		}
		return res;
	}

	public static void BuildRes()
	{
		InitEditorBuild();
		string zone = GetZone();
		TexFormatGroup texFormat = GetTexFormatGroup();
		AssetBuilder builder = new AssetBuilder(zone, EditorUserBuildSettings.activeBuildTarget, texFormat);
		builder.Build();
		bool clean = false;
		bool.TryParse(CommandLineReader.GetCustomArgument("append_clean").ToLower(), out clean);
		if (clean)
		{
			AssetBuilder cleanBuilder = new AssetBuilder(zone, EditorUserBuildSettings.activeBuildTarget, texFormat, "", builder.newVersion+"c");
			cleanBuilder.Build();
		}
	}

	public static void Verify()
	{
		string err = VerifyRes();
		if (err.IsNotEmpty())
		{
			throw new Exception(err);
		}
	}

	public static string VerifyRes()
	{
		InitEditorBuild();
		// verify tables
		string error1 = HeroVerifier.VerifyTables();
		// verify particles
		string error3 = Verifier.VerifyBattleEffectTable();
		string error4 = Verifier.VerifyThumbnail("Assets/res/download/hero");
		string error6 = Verifier.VerifyHeroNameCSV();

		string err = string.Join("\n", error1, error3, error4, error6);
		return err.Trim();
	}

	public static void RemoveStreamingAssets()
	{
		string dstDir = "Assets/StreamingAssets/"+FileAssetLoader.CDN_DIR2;
		if (Directory.Exists(dstDir))
		{
			Debug.LogFormat("Cleanup dir "+dstDir);
			AssetUtil.DeleteDirectory(dstDir);
		}
		string versionFile = "Assets/StreamingAssets/"+StreamingAssetLoader.VERSION_FILE;
		if (File.Exists(versionFile))
		{
			File.Delete(versionFile);
		}
	}

	public static void CopyToStreamingAssets(string srcDir)
	{
		#if !UNITY_WEBPLAYER
		RemoveStreamingAssets();
		if (srcDir.IsNotEmpty())
		{
			string rootdir = PathUtil.Combine(srcDir, EditorUserBuildSettings.activeBuildTarget);
			int ver = GetLatestVersion(rootdir);
			string dstDir = "Assets/StreamingAssets/"+FileAssetLoader.CDN_DIR2;
			Directory.CreateDirectory(dstDir);
			if (ver > 0)
			{
				Debug.LogFormat("Set Streaming Assets. version: {0:D3}", ver);
				// write asset version of files in StreamingAssets/
				File.WriteAllText("Assets/StreamingAssets/"+StreamingAssetLoader.VERSION_FILE, ver.ToString("D3"));
				string listFile = string.Format("{0}/{1:D3}/filelist_{1:D3}.txt", rootdir, ver);
				foreach (string f in File.ReadAllLines(listFile))
				{
					string dir = GetPrefixNumber(f);
					string path = PathUtil.Combine(rootdir, dir, f);
					Debug.Log("Unzip Asset "+path);
					int[] progress = new int[1];
					lzip.decompress_File(path, dstDir, progress);
				}
				foreach (FileInfo f in AssetUtil.ListFiles(dstDir, "digest_*.txt"))
				{
					f.Delete();
				}
			}
		}
		#endif
	}

	private static string GetPrefixNumber(string s)
	{
		StringBuilder str = new StringBuilder(s.Length);
		foreach (char c in s)
		{
			if (Char.IsDigit(c))
			{
				str.Append(c);
			} else
			{
				break;
			}
		}
		return str.ToString();
	}

	private static int GetLatestVersion(string dir)
	{
		if (!Directory.Exists(dir))
		{
			return 0;
		}
		int ver = 0;
		foreach (string d in Directory.GetDirectories(dir))
		{
			string lastDir = Path.GetFileName(d);
			int v = 0;
			if (int.TryParse(lastDir, out v))
			{
				ver = Math.Max(ver, v);
			}
		}
		return ver;
	}
}
*/