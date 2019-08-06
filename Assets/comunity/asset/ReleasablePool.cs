using UnityEngine;
using System.Collections.Generic;
using mulova.commons;

namespace comunity
{
    public class ReleasablePool
    {
        private static readonly List<IReleasablePool> pools = new List<IReleasablePool>();
        private static IReleasablePool def;
        private List<IReleasable> releasables;
        public static readonly Loggerx log = LogManager.GetLogger(typeof(Loggerx));
        
        public static void Register(IReleasablePool p) {
            Assert.IsFalse(pools.Contains(p));
            pools.Add(p);
        }
        
        public static void Deregister(IReleasablePool pool) {
            pools.Remove(pool);
        }
        
        public static void SetDefault(IReleasablePool pool) {
            def = pool;
        }
        
        public static void FindAndAdd(IReleasable r)
        {
            foreach (var p in pools) {
                if (r.transform.IsChildOf(p.transform)) {
                    p.Add(r);
                    return;
                }
            }
            def.Add(r);
        }
        
        public static void FindAndRemove(IReleasable r)
        {
            foreach (var p in pools) {
                p.Remove(r);
            }
            if (def != null) {
                def.Remove(r);
            }
        }
        
        public void Add(IReleasable r)
        {
            if (releasables == null)
            {
                releasables = new List<IReleasable>();
            }
            releasables.Add(r);
        }
        
        public void Remove(IReleasable r) {
            if (releasables != null) {
                releasables.Remove(r);
            }
        }
        
        public void Release()
        {
            if (releasables.IsNotEmpty())
            {
                foreach (IReleasable r in releasables)
                {
                    r.Release();
                }
            }
        }
    }
}
