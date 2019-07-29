using Object = UnityEngine.Object;
using System;
using comunity;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System.IO;
using System.Ex;
using System.Collections.Generic.Ex;
using System.Text.Ex;

namespace scenehistorian
{
    [Serializable]
    public class SceneHistoryItem : IComparable<SceneHistoryItem>
    {
        public List<UnityObjId> list;
		public SceneCamProperty camProperty;
        public bool starred;
		public int activeIndex;

		public UnityObjId activeScene
		{
			get
			{
				return list[activeIndex];
			}
		}

        public string name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(ToString());
            }
        }

        public SceneHistoryItem(Object o)
        {
            list = new List<UnityObjId>();
            list.Add(new UnityObjId(o));
        }

        public void SaveCam()
        {
			if (EditorUtil.sceneView == null)
			{
				return;
			}
			if (camProperty == null)
			{
				camProperty = new SceneCamProperty();
			}
			camProperty.Collect();
        }

		public void ApplyCam()
		{
            if (EditorUtil.sceneView == null)
			{
				return;
			}
			if (camProperty != null)
			{
				camProperty.Apply();
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
                var s = EditorSceneManager.OpenScene(list[i].path, OpenSceneMode.Additive);
				if (activeIndex == i)
				{
					EditorSceneManager.SetActiveScene(s);
				}
            }
        }

		public override string ToString()
		{
			string path = list[0].path;
			if (path != null) {
                return path;
			} else {
				return string.Empty;
			}
		}

        public int CompareTo(SceneHistoryItem that)
        {
            if (this.starred^that.starred)
            {
                return this.starred? -1 : 1;
            } else
            {
                var str1 = this.name;
                var str2 = that.name;
                if (str1 != null)
                {
                    if (str2 != null)
                    {
                        return str1.CompareTo(str2);
                    } else
                    {
                        return -1;
                    }
                } else
                {
                    if (str2 != null)
                    {
                        return 1;
                    } else
                    {
                        return 0;
                    }
                }
            }
        }

		public void SetActiveScene(string path)
		{
			int index = list.FindIndex(id => id.path.EqualsIgnoreSeparator(path));
			if (index >= 0)
			{
				activeIndex = index;
			}
		}
    }

}
