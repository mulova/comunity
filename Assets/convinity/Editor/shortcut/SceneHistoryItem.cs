using UnityEngine;
using Object = UnityEngine.Object;
using System;
using comunity;
using System.Collections.Generic;
using commons;
using UnityEditor.SceneManagement;
using UnityEditor;

namespace convinity
{
    [Serializable]
    public class SceneHistoryItem
    {
        public List<UnityObjId> list;
		public SceneCamProperty camProperty;

        public SceneView sceneView {
            get {
				if (SceneView.currentDrawingSceneView != null)
				{
					return SceneView.currentDrawingSceneView;
				}
                if (SceneView.lastActiveSceneView != null)
                {
                    return SceneView.lastActiveSceneView;
                }
                if (SceneView.sceneViews.Count > 0)
                {
                    return (SceneView)SceneView.sceneViews[0];
                }
                return null;
            }
        }

        public SceneHistoryItem(Object o)
        {
            list = new List<UnityObjId>();
            list.Add(new UnityObjId(o));
        }

        public void SaveCam()
        {
			if (sceneView == null)
			{
				return;
			}
			if (camProperty == null)
			{
				camProperty = new SceneCamProperty();
			}
			camProperty.Collect(sceneView);
        }

		public void ApplyCam()
		{
			if (sceneView == null)
			{
				return;
			}
			if (camProperty != null)
			{
				camProperty.Apply(sceneView);
			}
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

        public void RemoveScene(Object sceneObj)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].reference == sceneObj)
                {
                    list.RemoveAt(i);
                    break;
                }
            }
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

		public override string ToString()
		{
			return list[0].path;
		}
    }

}
