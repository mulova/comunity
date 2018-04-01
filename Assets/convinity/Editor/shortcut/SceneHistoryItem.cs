using UnityEngine;
using Object = UnityEngine.Object;
using System;
using comunity;
using System.Collections.Generic;
using commons;
using UnityEditor.SceneManagement;

namespace convinity
{
    [Serializable]
    public class SceneHistoryItem
    {
        public List<UnityObjId> list;

        public SceneHistoryItem(Object o)
        {
            list = new List<UnityObjId>();
            list.Add(new UnityObjId(o));
        }

        public UnityObjId first
        {
            get
            {
                return list.Get(0);
            }
        }

        public Object firstRef
        {
            get
            {
                var o = first;
                if (o != null)
                {
                    return o.reference;
                } else
                {
                    return null;
                }
            }
        }

        public void AddScene(Object sceneObj)
        {
            list.Add(new UnityObjId(sceneObj));
        }

        public bool Contains(Object sceneObj)
        {
            foreach (var o in list)
            {
                if (o.reference == sceneObj)
                {
                    return true;
                }
            }
            return false;
        }

        public void LoadAdditiveScenes()
        {
            for (int i = 1; i < list.Count; ++i)
            {
                EditorSceneManager.OpenScene(list[i].path, OpenSceneMode.Additive);
            }
        }
    }
}
