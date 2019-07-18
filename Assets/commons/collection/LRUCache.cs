using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.Generic.Ex;

namespace commons
{
    /// <summary>
    /// Cache using Least Recently Used policy.
    /// WeakReference is used
    /// </summary>
    public class LRUCache<T> : IEnumerable<KeyValuePair<object, CacheEntry<T>>>, IDisposable where T: class
    {
        /// <summary>
        /// The disposer. return true if object cleanup succeeds.
        /// </summary>
        public delegate bool Disposer(T kickedObj);
        
        public bool autoResize = true; // dispose overflow objects
        private Disposer disposer;
        private LinkedList<object> lruKey = new LinkedList<object>();
        private Dictionary<object, CacheEntry<T>> pool = new Dictionary<object, CacheEntry<T>>();
        private long capacity;
        private long size;
        private bool strong;
        public static readonly Loggerx log = LogManager.GetLogger(typeof(LRUCache<T>));
        
        public T this[object key]
        {
            get
            {
                return Get(key);
            }
            
            set
            {
                Put(key, value);
            }
        }
        
        public LRUCache(bool strong) : this(0, strong)
        {
        }
        
        public LRUCache(long capacity, bool strong)
        {
            SetCapacity(capacity);
            this.strong = strong;
        }
        
        public void SetCapacity(long capacity)
        {
            this.capacity = capacity;
        }
        
        public long GetCapacity()
        {
            return this.capacity;
        }
        
        public long GetSize()
        {
            return this.size;
        }
        
        public T Get(object key, bool remove = false)
        {
            CacheEntry<T> entry = pool.Get(key);
            if (entry != null)
            {
                if (remove)
                {
                    lruKey.Remove(key);
                    pool.Remove(key);
                    this.size -= entry.size;
                } else
                {
                    Use(key);
                }
                if (entry.Obj == null)
                {
                    Remove(key);
                }
                return (T)entry.Obj;
            } else
            {
                pool.Remove(key);
                return default(T);
            }
        }
        
        /// <summary>
        /// move to head. set as the most recently used
        /// </summary>
        /// <param name="key">Key.</param>
        public void Use(object key)
        {
            lruKey.Remove(key);
            lruKey.AddFirst(key);
        }
        
        public void Remove(object key)
        {
            CacheEntry<T> kicked = pool.Get(key);
            if (kicked != null)
            {
                if (disposer == null||kicked.Obj == null||disposer(kicked.Obj))
                {
                    lruKey.Remove(key);
                    pool.Remove(key);
                    this.size -= kicked.size;
                }
            } else
            {
                log.Warn("No cache key for {0}", key);
            }
        }
        
        public void Put(object key, T val)
        {
            Put(key, val, 1);
        }
        
        /**
        * @return kicked-off object if capacity is full or null
        */
        public void Put(object key, T val, long valSize)
        {
            if (lruKey.Contains(key))
            {
                Remove(key);
                log.Warn("Duplicate entry {0}", key);
            }
            lruKey.AddFirst(key);
            pool[key] = new CacheEntry<T>(val, valSize, strong);
            this.size += valSize;
            if (autoResize)
            {
                Resize();
            }
        }
        
        public void SetDisposer(Disposer disposer)
        {
            this.disposer = disposer;
        }
        
        /// <summary>
        /// Dispose overflowed objects
        /// </summary>
        public void Resize()
        {
            if (capacity > 0&&this.size > capacity)
            {
                LinkedListNode<object> keyNode = lruKey.Last;
                List<object> removeKeys = new List<object>();
                long s = size;
                while (keyNode != lruKey.First && s > capacity)
                {
                    object oldKey = keyNode.Value;
                    removeKeys.Add(oldKey);
                    CacheEntry<T> kicked = pool.Get(oldKey);
                    s -= kicked.size;
                    keyNode = keyNode.Previous;
                }
                foreach (object k in removeKeys)
                {
                    Remove(k);
                }
            }
        }
        
        public void Dispose()
        {
            if (disposer != null)
            {
                foreach (KeyValuePair<object, CacheEntry<T>> p in pool)
                {
                    T obj = p.Value.Obj;
                    if (obj != null)
                    {
                        disposer(obj);
                    }
                }
            }
            size = 0;
            pool.Clear();
            lruKey.Clear();
        }
        
        IEnumerator<KeyValuePair<object, CacheEntry<T>>> IEnumerable<KeyValuePair<object, CacheEntry<T>>>.GetEnumerator()
        {
            return pool.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return pool.GetEnumerator();
        }
    }
    
    public class CacheEntry<T> where T: class
    {
        private WeakReference<T> obj;
        public readonly long size;
        
        public CacheEntry(T obj, long size, bool strong)
        {
            this.obj = WeakReference<T>.Create(obj, strong);
            this.size = size;
        }
        
        public T Obj
        {
            get
            {
                return obj.Target;
            }
        }
    }
}
