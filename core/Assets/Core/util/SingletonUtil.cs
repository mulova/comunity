//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using Object = UnityEngine.Object;
using commons;


namespace core {
	public static class SingletonUtil {
		
		public static void Awake<T>(ref T singleton, T instance) where T:Component {
			if (singleton == null)
			{
				singleton = instance;
			}
			else
			{
				if (singleton != instance)
				{
					if (!instance.CompareTag("EditorOnly")) {
                        LogManager.GetLogger(typeof(T)).context = instance;
						LogManager.GetLogger(typeof(T)).Warn("Destroying subsidiary {0}.{1} singleton instance.\nSet 'EditorOnly' tag if this is for test.", instance.transform.GetScenePath(), typeof(T).FullName);
					}
					if (Application.isPlaying) {
						instance.gameObject.DestroyEx();
					}
				}
			}
		}
		
		public static void OnDestroy<T>(ref T singleton, T instance) where T:Component {
			if (singleton == instance)
			{
				singleton = null;
			}
		}

		public static T GetInstance<T>(ref T singleton) where T:new() {
			if (singleton == null)
			{
				lock(typeof(T)) {
					if (singleton == null) {
						singleton = new T();
					}
				}
			}
			return singleton;
		}
	}
}
