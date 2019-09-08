using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace mulova.comunity
{
    [System.Serializable]
    public class ObjRefList : List<ObjRef>
    {
        public void Add(Object obj)
        {
            Add(new ObjRef(obj));
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
            Insert(index, new ObjRef(obj));
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

        public static ObjRefList Load(string path)
        {
            if (File.Exists(path))
            {
                BinarySerializer reader = new BinarySerializer(path, FileAccess.Read);
                ObjRefList list = reader.Deserialize<ObjRefList>();
                reader.Close();
                if (list == null)
                {
                    list = new ObjRefList();
                }
                return list;
            } else
            {
                return new ObjRefList();
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
