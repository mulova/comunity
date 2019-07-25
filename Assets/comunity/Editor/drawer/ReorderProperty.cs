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
    public class ReorderProperty<T>
    {
        public readonly ReorderableList drawer;
        protected Object obj { get; set; }
        public bool changed { get; private set; }
        public delegate void FillNewItemDelegate(object item);
        public delegate bool DrawItemDelegate(Rect rect, int index, bool isActive, bool isFocused);
        public delegate T GettemDelegate(SerializedProperty p);
        public delegate void SetItemDelegate(SerializedProperty p, T value);

        private string _title;
        public string title
        {
            set
            {
                _title = value;
                drawer.headerHeight = _title.IsEmpty()? 0: 18;
            }
        }

        public FillNewItemDelegate fillNewItem;
        public DrawItemDelegate drawItem;
        public GettemDelegate getItem;
        public SetItemDelegate setItem;

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
                var e = drawer.serializedProperty.GetArrayElementAtIndex(i);
                return getItem(e);
            }
            set
            {
                if (drawer.serializedProperty.arraySize < i)
                {
                    drawer.serializedProperty.InsertArrayElementAtIndex(i);
                }
                var e = drawer.serializedProperty.GetArrayElementAtIndex(i);
                setItem(e, value);
                SetDirty();
            }
        }

        public int count
        {
            get {
                return drawer.serializedProperty.arraySize;
            }
        }

        public ReorderProperty(SerializedObject ser, string propPath)
        {
            this.obj = ser.targetObject;
            var prop = ser.FindProperty(propPath);
            this.drawer = new ReorderableList(ser, prop, true, false, true, true);
            this.title = propPath;
            this.getItem = GetItem;
            this.setItem = SetItem;
            Init();
        }

        private void Init()
        {
            this.drawer.onAddCallback = _OnAdd;
            this.drawer.drawHeaderCallback = DrawHeader;
            this.drawer.drawElementCallback = _DrawItem;
            this.drawer.onReorderCallback = Reorder;
            this.drawer.elementHeight = 18;
            this.fillNewItem = FillNewItem;
            this.drawItem = DrawItem;
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

        protected virtual void FillNewItem(object o)
        {
        }

        protected virtual T GetItem(SerializedProperty p)
        {
            throw new NotImplementedException();
        }

        protected virtual void SetItem(SerializedProperty p, T value)
        {
            throw new NotImplementedException();
        }

        protected virtual void Reorder(ReorderableList list)
        {
            SetDirty();
        }

        protected virtual void OnChange() {}

        private void _OnAdd(ReorderableList reorderList)
        {
            drawer.serializedProperty.InsertArrayElementAtIndex(drawer.serializedProperty.arraySize);
            drawer.index = drawer.serializedProperty.arraySize - 1;
            fillNewItem(drawer.serializedProperty.GetArrayElementAtIndex(drawer.index));
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
    }
}

