//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using UnityEngine.SceneManagement;
using commons;
using System.Ex;

namespace comunity
{
    /// <summary>
    /// Track the scene changes
    /// </summary>
    public static class SceneStack
    {
        private static Stack<string> sceneStack = new Stack<string>();
        //public static readonly Loggerx log = LogManager.GetLogger(typeof(SceneStack));
        private static string _current = SceneManager.GetActiveScene().name;
        private static HashSet<string> exclude = new HashSet<string>();
        private static HashSet<string> visited = new HashSet<string>();
        
        private static string[] values;
        
        public static int Count
        {
            get
            {
                return sceneStack.Count;
            }
        }
        
        public static void AddExclude(object ex)
        {
            exclude.Add(ex.ToText());
        }

        public static bool HasVisited(string scene)
        {
            return visited.Contains(scene);
        }
        
        public static bool Push(string scene)
        {
            if (scene == current)
            {
                return false;
            } else
            {
                visited.Add(scene);
                if (!exclude.Contains(current))
                {
                    sceneStack.Push(current);
                }
                _current = scene;
                return true;
            }
        }
        
        public static string Pop()
        {
            if (sceneStack.Count > 0)
            {
                _current = sceneStack.Pop();
                return current;
            } else
            {
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Pop scenes except current scene
        /// </summary>
        /// <returns>pop scene count</returns>
        /// <param name="scenes">Scenes.</param>
        public static int PopScenesExceptCurrent(params string[] scenes)
        {
            HashSet<string> sceneSet = new HashSet<string>(scenes);
            foreach (string s in scenes)
            {
                sceneSet.Add(s);
            }
            sceneStack.Pop();
            int count = 0;
            while (sceneSet.Contains(sceneStack.Peek()))
            {
                sceneStack.Pop();
                count++;
            }
            return count;
        }
        
        public static void ClearTrace()
        {
            sceneStack.Clear();
        }
        
        public static string previous
        {
            get
            {
                if (sceneStack.Count > 0)
                {
                    return sceneStack.Peek();
                }
                return string.Empty;
            }
        }
        
        public static string current
        {
            get
            {
                return _current;
            }
        }
        
        public static bool hasPrevious
        {   
            get
            {
                return sceneStack.Count > 0;
            }
        }
    }
}

