//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Collections;
using System.Text;

using System;
using UnityEngine.SceneManagement;

namespace core
{
    public static class TransformUtil
    {
/*
        public static IEnumerable<Transform> GetSceneRoots()
        {
            HashSet<Transform> set = new HashSet<Transform>();
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>())
            {
                if (t.root != null)
                {
                    set.Add(t.root);
                }
            }
            return set;
        }
*/      
        public static Transform GetSceneRoot(string name)
        {
            foreach (GameObject o in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (o.name == name)
                {
                    return o.transform;
                }
            }
            return null;
        }
        
        public static Transform Search(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            string[] names = path.Split(new char[] {'/'}, System.StringSplitOptions.RemoveEmptyEntries);
            Transform t = GetSceneRoot(names[0]);
            if (t == null)
            {
                return null;
            } else if (names.Length == 1)
            {
                return t;
            }
            string[] dst = new string[names.Length-1];
            Array.ConstrainedCopy(names, 1, dst, 0, names.Length-1);
            return t.Search(dst);
        }

        public static Dictionary<string, Transform> CreateTransformMap(this Transform root)
        {
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            Dictionary<string, Transform> map = new Dictionary<string, Transform>();
            foreach (Transform t in all)
            {
                map[t.name] = t;
            }
            return map;
        }
    }
}
