using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using comunity;
using System.Collections;
using UnityEditor;

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

    public ReorderList(Object obj, IList src)
    {
        this.obj = obj;
        this.drawer = new ReorderableList(src, typeof(T), true, true, true, true);
        this.drawer.onAddCallback = OnAdd;
        this.drawer.drawElementCallback = DrawItem0;
        this.drawer.onReorderCallback = Reorder;
        this.drawer.elementHeight = 18;
        this.drawer.headerHeight = 0;
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

    public void Refresh()
    {
        Filter(match);
    }

    protected void OnAdd(ReorderableList list)
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
            SetDirty();
            return true;
        } else
        {
            return false;
        }
    }

    private Predicate<T> match;
    private int[] indexer; // used for filtering
    private int filteredCount;
    public void Filter(Predicate<T> match)
    {
        this.match = match;
        this.drawer.draggable = match == null;
        if (match != null)
        {
            indexer = new int[count];
            int n = 0;
            for (int i=0; i<count; ++i)
            {
                if (match(this[i]))
                {
                    indexer[n] = i;
                    n++;
                }
            }
            filteredCount = n;
        } else
        {
            indexer = null;
            filteredCount = allCount;
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
        changed = true;
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
        changed = true;
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
        changed = true;
        if (this.drawer.onReorderCallback != null)
        {
            this.drawer.onReorderCallback(this.drawer);
        }
    }

    public void Clear()
    {
        list.Clear();
        changed = true;
    }
}

