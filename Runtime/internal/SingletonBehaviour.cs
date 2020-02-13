using UnityEngine;
using System.Collections;
using System.Reflection;
using mulova.commons;
using System.Ex;

namespace mulova.comunity {

	/// <summary>
	/// Singleton should not be disabled
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough]
	public abstract class SingletonBehaviour<T> : LogBehaviour where T : SingletonBehaviour<T>
	{
		protected static T singleton;

		public static T inst
		{
			get
			{
                #if AUTO_GENERATION
				if (singleton == null && IsPlaying()) {
					GameObject obj = new GameObject();
					obj.name = typeof(T).Name;
					T t = obj.AddComponent(typeof(T)) as T;
					singleton = t;
					singleton.Init();
				}
                #endif
				return singleton;
			}
		}

		private static bool IsPlaying() {
			if (!Application.isEditor) {
				return true;
			}
            System.Type type = TypeEx.GetType("UnityEditor.EditorApplication");
			PropertyInfo p = type.GetProperty("isPlayingOrWillChangePlaymode", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method = p.GetGetMethod();
			bool isChange = (bool) method.Invoke(null, null);
			return Application.isPlaying && isChange;
		}

		public static bool IsInitialized() {
			return singleton != null;
		}

		protected virtual void Awake()
		{
			if (singleton == null)
			{
				singleton = this as T;
			}
			else
			{
				if (singleton != this)
				{
					if (!IsEditorOnly()) {
						log.Error("Duplicate Singleton {0}", GetType());
					}
					Destroy(gameObject);
				}
			}
		}

		private bool IsEditorOnly() {
			Transform t = transform;
			while (t != null) {
				if (t.CompareTag("EditorOnly")) {
					return true;
				}
				t = t.parent;
			}
			return false;
		}

		protected virtual void OnDestroy()
		{
			if (singleton == this)
			{
				singleton = null;
			}
		}
	}

}