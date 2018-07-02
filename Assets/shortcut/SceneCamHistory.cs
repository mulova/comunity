using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

namespace convinity
{
    [System.Serializable]
    public class SceneCamHistory
    {
        [SerializeField] private List<SceneCamProperty> _items = new List<SceneCamProperty>();
        [SerializeField] internal int maxSize = 100;

        public List<SceneCamProperty> items
        {
            get
            {
                return _items;
            }
        }


        public int Count
        {
            get 
            {
                return items.Count;
            }
        }

        public SceneCamProperty this[int i]
        {
            get
            {
                return items[i];
            }
        }

        public void Add(SceneCamProperty item)
        {
            items.Add(item);
            Resize();
        }

        public void RemoveAt(int i)
        {
            items.RemoveAt(i);
        }

        public void Insert(int i, SceneCamProperty item)
        {
            items.Insert(i, item);
            Resize();
        }

        public void Clear()
        {
            items.Clear();
        }

        private void Resize()
        {
            int i = Count-1;
            while (Count > maxSize && i >= 0)
            {
                items.RemoveAt(i);
            }
        }

        public void Add()
        {
            var cam = new SceneCamProperty();
            cam.Collect();
            Add(cam);
        }

        public static SceneCamHistory Load(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    if (text != null)
                    {
                        return JsonUtility.FromJson<SceneCamHistory>(text);
                    }
                }
            } catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            return new SceneCamHistory();
        }

        public void Save(string path)
        {
            try
            {
                var json = JsonUtility.ToJson(this);
                File.WriteAllText(path, json);
            } catch(Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
    }
}

