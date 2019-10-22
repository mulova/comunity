//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using UnityEngine;

namespace mulova.comunity
{
	public static class BuildTargetEx
	{
		public static RuntimePlatform ToRuntimePlatform(this UnityEditor.BuildTarget target)
		{
            switch (target)
            {
                case UnityEditor.BuildTarget.StandaloneOSX:
                    return RuntimePlatform.OSXPlayer;
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    return RuntimePlatform.WindowsPlayer;
                case UnityEditor.BuildTarget.iOS:
                    return RuntimePlatform.IPhonePlayer;
                case UnityEditor.BuildTarget.Android:
                    return RuntimePlatform.Android;
                case UnityEditor.BuildTarget.StandaloneLinux64:
                    return RuntimePlatform.LinuxPlayer;
                case UnityEditor.BuildTarget.WebGL:
                    return RuntimePlatform.WebGLPlayer;
                case UnityEditor.BuildTarget.tvOS:
                    return RuntimePlatform.tvOS;
                case UnityEditor.BuildTarget.Switch:
                    return RuntimePlatform.Switch;
                case UnityEditor.BuildTarget.Lumin:
                    return RuntimePlatform.Lumin;
                case UnityEditor.BuildTarget.Stadia:
                    return RuntimePlatform.Stadia;
                default:
                    return RuntimePlatform.Android;
            }
        }
    }
}

