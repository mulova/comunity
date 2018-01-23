using System;

namespace drawer.ex
{
    public interface IItemDrawer<T>
    {
        bool DrawItem(UnityEngine.Rect position, int index, T obj, out T changedObj);
        void DrawItemBackground(UnityEngine.Rect position, int index, T obj);
        float GetItemHeight(int index, T obj);
    }
}

