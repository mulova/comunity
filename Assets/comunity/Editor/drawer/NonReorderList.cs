using UnityEngine;
using UnityEditorInternal;
using System;
using Object = UnityEngine.Object;
using System.Collections;
using UnityEditor;
using System.Text.Ex;
using System.Collections.Generic;

namespace comunity
{
    /// <summary>
    /// Pure C# IList (non SerializableProperty array) cannot be reordered in UnityInternal ReorderableList
    /// </summary>
    public class NonReorderList<T>
    {
        public delegate T CreateItemDelegate();
        public delegate bool DrawItemDelegate(T item, Rect rect, int index, bool isActive, bool isFocused);
        public delegate void ChangeDelegate();
        public const float HEIGHT = 21;

        public readonly ReorderableList drawer;
        public bool changed { get; private set; }

        public CreateItemDelegate createItem;
        public DrawItemDelegate drawItem;
        public ChangeDelegate onChange = () => { };

        private string _title;
        public string title
        {
            set
            {
                _title = value;
                drawer.headerHeight = _title.IsEmpty()? 0: HEIGHT;
            }
        }


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

        public IList list
        {
            get
            {
                return drawer.list;
            }
            set
            {
                drawer.list = value;
            }
        }

        public NonReorderList(IList list)
        {
            this.drawer = new ReorderableList(list, typeof(T), true, false, true, true);
            this.list = (IList)list;
            Init();
        }

        private void Init()
        {
            this.drawer.onAddCallback = _OnAdd;
            this.drawer.drawHeaderCallback = DrawHeader;
            this.drawer.drawElementCallback = _DrawItem;
            this.drawer.onReorderCallback = Reorder;
            this.drawer.elementHeight = HEIGHT;
            this.drawer.elementHeightCallback = GetElementHeight;
            this.drawer.onRemoveCallback = OnRemove;
            this.createItem = CreateItem;
            this.drawItem = DrawItem;
        }

        private void OnRemove(ReorderableList list)
        {
            throw new NotImplementedException();
        }

        private float GetElementHeight(int index)
        {
            if (match == null || match(this[index]))
            {
                return drawer.elementHeight;
            } else
            {
                return 0;
            }
        }

        private void DrawHeader(Rect rect)
        {
            if (_title != null)
            {
                EditorGUI.LabelField(rect, new GUIContent(ObjectNames.NicifyVariableName(_title)));
            }
        }

        protected virtual bool DrawItem(T item, Rect rect, int index, bool isActive, bool isFocused)
        {
            return false;
        }

        private void _DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (match == null || match(this[index]))
            {
                Rect r = rect;
                r.y += 1;
                r.height -= 2;
                if (drawItem(this[index], r, index, isActive, isFocused))
                {
                    changed = true;
                    SetDirty();
                }
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
            onChange();
        }

        public void SetDirty()
        {
            changed = true;
        }

        public bool Draw()
        {
            changed = false;
            drawer.DoLayoutList();
            if (changed)
            {
                onChange();
                SetDirty();
                return true;
            } else
            {
                return false;
            }
        }

        private Predicate<T> match;
        public void Filter(Predicate<T> match)
        {
            this.match = match;
        }

        public void Duplicate(int index)
        {
            T o = (T)list[index];
            list.Insert(index+1, o);
            this.drawer.index = index+1;
            SetDirty();
            onChange();
            this.drawer.onAddCallback?.Invoke(this.drawer);
        }

        public void Remove(int i)
        {
            T o = (T)list[i];
            list.RemoveAt(i);
            SetDirty();
            onChange();
            this.drawer.onRemoveCallback?.Invoke(this.drawer);
        }

        public void Move(int si, int di)
        {
            list.Insert(di, list[si]);
            int si2 = si > di? si+1: si;
            list.RemoveAt(si2);
            SetDirty();
            onChange();
            this.drawer.onReorderCallback?.Invoke(this.drawer);
        }

        public void Clear()
        {
            list.Clear();
            SetDirty();
            onChange();
        }
    }
}

