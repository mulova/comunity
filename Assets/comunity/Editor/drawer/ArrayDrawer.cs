using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine;
using commons;

namespace comunity
{
    public class ArrayDrawer<T> : ListDrawer<T> where T:class
    {
        private object target;
        private string fieldName;

        public ArrayDrawer(Object target, string fieldName) : this(target, target, fieldName, new ItemDrawer<T>())
        {
        }

        public ArrayDrawer(Object unityObj, object target, string fieldName) : this(unityObj, target, fieldName, new ItemDrawer<T>())
        {
        }

        public ArrayDrawer(Object unityObj, object target, string fieldName, IItemDrawer<T> itemDrawer)
            : base(new List<T>(ReflectionUtil.GetFieldValue<T[]>(target, fieldName)), itemDrawer)
        {
            this.target = target;
            this.fieldName = fieldName;
            this.title = fieldName;
            this.undoTarget = unityObj;
            onDuplicate += Refresh;
            onInsert += Refresh;
            onMove += Refresh;
            onRemove += Refresh;
            onChange += SetDirty;
        }

        void SetDirty()
        {
            if (target is Object)
            {
                CompatibilityEditor.SetDirty(target as Object);
            }
            if (undoTarget is Object)
            {
                CompatibilityEditor.SetDirty(undoTarget as Object);
            }
        }

        void SetDirty(int arg1, T arg2)
        {
            SetDirty();
            Refresh();
        }

        void Refresh(int arg1, T arg2)
        {
            Refresh();
        }

        void Refresh(int arg1, int arg2, T arg3)
        {
            Refresh();
        }

        private void Refresh()
        {
            ReflectionUtil.SetFieldValue(target, fieldName, list.ToArray());
            SetDirty();
        }

        public override void Add(T item)
        {
            base.Add(item);
            Refresh();
        }
    }
}

