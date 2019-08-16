using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using mulova.commons;
using Rotorz.Games.Collections;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace comunity
{
    public class ListDrawer<T> : IReorderableListAdaptor where T : class
    {
        protected List<T> list;

        public bool changed;
        public float itemHeight = 16;
        public event Action<int, T> onChange;
        public event Action<int, T> onInsert;
        public event Action<int, T> onRemove;
        public event Action<int, T> onDuplicate;
        public event Action<int, int, T> onMove;
        public bool allowDuplicate = false;
        public bool addSelected = true;
        public bool horizontal;
        public Func<T> createDefaultValue = () => default(T);
        public Func<Object, T> createItem;
        protected IItemDrawer<T> itemDrawer;
        public ReorderableListFlags flags;

        public int Count
        {
            get
            {
                return list.GetCount();
            }
        }

        public T this[int i]
        {
            get
            {
                return list[i];
            }
        }

        public ListDrawer(List<T> list) : this(list, new ItemDrawer<T>()) { }

        public ListDrawer(List<T> list, IItemDrawer<T> itemDrawer)
        {
            this.list = list;
            this.itemDrawer = itemDrawer; // just for reference count
            this.createItem = CreateItem;
        }

        public virtual bool CanDrag(int index)
        {
            return true;
        }

        public virtual bool CanRemove(int index)
        {
            return true;
        }

        public void Add()
        {
            var objs = GetSelected();
            if (objs != null && addSelected)
            {
                foreach (var o in objs)
                {
                    if (allowDuplicate || !list.Contains(o))
                    {
                        changed = true;
                        list.Add(o);
                        onInsert?.Invoke(list.Count - 1, o);
                    }
                }
            }
            else
            {
                Insert(Count);
            }
        }

        public void Insert(int index)
        {
            var objs = GetSelected();
            if (objs != null && addSelected)
            {
                for (int i = 0; i < objs.Count; ++i)
                {
                    if (allowDuplicate || !list.Contains(objs[i]))
                    {
                        list.Insert(index + i, objs[i]);
                        onInsert?.Invoke(index + i, objs[i]);
                    }
                }
            }
            else
            {
                var defaultVale = createDefaultValue();
                list.Insert(index, defaultVale);
                onInsert(index, defaultVale);
            }
            changed = true;
        }

        public void Duplicate(int index)
        {
            T obj = list[index];
            list.Insert(index, obj);
            onDuplicate?.Invoke(index, obj);
            changed = true;
        }

        public void Remove(int index)
        {
            T obj = list[index];
            list.RemoveAt(index);
            onRemove?.Invoke(index, obj);
            changed = true;
        }

        public void Move(int srcIndex, int dstIndex)
        {
            list.Insert(dstIndex, list[srcIndex]);
            int si2 = srcIndex > dstIndex ? srcIndex + 1 : srcIndex;
            list.RemoveAt(si2);
            onMove?.Invoke(srcIndex, dstIndex, list[srcIndex]);
            changed = true;
        }

        public void Clear()
        {
            list.Clear();
            changed = true;
        }

        public void BeginGUI()
        {
        }

        public void EndGUI()
        {
        }

        public void DrawItemBackground(Rect position, int index)
        {
            itemDrawer.DrawItemBackground(position, index, list[index]);
        }

        public void DrawItem(Rect bound, int index)
        {
            if (bound.height <= 0)
            {
                return;
            }
            T obj = list[index];
            T changedObj = default(T);

            if (itemDrawer.DrawItem(bound, index, obj, out changedObj))
            {
                changed = true;
                list[index] = changedObj;
                onChange?.Invoke(index, changedObj);
            }
        }

        public float GetItemHeight(int index)
        {
            if (match != null && !match(list[index]))
            {
                return -4;
            }
            return itemDrawer.GetItemHeight(index, list[index]);
        }

        private Predicate<T> match;
        public void Filter(Predicate<T> match)
        {
            this.match = match;
        }

        public bool Draw()
        {
            return Draw(flags);
        }

        public bool Draw(ReorderableListFlags flags)
        {
            ReorderableListGUI.ListField(this, flags);
            bool ret = changed;
            changed = false;
            return ret;
        }

        private T CreateItem(Object o)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                var go = o as GameObject;
                T c = null;
                if (go != null)
                {
                    c = go.GetComponent<T>();
                }
                if (c != null)
                {
                    return c as T;
                }
                else if (typeof(T).IsAssignableFrom(o.GetType()))
                {
                    return o as T;
                }
                else
                {
                    return createDefaultValue();
                }
            }
            else
            {
                return o as T;
            }
        }

        protected virtual IList<T> GetSelected()
        {
            List<T> l = new List<T>();
            Object[] selObjs = Selection.objects;
            if (selObjs != null && selObjs.Length > 1)
            {
                if (Selection.objects.IsNotEmpty())
                {
                    foreach (Object o in Selection.objects)
                    {
                        l.Add(createItem(o));
                    }
                }
                else
                {
                    foreach (var guid in Selection.assetGUIDs)
                    {
                        var sel = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
                        if (sel != null)
                        {
                            l.Add(createItem(sel));
                        }
                    }
                }
            }

            if (l.IsEmpty())
            {
                l.Add(createDefaultValue());
            }
            return l;
        }

        public virtual void Add(T item)
        {
            list.Add(item);
            changed = true;
        }
    }
}
