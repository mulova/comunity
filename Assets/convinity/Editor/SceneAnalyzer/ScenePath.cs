using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using commons;

namespace convinity
{
    public class ScenePath
    {
        public string scene;
        public readonly string[] paths;
        public readonly int[] siblingIndex;

        public ScenePath(Object o)
        {
            scene = SceneManager.GetActiveScene().path;

            Transform t = null;
            if (o is GameObject)
            {
                t = (o as GameObject).transform;
            } else if (o is Component)
            {
                t = (o as Component).transform;
            }
            if (t != null)
            {
                Stack<string> pathList = new Stack<string>();
                Stack<int> indexList = new Stack<int>();
                while (t != null)
                {
                    pathList.Push(t.name);
                    indexList.Push(t.GetSiblingIndex());
                    t = t.parent;
                }
                paths = pathList.ToArray();
                siblingIndex = indexList.ToArray();
            }
        }

        public Transform Find()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.path == scene)
            {
                GameObject[] roots = currentScene.GetRootGameObjects();
                Transform t = roots[siblingIndex[0]].transform;
                for (int i=1; i<siblingIndex.Length; ++i)
                {
                    t = t.GetChild(siblingIndex[i]);
                }
                return t;
            } else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return paths.Join("/");
        }
    }
}

