using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.Collections;
using UnityEditor;
using UnityEngine.Ex;
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
        private string _title;
        public string title
        {
            set
            {
                _title = value;
                drawer.headerHeight = _title.IsEmpty()? 0: 18;
            }
        }

        protected CreateItemDelegate createItem = () => default(T);
        protected DrawItemDelegate drawItem = (r,i,a,f) => false;

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

        protected virtual T GetSerializedItem(SerializedProperty p, int i) {
            return default(T);
        }

        protected virtual void SetSerializedItem(SerializedProperty p, int i, T val) {}


        public T this[int i]
        {
            get {
                if (drawer.serializedProperty != null && drawer.serializedProperty.isArray)
                {
                    return GetSerializedItem(drawer.serializedProperty, i);
                } else
                {
                    return (T)drawer.list[i];
                }
            }
            set
            {
                if (drawer.serializedProperty != null && drawer.serializedProperty.isArray)
                {
                    SetSerializedItem(drawer.serializedProperty, i, value);
                } else
                {
                    drawer.list[i] = value;
                }
                SetDirty();
            }
        }

        public int count
        {
            get {
                if (drawer.serializedProperty != null)
                {
                    return drawer.serializedProperty.arraySize;
                } else
                {
                    return drawer.list.Count;
                }
            }
        }

        public int allCount
        {
            get {
                return list.Count;
            }
        }

        public ReorderList(Object obj, IList list)
        {
            this.obj = obj;
            this.list = list;
            this.drawer = new ReorderableList(list, typeof(T), true, false, true, true);
            Init();
        }

        public ReorderList(SerializedObject ser, string propPath)
        {
            this.obj = ser.targetObject;
            var prop = ser.FindProperty(propPath);
            this.drawer = new ReorderableList(ser, prop, true, false, true, true);
            this.title = propPath;
            Init();
        }

        private void Init()
        {
            this.drawer.onAddCallback = OnAdd;
            this.drawer.drawHeaderCallback = DrawHeader;
            this.drawer.drawElementCallback = DrawItem0;
            this.drawer.onReorderCallback = Reorder;
            this.drawer.elementHeight = 18;
        }

        private void DrawHeader(Rect rect)
        {
            if (_title != null)
            {
                EditorGUI.LabelField(rect, new GUIContent(ObjectNames.NicifyVariableName(_title)));
            }
        }

        private void DrawItem0(Rect rect, int index, bool isActive, bool isFocused)
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

        protected void OnAdd(ReorderableList reorderList)
        {
            if (drawer.serializedProperty != null)
            {
                drawer.serializedProperty.arraySize += 1;
                drawer.index = reorderList.serializedProperty.arraySize - 1;
                drawer.list[drawer.index] = createItem();
            }
            else
            {
                if (reorderList.list.IsFixedSize)
                {
                    var arr = new T[list.Count+1];
                    arr[arr.Length-1] = createItem();
                    list = arr;
                    drawer.list = arr;
                } else
                {
                    drawer.index = drawer.list.Add(createItem());
                }
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
            T obj = (T)list[GetActualIndex(index)];
            list.Insert(i+1, obj);
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
            T obj = (T)list[i];
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

