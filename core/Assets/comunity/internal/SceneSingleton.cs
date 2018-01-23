using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.SceneManagement;
using commons;

namespace core
{

    /// <summary>
    /// Destroyed when the specified scene is over
    /// Singleton should not be disabled
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public abstract class SceneSingleton<T> : Script where T : SceneSingleton<T>
    {
        protected virtual string activeScene { get; private set; }

        protected static T singleton;

        public static T inst
        {
            get
            {
                if (singleton == null&&IsPlaying())
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    T t = obj.AddComponent(typeof(T)) as T;
                    singleton = t;
                    singleton.Init();
                }
                return singleton;
            }
        }

        private static bool IsPlaying()
        {
            if (!Application.isEditor)
            {
                return true;
            }
            System.Type type = ReflectionUtil.GetType("UnityEditor.EditorApplication");
            PropertyInfo p = type.GetProperty("isPlayingOrWillChangePlaymode", BindingFlags.Static|BindingFlags.Public);
            MethodInfo method = p.GetGetMethod();
            bool isChange = (bool)method.Invoke(null, null);
            return Application.isPlaying&&isChange;
        }

        public static bool IsInitialized()
        {
            return singleton != null;
        }

        private void Init()
        {
            DontDestroyOnLoad(go);
            hideFlags = HideFlags.HideAndDontSave;
            activeScene = SceneManager.GetActiveScene().name;
        }

        private void OnSceneChange(Scene s1, Scene s2)
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
            Destroy(go);
        }

        protected virtual void Awake()
        {
            if (singleton == null)
            {
                singleton = this as T;
                Init();
                SceneManager.activeSceneChanged += OnSceneChange;
            } else
            {
                if (singleton != this)
                {
                    if (!IsEditorOnly())
                    {
                        log.Error("Duplicate Singleton {0}", GetType());
                    }
                    Destroy(gameObject);
                }
            }
        }

        private bool IsEditorOnly()
        {
            Transform t = transform;
            while (t != null)
            {
                if (t.CompareTag("EditorOnly"))
                {
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