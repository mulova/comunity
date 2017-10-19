using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Threading;
using commons;
using UnityEditor.SceneManagement;

namespace core
{
    public abstract class SearchTab<T> : EditorTab where T:Object
    {
        private Object root;
        private StringBuilder str = new StringBuilder();
        
        public SearchTab(object id, TabbedEditorWindow window) : base(id, window)
        {
        }
        
        private string GetString(string name, string parenthesis)
        {
            str.Length = 0;
            str.Append(name);
            str.Append(" (").Append(parenthesis).Append(")");
            return str.ToString();
        }

        // TODOM use IEnumerator to save memory
        protected List<Object> SearchAssets(Type type, params FileType[] fileTypes)
        {
            List<Object> found = new List<Object>();
            List<Object> roots = new List<Object>();
            if (root != null)
            {
                roots.Add(root);
            } else
            {
                roots.AddRange(Selection.objects);
            }
            foreach (Object rootObj in roots)
            {
                foreach (FileType t in fileTypes)
                {
                    found.AddRange(EditorAssetUtil.SearchAssetObjects(rootObj, type, t));
                }
            }
            return found;
        }
        
        public override void OnHeaderGUI()
        {
            EditorGUIUtil.ObjectField<Object>("Root", ref root, true);
            OnHeaderGUI(found);
            EditorGUIUtil.DrawSeparator();
        }

        public abstract void OnHeaderGUI(List<T> found);

        private List<T> found = new List<T>();

        public sealed override void OnInspectorGUI()
        {
            if (progress < 1f)
            {
                SetInfo("Progress: {P0}", progress);
            } else
            {
                if (found.Count > 0)
                {
                    OnInspectorGUI(found);
                }
            }
        }

        protected abstract void OnInspectorGUI(List<T> found);
        
        public void SetFound(List<T> found)
        {
            this.found = found;
        }
        
        private float progress = 1;

        public void SetProgress(float progress)
        {
            this.progress = progress;
        }
        
        protected void Search()
        {
            SetFound(SearchResource(root));
        }
        
        protected abstract List<T> SearchResource(Object root);
        
        public override void OnFooterGUI()
        {
            OnFooterGUI(found);
            if (found != null&&found.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select GameObject"))
                {
                    List<GameObject> list = new List<GameObject>();
                    foreach (T t in found)
                    {
                        if (t is Component)
                        {
                            list.Add((t as Component).gameObject);
                        } else if (t is GameObject)
                        {
                            list.Add(t as GameObject);
                        }
                    }
                    Selection.objects = list.ToArray();
                }
                if (GUILayout.Button("Select"))
                {
                    Selection.objects = found.ToArray();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public abstract void OnFooterGUI(List<T> found);
        
        public override void OnEnable()
        {
        }
        
        public override void OnDisable()
        {
        }

        public object Id
        {
            get
            {
                return id;
            }
        }
    }
}
