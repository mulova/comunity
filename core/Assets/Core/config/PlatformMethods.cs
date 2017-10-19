//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace core
{
	public static class PlatformMethods
	{
		public static IPlatformMethods inst = 
#if UNITY_ANDROID
			new AndroidPlatformMethods();
#elif UNITY_IOS
			new IosPlatformMethods();
#else
			new DummyPlatformMethods();
#endif
	}

	class DummyPlatformMethods : IPlatformMethods
	{
		public void SetNoBackupFlag(string path)
		{
		}

		public void SetNoBackupFlag(string path, int version)
		{
		}
	}
}
