using UnityEngine;
using UnityEditorInternal;
using System;
using Object = UnityEngine.Object;
using UnityEditor;
using System.Text.Ex;

namespace comunity
{
    public class ReorderSerialized<T>
    {
        public delegate void FillNewItemDelegate(int index);
        public delegate bool DrawItemDelegate(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused);
        public delegate T GettemDelegate(SerializedProperty p);
        public delegate void SetItemDelegate(SerializedProperty p, T value);
        public delegate void ChangeDelegate();

        public ReorderableList drawer { get; private set; }
        public bool changed { get; private set; }

        public FillNewItemDelegate fillNewItem { private get; set; }
        public DrawItemDelegate drawItem { private get; set; }
        public GettemDelegate getItem { private get; set; }
        public SetItemDelegate setItem { private get; set; }
        public ChangeDelegate onChange = () => { };

        // backup
        private float elementHeight;
        private float headerHeight;
        private float footerHeight;

        private string _title;
        public string title
        {
            set
            {
                _title = value;
                drawer.headerHeight = _title.IsEmpty()? 0: headerHeight;
            }
        }

        public bool displayIndex;

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

        protected Object obj
        {
            get
            {
                return drawer.serializedProperty.serializedObject.targetObject;
            }
        }

        public ReorderSerialized(SerializedObject ser, string propPath)
        {
            var prop = ser.FindProperty(propPath);
            Init(prop);
        }

        public ReorderSerialized(SerializedProperty prop)
        {
            Init(prop);
        }


        private void Init(SerializedProperty prop)
        {
            this.drawer = new ReorderableList(prop.serializedObject, prop, true, false, true, true);
            this.title = prop.propertyPath;
            this.getItem = GetItem;
            this.setItem = SetItem;
            this.drawer.onAddCallback = _OnAdd;
            this.drawer.drawHeaderCallback = DrawHeader;
            this.drawer.drawElementCallback = _DrawItem;
            this.drawer.onReorderCallback = Reorder;
            this.drawer.elementHeightCallback = GetElementHeight;
            this.fillNewItem = FillNewItem;
            this.drawItem = DrawItem;
            // backup
            elementHeight = this.drawer.elementHeight;
            headerHeight = this.drawer.headerHeight;
            footerHeight = this.drawer.footerHeight;
        }

        private float GetElementHeight(int index)
        {
            if (match == null || match(this[index]))
            {
                return drawer.elementHeight;
            }
            else
            {
                return 0;
            }
        }

        private void DrawHeader(Rect rect)
        {
            if (!_title.IsEmpty())
            {
                EditorGUI.LabelField(rect, new GUIContent(ObjectNames.NicifyVariableName(_title)));
            }
        }

        protected virtual bool DrawItem(SerializedProperty item, Rect rect, int index, bool isActive, bool isFocused)
        {
            return EditorGUI.PropertyField(rect, item, new GUIContent(""));
        }

        private void _DrawItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (match == null || match(this[index]))
            {
                Rect r = rect;
                r.y += 1;
                r.height -= 2;

                var item = drawer.serializedProperty.GetArrayElementAtIndex(index);
                if (displayIndex)
                {
                    var rects = rect.SplitByWidths(20);
                    EditorGUI.LabelField(rects[0], index.ToString(), EditorStyles.boldLabel);
                    r = rects[1];
                }

                if (drawItem(item, r, index, isActive, isFocused))
                {
                    changed = true;
                    SetDirty();
                }
            }
        }

        protected virtual void FillNewItem(int index)
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

        private void _OnAdd(ReorderableList reorderList)
        {
            drawer.serializedProperty.InsertArrayElementAtIndex(drawer.serializedProperty.arraySize);
            drawer.index = drawer.serializedProperty.arraySize - 1;
            fillNewItem(drawer.index);
            SetDirty();
            onChange();
        }

        public void SetDirty()
        {
            EditorUtil.SetDirty(obj);
            changed = true;
        }

        public bool Draw()
        {
            return DrawInternal(drawer.DoLayoutList);
        }

        public bool Draw(Rect rect)
        {
            return DrawInternal(()=>drawer.DoList(rect));
        }

        private bool DrawInternal(Action drawAction)
        {
            changed = false;
            if (obj != null && drawer.serializedProperty == null)
            {
                Undo.RecordObject(obj, obj.name);
            }
            drawAction();
            if (!changed && obj != null && drawer.serializedProperty == null)
            {
                Undo.ClearUndo(obj);
            }
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
    }
}

