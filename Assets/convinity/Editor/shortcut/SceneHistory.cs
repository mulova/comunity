using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using comunity;
using Object = UnityEngine.Object;
using System.IO;

namespace convinity
{
    [System.Serializable]
    public class SceneHistory : List<SceneHistoryItem>
    {
        public void Add(Object obj)
        {
            Add(new SceneHistoryItem(obj));
        }

        public int IndexOf(Object obj)
        {
            for (int i=0; i<Count; ++i)
            {
                if (this[i].list.Count > 0 &&  this[i].list[0].reference == obj)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, Object obj)
        {
            Insert(index, new SceneHistoryItem(obj));
        }

        public void Remove(Object obj)
        {
            int index = FindIndex(o => o.firstRef == obj);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        public void Remove(string guid)
        {
            int index = FindIndex(o => o.first != null && o.first.id == guid);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        public bool Contains(Object obj)
        {
            return IndexOf(obj) >= 0;
        }

        public static SceneHistory Load(string path)
        {
            if (File.Exists(path))
            {
                BinarySerializer reader = new BinarySerializer(path, FileAccess.Read);
                SceneHistory list = reader.Deserialize<SceneHistory>();
                reader.Close();
                if (list == null)
                {
                    list = new SceneHistory();
                }
                return list;
            } else
            {
                return new SceneHistory();
            }
        }

        public void Save(string path)
        {
            BinarySerializer writer = new BinarySerializer(path, FileAccess.Write);
            writer.Serialize(this);
            writer.Close();
        }
    }
}
