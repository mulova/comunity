#if UNITY_IOS
using System;
using UnityEngine;
using System.Diagnostics;


namespace comunity
{
	public class IosPlatformMethods : IPlatformMethods
	{
		public void SetNoBackupFlag(string path)
		{
			UnityEngine.iOS.Device.SetNoBackupFlag(path);
		}
		
        [Obsolete]
		public void SetNoBackupFlag(string path, int version)
		{
			Caching.SetNoBackupFlag(path, version);
		}

        public void SetNoBackupFlag(string path, Hash128 hash)
        {
            Caching.SetNoBackupFlag(path, hash);
        }
	}
}
#endif


