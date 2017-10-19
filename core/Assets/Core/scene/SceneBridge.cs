#if UNITY_5_3_OR_NEWER
#define MULTI_SCENE
#endif

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace core
{
	public static class SceneBridge
	{

		public static string loadedLevelName
		{
			get {
				#if MULTI_SCENE
				return SceneManager.GetActiveScene().name;
				#else
				return Application.loadedLevelName;
				#endif
			}
		}

	}
}


