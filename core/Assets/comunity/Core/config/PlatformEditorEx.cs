using System;
using UnityEngine;

public static class PlatformEditorEx
{
	public static RuntimePlatform GetPlatform() {
		switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget) {
			case UnityEditor.BuildTarget.StandaloneOSXIntel:
			case UnityEditor.BuildTarget.StandaloneOSXIntel64:
            #if UNITY_2017_3_OR_NEWER
            case UnityEditor.BuildTarget.StandaloneOSX:
            #else
            case BuildTarget.StandaloneOSXUniversal:
            #endif
				return RuntimePlatform.OSXPlayer;
			case UnityEditor.BuildTarget.StandaloneWindows:
			case UnityEditor.BuildTarget.StandaloneWindows64:
				return RuntimePlatform.WindowsPlayer;
			case UnityEditor.BuildTarget.iOS:
				return RuntimePlatform.IPhonePlayer;
			case UnityEditor.BuildTarget.Android:
				return RuntimePlatform.Android;
			case UnityEditor.BuildTarget.StandaloneLinux:
			case UnityEditor.BuildTarget.StandaloneLinux64:
			case UnityEditor.BuildTarget.StandaloneLinuxUniversal:
				return RuntimePlatform.LinuxPlayer;
			case UnityEditor.BuildTarget.WebGL:
				return RuntimePlatform.WebGLPlayer;
			default:
				return RuntimePlatform.Android;
		}
	}
}
