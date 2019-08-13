using UnityEngine;

public static class RectEx
{
    public static Rect[] SplitHorizontally(this Rect src, int count)
    {
        Rect[] rects = new Rect[count];
        float width = src.width / count;
        for (int i = 0; i < count; ++i)
        {
            rects[i] = src;
            rects[i].x = src.x + i * width;
            rects[i].width = width;
        }
        return rects;
    }
}
