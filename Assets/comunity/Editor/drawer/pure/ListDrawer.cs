using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;
using commons;
using Rotorz.Games.Collections;

namespace comunity
{
    public class ListDrawer<T> : IReorderableListAdaptor where T:class
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
        public bool allowSceneObject = true;
        public bool addSelected = true;
        public bool allowSelection = true;
        public bool horizontal;
        public Object undoTarget;
        public Func<T> createDefaultValue = ()=> default(T);
        public Func<Object, T> createItem;
        protected IItemDrawer<T> itemDrawer;
        public ReorderableListFlags flags;

        private int[] indexer; // used for filtering
        public int Count { get; private set; }

        public T this[int i]
        {
            get
            {
                return list[i];
            }
        }

        public ListDrawer(IList<T> list) : this(list, new ItemDrawer<T>()) { }

        public ListDrawer(IList<T> list, IItemDrawer<T> itemDrawer)
        {
            this.list = new List<T>(list);
            this.Count = list.Count;
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
                        list.Add(o);
                        onInsert?.Invoke(list.Count - 1, o);
                    }
                }
            } else
            {
                Insert(Count);
            }
            Refresh();
        }

        private void Refresh()
        {
            Filter(match);
        }

        public void Insert(int index)
        {
            int i0 = GetActualIndex(index);
            var objs = GetSelected();
            if (objs != null && addSelected)
            {
                for (int i=0; i<objs.Count; ++i)
                {
                    if (allowDuplicate || !list.Contains(objs[i]))
                    {
                        list.Insert(i0+i, objs[i]);
                        if (onInsert != null)
                        {
                            onInsert(i0+i, objs[i]);
                        }
                    }
                }
            } else
            {
                var defaultVale = createDefaultValue();
                list.Insert(i0, defaultVale);
                onInsert(i0, defaultVale);
            }
            changed = true;
        }

        public void Duplicate(int index)
        {
            int i = GetActualIndex(index);
            T obj = list[GetActualIndex(index)];
            list.Insert(i, obj);
            onDuplicate?.Invoke(i, obj);
            changed = true;
        }

        public void Remove(int index)
        {
            int i = GetActualIndex(index);
            T obj = list[i];
            list.RemoveAt(i);
            onRemove?.Invoke(i, obj);
            changed = true;
            Refresh();
        }

        public void Move(int sourceIndex, int destIndex)
        {
            int si = GetActualIndex(sourceIndex);
            int di = GetActualIndex(destIndex);
            list.Insert(di, list[si]);
            int si2 = si > di? si+1: si;
            list.RemoveAt(si2);
            onMove?.Invoke(si, di, list[si]);
            changed = true;
            Refresh();
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

        public void DrawItemBackground(UnityEngine.Rect position, int index)
        {
//            int i = GetActualIndex(index);
        }

        public void DrawItem(UnityEngine.Rect position, int index)
        {
            int i = GetActualIndex(index);
            T obj = list[i];
            T changedObj = default(T);

            if (itemDrawer.DrawItem(position, index, obj, out changedObj))
            {
                changed = true;
                list[i] = changedObj;
                if (onChange != null)
                {
                    onChange(index, changedObj);
                }
            }
        }

        public float GetItemHeight(int index)
        {
//            int i = GetActualIndex(index);
            return itemHeight;
        }

        private int GetActualIndex(int index)
        {
            if (indexer != null && index < indexer.Length)
            {
                return indexer[index];
            } else
            {
                return index;
            }
        }

        private Predicate<T> match;
        public void Filter(Predicate<T> match)
        {
            this.match = match;
            if (match != null)
            {
                indexer = new int[list.Count];
                int n = 0;
                for (int i=0; i<list.Count; ++i)
                {
                    if (match(list[i]))
                    {
                        indexer[n] = i;
                        n++;
                    }
                }
                Count = n;
            } else
            {
                indexer = null;
                Count = list.Count;
            }
        }

        public bool Draw()
        {
            return Draw(flags);
        }

        public bool Draw(ReorderableListFlags flags)
        {
            if (undoTarget != null)
            {
                Undo.RecordObject(undoTarget, undoTarget.name);
            }
            ReorderableListGUI.ListField(this, flags);
            if (undoTarget != null && !changed)
            {
                Undo.ClearUndo(undoTarget);
            }
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
                } else if (typeof(T).IsAssignableFrom(o.GetType()))
                {
                    return o as T;
                } else
                {
                    return default(T);
                }
            } else
            {
                return o as T;
            }
        }

        protected virtual IList<T> GetSelected()
        {
            List<T> list = new List<T>();
            Object[] selObjs = Selection.objects;
            if (allowSelection || (selObjs != null && selObjs.Length > 1))
            {
                if (allowSceneObject && Selection.objects.IsNotEmpty())
                {
                    foreach (Object o in Selection.objects)
                    {
                        list.Add(createItem(o));
                    }
                } else
                {
                    foreach (var guid in Selection.assetGUIDs)
                    {
                        var sel = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
                        if (sel != null)
                        {
                            list.Add(createItem(sel));
                        }
                    }
                }
            }

            if (list.IsEmpty())
            {
                list.Add(default(T));
            }
            return list;
        }

        public virtual void Add(T item)
        {
            list.Add(item);
            Count = list.Count;
            changed = true;
        }
    }
}
