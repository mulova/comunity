//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using UnityEngine;

namespace comunity
{
	public static class BuildTargetEx
	{
		public static RuntimePlatform ToRuntimePlatform(this UnityEditor.BuildTarget target)
		{
			switch (target)
			{
#pragma warning disable 0618
				case UnityEditor.BuildTarget.StandaloneOSXIntel:
#pragma warning restore 0618
#pragma warning disable 0618
				case UnityEditor.BuildTarget.StandaloneOSXIntel64:
#pragma warning restore 0618
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
}

