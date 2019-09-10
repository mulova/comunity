using System;

namespace UnityEngine.UI
{
    public static class TextEx
    {
        public static void SetText(this Text t, string str)
        {
            if (t == null)
            {
                return;
            }
            t.text = str;
        }
    }
}

