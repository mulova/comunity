using UnityEngine;
using mulova.comunity;

namespace mulova.comunity
{
	public static class RuntimePlatformEx
	{
		public static bool IsMobile(this RuntimePlatform platform)
		{
			return platform == RuntimePlatform.Android
				||platform == RuntimePlatform.IPhonePlayer;
		}
		
		public static bool IsStandalone(this RuntimePlatform platform)
		{
			return platform.IsOSX()
				||platform.IsWindows()
				||platform.IsLinux();
		}
		
		public static bool IsIos(this RuntimePlatform platform)
		{
			return platform == RuntimePlatform.IPhonePlayer;
		}
		
		public static bool IsOSX(this RuntimePlatform platform)
		{
			return platform == RuntimePlatform.OSXPlayer
				||platform == RuntimePlatform.OSXEditor;
		}
		
		public static bool IsWindows(this RuntimePlatform platform)
		{
			return platform == RuntimePlatform.WindowsEditor
				||platform == RuntimePlatform.WindowsPlayer;
		}
		
		public static bool IsLinux(this RuntimePlatform platform)
		{
			return platform == RuntimePlatform.LinuxPlayer;
		}
		
		public static bool IsWeb(this RuntimePlatform platform)
		{
			return platform == RuntimePlatform.WebGLPlayer;
		}
		
		public static string GetPlatformName(this RuntimePlatform platform)
		{
			switch (platform)
			{
				case RuntimePlatform.Android:
					return BuildConfig.TARGET_ANDROID;
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.OSXPlayer:
					return BuildConfig.TARGET_OSX;
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					return BuildConfig.TARGET_WIN;
				case RuntimePlatform.IPhonePlayer:
					return BuildConfig.TARGET_IOS;
				case RuntimePlatform.LinuxPlayer:
					return BuildConfig.TARGET_LINUX;
				case RuntimePlatform.WebGLPlayer:
					return BuildConfig.TARGET_WEBGL;
				default:
					return "none";
			}
		}
		
		public static string GetTargetName(this RuntimePlatform platform)
		{
			switch (platform)
			{
				case RuntimePlatform.Android:
					return "android";
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.OSXPlayer:
					return "osx";
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					return "win64";
				case RuntimePlatform.IPhonePlayer:
					return "ios";
				case RuntimePlatform.LinuxPlayer:
					return "linux64";
				case RuntimePlatform.WebGLPlayer:
					return "webgl";
				default:
					return "none";
			}
		}
		
		public static string GetAbCategory(this RuntimePlatform platform, TexFormatGroup group)
		{
			string dir = platform.IsStandalone()? "pc" : platform.GetPlatformName();
			if (group != TexFormatGroup.AUTO)
			{
				dir = string.Concat(dir, "_", group.id);
			}
			return dir;
		}
		
		public static string GetAbCategory()
		{
			return GetAbCategory(Platform.platform, TexFormatGroup.GetBest());
		}
	}
}

