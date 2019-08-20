using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Threading;
using mulova.commons;
using UnityEditor.SceneManagement;
using comunity;

namespace convinity
{
    public abstract class SearchTab<T> : EditorTab
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
        protected IEnumerable<Object> SearchAssets(Type type, params FileType[] fileTypes)
        {
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
                    foreach (var o in EditorAssetUtil.SearchAssetObjects(rootObj, type, t))
                    {
                        yield return o;
                    }
                }
            }
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

		protected IEnumerable<Object> roots
		{
			get
			{
				if (root != null)
				{
					return new [] { root };
				} else
				{
					return EditorUtil.GetSceneRoots().ConvertAll<Object>(o=>o);
				}
			}
		}
        
		protected void Search()
        {
			SetFound(SearchResource());
        }
        
        protected abstract List<T> SearchResource();
        
        public override void OnFooterGUI()
        {
            OnFooterGUI(found);
            if (found != null&&found.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select GameObject"))
                {
                    SelectGameObjects(found);
                }
                if (GUILayout.Button("Select"))
                {
                    SelectObjects(found);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        protected virtual void SelectGameObjects(List<T> found)
        {
            List<Object> list = new List<Object>();
            foreach (T t in found)
            {
                if (t is Component)
                {
                    list.Add((t as Component).gameObject);
                } else
                {
                    var o = t as GameObject;
                    if (o != null)
                    {
                        list.Add(o);
                    }
                }
            }
            Selection.objects = list.ToArray();
        }

        protected virtual void SelectObjects(List<T> found)
        {
            List<Object> list = new List<Object>();
            foreach (var t in found)
            {
                Object o = t as Object;
                if (o != null)
                {
                    list.Add(o);
                }
            }
            Selection.objects = list.ToArray();
        }

        public abstract void OnFooterGUI(List<T> found);
        
        public override void OnEnable()
        {
        }
        
        public override void OnDisable()
        {
        }
    }
}
