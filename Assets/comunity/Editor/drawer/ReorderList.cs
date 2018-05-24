using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using comunity;
using System.Collections;

public abstract class ReorderList<T>
{
    public readonly ReorderableList drawer;
    private Object obj;
    private IList list;
    private bool changed;

    public T this[int i]
    {
        get {
            return (T)drawer.list[i];
        }
        set
        {
            drawer.list[i] = value;
        }
    }

    public int AllCount
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

    public ReorderList(Object obj, IList src)
    {
        this.obj = obj;
        this.drawer = new ReorderableList(src, typeof(T), true, true, true, true);
        this.drawer.onAddCallback = AddItem;
        this.drawer.drawElementCallback = DrawItem0;
        this.drawer.onReorderCallback = Reorder;
        this.drawer.elementHeight = 18;
    }

    protected abstract T createItem();

    protected abstract bool DrawItem(Rect rect, int index, bool isActive, bool isFocused);

    private void DrawItem0(Rect rect, int index, bool isActive, bool isFocused)
    {
        Rect r = rect;
        r.y += 1;
        r.height -= 2;
        changed |= DrawItem(r, index, isActive, isFocused);
    }

    protected virtual void Reorder(ReorderableList list)
    {
        SetDirty();
    }

    protected void AddItem(ReorderableList list)
    {
        if (list.serializedProperty != null)
        {
            list.serializedProperty.arraySize += 1;
            list.index = list.serializedProperty.arraySize - 1;
        }
        else
        {
            list.index = list.list.Add(createItem());
        }
        changed = true;
    }

    public void SetDirty()
    {
        if (obj != null)
        {
            EditorUtil.SetDirty(obj);
        }
        changed = false;
    }

    public bool Draw()
    {
        changed = false;
        drawer.DoLayoutList();
        if (changed)
        {
            SetDirty();
            return true;
        } else
        {
            return false;
        }
    }

    private Predicate<T> match;
    private int[] indexer; // used for filtering
    private int count;
    public void Filter(Predicate<T> match)
    {
        this.match = match;
        if (match != null)
        {
            indexer = new int[AllCount];
            int n = 0;
            for (int i=0; i<AllCount; ++i)
            {
                if (match(this[i]))
                {
                    indexer[n] = i;
                    n++;
                }
            }
            count = n;
        } else
        {
            indexer = null;
            count = AllCount;
        }
    }
}

