#if UNITY_IOS
using System;
using UnityEngine;
using System.Diagnostics;


namespace core
{
	public class IosPlatformMethods : IPlatformMethods
	{
		public void SetNoBackupFlag(string path)
		{
			UnityEngine.iOS.Device.SetNoBackupFlag(path);
		}
		
		public void SetNoBackupFlag(string path, int version)
		{
			Caching.SetNoBackupFlag(path, version);
		}
	}
}
#endif


