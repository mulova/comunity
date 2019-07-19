using System.Collections.Generic;
using UnityEngine;
using commons;
using System.Collections.Generic.Ex;
using UnityEngine.Ex;

namespace comunity
{
    public class GOPoolElement 
    {
        public Queue<GameObject> pool;
        public GameObject prefab;
        
        public GOPoolElement(GameObject pref) 
        {
            pool = new Queue<GameObject> ();
            prefab = pref;
        }
    }
    
    public class PoolElement
    {
    }
    
    /// <summary>
    /// You can get or release prefab obj that registed
    /// </summary>
    public class GameObjectPool : MonoBehaviour
    {
        private Dictionary<string, GOPoolElement> poolMap;
        private Transform inactivePool; // inactive instance's parent
        
        private void Awake() 
        {
            Init();
        }
        
        /// <summary>
        /// Make inactive pool obj and allocate pool map
        /// </summary>
        private void Init() 
        {
            if (inactivePool != null)
            {
                Debug.LogError ("This pool already initialized");
                return;
            }
            GameObject go = gameObject.CreateChild ("inactive");
            inactivePool = go.transform;
            go.SetActiveEx (false);
            poolMap = new Dictionary<string, GOPoolElement>();
        }
        
        /// <summary>
        /// Call SetObjectPrefab(key, prefab) that key to prefab.name
        /// </summary>
        /// <param name="prefabs">Prefabs.</param>
        public void SetObjectPrefabs(GameObject[] prefabs)
        {
            prefabs.ForEachEx (p => SetObjectPrefab (p.name, p));
        }
        
        /// <summary>
        /// Regist prefab to pool for key
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="prefab">Prefab.</param>
        public void SetObjectPrefab(string key, GameObject prefab) 
        {
            if (prefab == null) 
            {
                Debug.LogError ("Don't try add null object to pool");
                return;
            }
            if (poolMap.ContainsKey (key)) 
            {
                Debug.LogErrorFormat ("Name {0} prefab already added to pool", key);
                return;
            }
            poolMap.Add (key, new GOPoolElement (prefab));
        }
        
        /// <summary>
        /// Get obj for key and move to below of parent in hirerachy
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="parent">Parent.</param>
        public GameObject Get(string key, Transform parent) 
        {
            GOPoolElement elem = poolMap.Get (key);
            if (elem == null) 
            {
                Debug.LogErrorFormat ("Name {0} prefab doesn't added to pool", key);
                return null;
            }
            Queue<GameObject> queue = elem.pool;
            GameObject obj = queue.Empty() ? null : queue.Dequeue();
            if (obj == null) 
            {
                obj = elem.prefab.InstantiateEx ();
            }
            obj.transform.SetParent(parent);
            return obj;
        }
        
        /// <summary>
        /// Call Release(key, obj) that key to obj.name
        /// </summary>
        /// <param name="obj">Object.</param>
        public void Release(GameObject obj) 
        {
            Release (obj.name, obj);
        }
        
        /// <summary>
        /// Move obj to pool in hirerachy and enqueue for key
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="obj">Object.</param>
        public void Release(string key, GameObject obj) 
        {
            GOPoolElement elem = poolMap.Get(key);
            if (elem == null)
            {
                Debug.LogErrorFormat ("Name {0} prefab doesn't registed", key);
                return;
            }
            obj.transform.SetParent (inactivePool);
            elem.pool.Enqueue(obj);
        }
        
        /// <summary>
        /// Clear registed prefab instance.
        /// </summary>
        public void Clear()
        {
            foreach (GOPoolElement elem in poolMap.Values)
            {
                elem.pool.ForEach (go => go.DestroyEx ());
                elem.pool.Clear ();
            }
            poolMap.Clear ();
            inactivePool.DestroyEx ();
        }
    }
    
}