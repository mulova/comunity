using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.Collections;
using UnityEditor;
using System.Text.Ex;

namespace comunity
{
    public class ReorderList<T>
    {
        public readonly ReorderableList drawer;
        protected Object obj { get; set; }
        protected IList list;
        public bool changed { get; private set; }
        public delegate T CreateItemDelegate();
        public delegate bool DrawItemDelegate(Rect rect, int index, bool isActive, bool isFocused);
        public CreateItemDelegate createItem;

        private string _title;
        public string title
        {
            set
            {
                _title = value;
                drawer.headerHeight = _title.IsEmpty()? 0: 18;
            }
        }

        public DrawItemDelegate drawItem;

        public bool displayAdd
        {
            get {
                return drawer.displayAdd;
            }
            set {
                drawer.displayAdd = value;
            }
        }

        public bool displayRemove
        {
            get {
                return drawer.displayRemove;
            }
            set {
                drawer.displayRemove = value;
            }
        }

        public bool draggable
        {
            get {
                return drawer.draggable;
            }
            set {
                drawer.draggable = value;
            }
        }

        public T this[int i]
        {
            get {
                return (T)drawer.list[i];
            }
            set
            {
                if (i <= drawer.list.Count)
                {
                    drawer.list.Insert(i, value);
                } else
                {
                    drawer.list[i] = value;
                }
            }
        }

        public int count
        {
            get {
                return drawer.list.Count;
            }
        }

        public ReorderList(Object obj, IList list)
        {
            this.obj = obj;
            this.list = list;
            this.drawer = new ReorderableList(list, typeof(T), true, false, true, true);
            Init();
        }

        private void Init()
        {
            this.drawer.onAddCallback = _OnAdd;
            this.drawer.drawHeaderCallback = DrawHeader;
            this.drawer.drawElementCallback = _DrawItem;
            this.drawer.onReorderCallback = Reorder;
            this.drawer.elementHeight = 18;
            this.drawItem = DrawItem;
            this.createItem = CreateItem;
        }

        private void DrawHeader(Rect rect)
        {
            if (_title != null)
            {
                EditorGUI.LabelField(rect, new GUIContent(ObjectNames.NicifyVariableName(_title)));
            }
        }

        protected virtual bool DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            return false;
        }

        private void _DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            Rect r = rect;
            r.y += 1;
            r.height -= 2;
            if (drawItem(r, index, isActive, isFocused))
            {
                changed = true;
                SetDirty();
            }
        }

        protected virtual void Reorder(ReorderableList list)
        {
            SetDirty();
        }

        public void Refresh()
        {
            Filter(match);
        }

        protected virtual void OnChange() {}

        protected virtual T CreateItem() { return default(T); }

        private void _OnAdd(ReorderableList reorderList)
        {
            var o = CreateItem();
            if (drawer.list.IsFixedSize)
            {
                var arr = new T[list.Count + 1];
                arr[arr.Length - 1] = o;
                drawer.list = arr;
            }
            else
            {
                drawer.index = drawer.list.Add(o);
            }
            SetDirty();
            OnChange();
        }

        public void SetDirty()
        {
            EditorUtil.SetDirty(obj);
            changed = true;
        }

        public bool Draw()
        {
            changed = false;
            if (obj != null && drawer.serializedProperty == null)
            {
                Undo.RecordObject(obj, obj.name);
            }
            drawer.DoLayoutList();
            if (!changed && obj != null && drawer.serializedProperty == null)
            {
                Undo.ClearUndo(obj);
            }
            if (changed)
            {
                OnChange();
                SetDirty();
                return true;
            } else
            {
                return false;
            }
        }

        private Predicate<T> match;
        private int[] indexer; // used for filtering
        public void Filter(Predicate<T> match)
        {
            this.match = match;
            if (match != null)
            {
                List<T> filtered = new List<T>();
                indexer = new int[count];
                for (int i=0; i<count; ++i)
                {
                    if (match(this[i]))
                    {
                        indexer[filtered.Count] = i;
                        filtered.Add(this[i]);
                    }
                }
                drawer.list = filtered;
            } else
            {
                indexer = null;
                drawer.list = list;
            }
        }

        public int GetActualIndex(int index)
        {
            if (indexer != null && index < indexer.Length)
            {
                return indexer[index];
            } else
            {
                return index;
            }
        }

        public void Duplicate(int index)
        {
            int i = GetActualIndex(index);
            T o = (T)list[GetActualIndex(index)];
            list.Insert(i+1, o);
            this.drawer.index = i+1;
            SetDirty();
            OnChange();
            if (this.drawer.onAddCallback != null)
            {
                this.drawer.onAddCallback(this.drawer);
            }
        }

        public void Remove(int index)
        {
            int i = GetActualIndex(index);
            T o = (T)list[i];
            list.RemoveAt(i);
            SetDirty();
            OnChange();
            if (this.drawer.onRemoveCallback != null)
            {
                this.drawer.onRemoveCallback(this.drawer);
            }
        }

        public void Move(int sourceIndex, int destIndex)
        {
            int si = GetActualIndex(sourceIndex);
            int di = GetActualIndex(destIndex);
            list.Insert(di, list[si]);
            int si2 = si > di? si+1: si;
            list.RemoveAt(si2);
            SetDirty();
            OnChange();
            if (this.drawer.onReorderCallback != null)
            {
                this.drawer.onReorderCallback(this.drawer);
            }
        }

        public void Clear()
        {
            list.Clear();
            SetDirty();
            OnChange();
        }
    }
}

