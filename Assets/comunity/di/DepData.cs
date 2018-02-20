using System;
using UnityEngine;
using System.Collections.Generic;
using commons;

namespace comunity
{
    [Serializable]
    public class DepData
    {
        public List<MonoBehaviour> sources = new List<MonoBehaviour>();
        private DepInjector<MonoBehaviour> injector;

        public T Get<T>() where T: MonoBehaviour
        {
            return injector.GetSource(typeof(T)) as T;
        }

        public void Register(MonoBehaviour instance)
        {
            if (injector.RegisterIf(instance, typeof(InjectionSourceAttribute)))
            {
                sources.Add(instance);
            }
        }

        /// <summary>
        /// Resolves the scene.
        /// </summary>
        /// <returns><c>true</c>, if scene was changed, <c>false</c> otherwise.</returns>
//        [ContextMenu("Inject")]
        public List<MonoBehaviour> ResolveScene(MonoBehaviour[] all)
        {
            List<MonoBehaviour> changeList = new List<MonoBehaviour>();
            InitInjector(all);

            foreach (MonoBehaviour m in all)
            {
                if (injector.Resolve(m))
                {
                    changeList.Add(m);
                }
            }
            return changeList;
        }

        public void InitInjector(IEnumerable<MonoBehaviour> source)
        {
            injector = new DepInjector<MonoBehaviour>();
            // register source
            foreach (MonoBehaviour s in source)
            {
                injector.RegisterIf(s, typeof(InjectionSourceAttribute));
            }
            sources = injector.GetSources();
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
                changed |= injector.Resolve(m);
            }
            return changed;
        }

        public bool Resolve(MonoBehaviour m)
        {
            return injector.Resolve(m);
        }
    }
}

