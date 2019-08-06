//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.IO;
using mulova.commons;

namespace comunity
{
	public static class Platform {
		public static SimulationMode simulationMode = SimulationMode.Editor;
		private static bool debug;
		private static bool playing;
		private static bool editor;
		private static RuntimePlatform platform0 = RuntimePlatform.Android;
		private static string persistentPath;
		private static string dlPath;
		private static string streamingPath;
		private static string tempCachePath;
		private static string path;
		private static bool init;
		
		public static bool IsInitialized() {
			return init;
		}

		public static void Init() {
			try {
				if (!init) {
					playing = Application.isPlaying;
					editor = Application.isEditor;
					persistentPath = Application.persistentDataPath;
					tempCachePath = Application.temporaryCachePath;
					path = Application.dataPath;
					debug = Debug.isDebugBuild;
					platform0 = GetPlatform();
					streamingPath = editor? Path.Combine(path, "StreamingAssets"): Application.streamingAssetsPath;
					if (playing) {
						init = true;
					}
				}
			} catch (System.Exception ex) {
				UnityEngine.Debug.LogError(ex);
			}
		}
		
		public static bool isPlaying {
			get {
				Init();
				return playing;
			}
		}
		
		/// <summary>
		/// Gets the persistent data path.
		/// Don't use this in Constructor
		/// </summary>
		/// <value>The persistent data path.</value>
		public static string persistentDataPath {
			get {
				Init();
				return persistentPath;
			}
		}
		
		public static string streamingAssetsPath {
			get {
				Init();
				return streamingPath;
			}
		}
		
		public static string temporaryCachePath {
			get {
				Init ();
				return tempCachePath;
			}
		}
		
		public static string downloadPath {
			get {
				Init();
				if (dlPath == null) {
					if (platform.IsIos()) {
						return temporaryCachePath;
					} else {
						return persistentDataPath;
					}
				}
				return dlPath;
			}
			set {
				dlPath = value;
			}
		}
		
		/// <summary>
		/// Gets the data path.
		/// Don't use this in Constructor
		/// </summary>
		/// <value>The data path.</value>
		public static string dataPath {
			get {
				Init();
				return path;
			}
		}
		
		/// <summary>
		/// Don't use this in Constructor
		/// </summary>
		/// <value><c>true</c> if is build; otherwise, <c>false</c>.</value>
		public static bool isBuild {
			get {
				return !isEditor;
			}
		}
		
		/// <summary>
		/// Don't use this in Constructor
		/// </summary>
		/// <value><c>true</c> if is editor; otherwise, <c>false</c>.</value>
		public static bool isEditor {
			get {
				Init();
				if (!editor) {
					return false;
				}
				return !simulationMode.IsBuild();
			}
		}
		
		public static bool isReleaseBuild {
			get {
				if (simulationMode == SimulationMode.ReleaseBuild) {
					return true;
				}
				return isBuild && !debug;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="Platform"/> is not release build.
		/// </summary>
		/// <value><c>true</c> if current status is not release build otherwise, <c>false</c>.</value>
		public static bool isDebug {
			get {
				return !isReleaseBuild;
			}
		}
		
		public static RuntimePlatform platform {
			get {
				Init();
				return platform0;
			}
			set {
				Init();
				platform0 = value;
			}
		}
		
        private static PropertiesReader _conf;
		public static PropertiesReader conf {
			get {
				if (_conf == null) {
					_conf = new PropertiesReader();
					_conf.LoadResource("platform_config");
				}
				return _conf;
			}
		}

        public static void Reset()
        {
            init = false;
            _conf = null;
        }

		private static RuntimePlatform GetPlatform()
		{
			if (editor)
			{
                return (RuntimePlatform)ReflectionUtil.ExecuteMethod("comunity.PlatformEditorEx.GetPlatform");
			} else
			{
				return Application.platform;
			}
		}
	}
}
