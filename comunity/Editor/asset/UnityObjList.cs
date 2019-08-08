﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace comunity
{
    [System.Serializable]
    public class UnityObjList : List<UnityObjId>
    {
        public void Add(Object obj)
        {
            Add(new UnityObjId(obj));
        }

        public int IndexOf(Object obj)
        {
            for (int i=0; i<Count; ++i)
            {
                if (this[i].reference == obj)
                {
                        return i;
                }
            }
            return -1;
        }

        public void Insert(int index, Object obj)
        {
            Insert(index, new UnityObjId(obj));
        }

        public void Remove(Object obj)
        {
            int index = FindIndex(o => o.reference == obj);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        public void Remove(string guid)
        {
            int index = FindIndex(o => o.id == guid);
            if (index >= 0)
            {
                RemoveAt(index);
            }
        }

        public bool Contains(Object obj)
        {
            return IndexOf(obj) >= 0;
        }

        public static UnityObjList Load(string path)
        {
            if (File.Exists(path))
            {
                BinarySerializer reader = new BinarySerializer(path, FileAccess.Read);
                UnityObjList list = reader.Deserialize<UnityObjList>();
                reader.Close();
                if (list == null)
                {
                    list = new UnityObjList();
                }
                return list;
            } else
            {
                return new UnityObjList();
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