using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using commons;

namespace comunity
{
    [ExecuteInEditMode]
    public class DiService : MonoBehaviour
    {
        public const string PREF_PATH = "di";
        public const string ENABLED = "enabled";
        public DepData data;
        public static DiService inst { get; private set; }
        public static bool? _enableDi;
        public static bool enableDi {
            get
            {
                if (!_enableDi.HasValue)
                {
                    LoadPreference();
                }
                return _enableDi.Value;
            }
            set
            {
                _enableDi = value;
            }
        }

        void Awake()
        { 
            inst = this;
            LoadPreference();
            data.InitInjector(data.sources);
        }

        void OnDestroy()
        {
            if (inst == this)
            {
                inst = null;
            }
        }

        public T Get<T>() where T: MonoBehaviour
        {
            return data.Get<T>();
        }

        public static void LoadPreference()
        {
            TextAsset bytes = Resources.Load<TextAsset>(PREF_PATH);
            if (bytes != null)
            {
                PropertiesReader reader = new PropertiesReader(bytes);
                _enableDi = reader.GetBool(ENABLED, false);
            } else
            {
                _enableDi = false;
            }
        }

        public void Register(MonoBehaviour instance)
        {
            data.Register(instance);
        }

//        [RuntimeInitializeOnLoadMethod]
        public static DiService Create()
        {
            LoadPreference();
            if (enableDi)
            {
                DiService di = UnityEngine.Object.FindObjectOfType<DiService>();
                if (di == null)
                {
                    GameObject go = new GameObject("_di");
                    di = go.AddComponent<DiService>();
                }
                return di;
            }
            return null;
        }

        /// <summary>
        /// Resolves the scene.
        /// </summary>
        /// <returns><c>true</c>, if scene was changed, <c>false</c> otherwise.</returns>
//        [ContextMenu("Inject")]
        public List<MonoBehaviour> ResolveScene(MonoBehaviour[] all)
        {
            return data.ResolveScene(all);
        }

        /// <summary>
        /// Resolves the object.
        /// </summary>
        /// <returns><c>true</c>, if scene was changed, <c>false</c> otherwise.</returns>
        public bool Resolve(Transform root)
        {
            bool changed = false;
            foreach (MonoBehaviour m in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                changed |= data.Resolve(m);
            }
            return changed;
        }
    }
}

