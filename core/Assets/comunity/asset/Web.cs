//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using Object = UnityEngine.Object;
using System.IO;

namespace comunity
{
	public class Web
	{
		private static AssetCache _cache;

		public static AssetCache cache
		{
			get
			{
				if (_cache == null)
				{
					_cache = new AssetCache();
					#if UNITY_WEBGL
					_cache.SetCaching(true);
					#else
					cache.ToLocalRemote();
					#endif
				}
				return _cache;
			}
		}

		private static AssetCache _noCache;

		public static AssetCache noCache
		{
			get
			{
				if (_noCache == null)
				{
					_noCache = new AssetCache();
					_noCache.SetCaching(false);
				}
				return _noCache;
			}
		}
	}
}
