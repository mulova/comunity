using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace comunity
{
    public class Lifecycle : MonoBehaviour
    {
        public enum LifeTime
        {
            Normal,
            Editor,
            Build,
            Debug,
            Release,
            Forever,
            NotPlaying,
        }
        
        public LifeTime lifeTime = LifeTime.Normal;
        
        public bool isValid
        {
            get
            {
                if (lifeTime == LifeTime.Editor)
                {
                    return Platform.isEditor;
                } else if (lifeTime == LifeTime.Build)
                {
                    return Platform.isBuild;
                } else if (lifeTime == LifeTime.Debug)
                {
                    return Platform.isDebug;
                } else if (lifeTime == LifeTime.Release)
                {
                    return Platform.isReleaseBuild;
                } else if (lifeTime == LifeTime.Forever)
                {
                    DontDestroyOnLoad(gameObject);
                    return true;
                } else if (lifeTime == LifeTime.NotPlaying)
                {
                    if (Application.isEditor)
                    {
                        if (!gameObject.CompareTag("EditorOnly"))
                        {
                            Debug.LogError(name+" is not EditorOnly");
                        }
                    }
                    return !Application.isPlaying;
                } else
                {
                    return false;
                }
            }
        }
        
        void Awake()
        {
            if (!isValid)
            {
                Destroy(gameObject);
            } else
            {
                AwakeImpl();
            }
        }
        
        protected virtual void AwakeImpl() {}
        
        IEnumerator DestroyAfterPostRender()
        {
            yield return null;
            yield return null;
            Destroy(gameObject);
        }
    }
}
