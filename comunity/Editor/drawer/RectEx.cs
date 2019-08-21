using UnityEngine;

public static class RectEx
{
    public static Rect[] SplitByHeightCount(this Rect src, int count)
    {
        Rect[] rects = new Rect[count];
        for (int i = 0; i < count; ++i)
        {
            rects[i] = src;
            rects[i].height = src.height/count;
            rects[i].y = src.y + src.height / count * i;
        }
        return rects;
    }

    public static Rect[] SplitByHeights(this Rect src, params int[] heights)
    {
        Rect[] rects = new Rect[heights.Length + 1];
        for (int i = 0; i < heights.Length; ++i)
        {
            rects[i] = src;
            rects[i].height = heights[i];
            if (i > 0)
            {
                rects[i].y = rects[i - 1].y + heights[i - 1];
            }
        }
        rects[heights.Length] = src;
        rects[heights.Length].y = rects[heights.Length - 1].y + heights[heights.Length - 1];
        rects[heights.Length].height = src.x + src.height - rects[heights.Length].x;
        return rects;
    }

    public static Rect[] SplitByWidthsRatio(this Rect src, params float[] ratio)
    {
        Rect[] rects = new Rect[ratio.Length];
        for (int i = 0; i < rects.Length; ++i)
        {
            rects[i] = src;
            if (i > 0)
            {
                rects[i].x = rects[i-1].x + ratio[i-1] * src.width;
            }
            rects[i].width = src.width*ratio[i];
        }
        return rects;
    }

    public static Rect[] SplitByWidths(this Rect src, params int[] widths)
    {
        Rect[] rects = new Rect[widths.Length+1];
        for (int i = 0; i < widths.Length; ++i)
        {
            rects[i] = src;
            rects[i].width = widths[i];
            if (i > 0)
            {
                rects[i].x = rects[i - 1].x + widths[i - 1];
            }
        }
        rects[widths.Length] = src;
        rects[widths.Length].x = rects[widths.Length-1].x + widths[widths.Length - 1];
        rects[widths.Length].width = src.x + src.width - rects[widths.Length].x;
        return rects;
    }
}
