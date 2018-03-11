using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;
using commons;
using Rotorz.ReorderableList;

namespace comunity
{
    public delegate string ConvToString(object o);
    public delegate Object ConvToObject(object o);

    public class ListDrawer<T> : IReorderableListAdaptor where T:class
    {
        public delegate IItemDrawer<T> GetItemDrawer(T obj);

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
        public string title;
        public Object undoTarget;
        public Func<T> createDefaultValue = ()=> default(T);
        private GetItemDrawer getItemDrawer;

        private int[] indexer; // used for filtering
        private int count;
        #pragma warning disable 0414
        private IItemDrawer<T> itemDrawer;
        #pragma warning restore 0414

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
            this.count = list.Count;
            this.itemDrawer = itemDrawer; // just for reference count
            this.getItemDrawer = t => itemDrawer;
        }

        public ListDrawer(List<T> list, GetItemDrawer getItemDrawer)
        {
            this.list = list;
            this.count = list.Count;
            this.getItemDrawer = getItemDrawer;
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
                        if (onInsert != null)
                        {
                            onInsert(list.Count-1, o);
                        }
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
            if (onDuplicate != null)
            {
                onDuplicate(i, obj);
            }
            changed = true;
        }

        public void Remove(int index)
        {
            int i = GetActualIndex(index);
            T obj = list[i];
            list.RemoveAt(i);
            if (onRemove != null)
            {
                onRemove(i, obj);
            }
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
            if (onMove != null)
            {
                onMove(si, di, list[si]);
            }
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

            if (getItemDrawer(obj).DrawItem(position, index, obj, out changedObj))
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

        public int Count
        {
            get
            {
                return count;
            }
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
                count = n;
            } else
            {
                indexer = null;
                count = list.Count;
            }
        }

        public bool Draw()
        {
            return Draw((ReorderableListFlags)0);
        }

        public bool Draw(ReorderableListFlags flags)
        {
            if (title != null)
            {
                GUILayout.Label(title);
            }
            if (undoTarget != null)
            {
                string id = title!=null? title: undoTarget.name;
                Undo.RecordObject(undoTarget, id);
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

        protected virtual IList<T> GetSelected()
        {
            List<T> list = new List<T>();
            Object[] selObjs = Selection.objects;
            if (allowSelection || (selObjs != null && selObjs.Length > 1))
            {
                if (allowSceneObject && Selection.objects.IsNotEmpty())
                {
                    foreach (var o in Selection.objects)
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
                                list.Add(c as T);
                            } else if (typeof(T).IsAssignableFrom(o.GetType()))
                            {
                                list.Add(o as T);
                            }
                        } else
                        {
                            list.Add(o as T);
                        }
                    }
                } else
                {
                    foreach (var guid in Selection.assetGUIDs)
                    {
                        var sel = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T));
                        if (sel != null)
                        {
                            list.Add(sel as T);
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
            count = list.Count;
            changed = true;
        }
    }
}

