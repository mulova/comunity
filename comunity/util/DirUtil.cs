﻿using comunity;
using System.IO;

namespace comunity
{
    public static class DirUtil2
    {
        public static void CreateDirectory(string path)
        {
            if (Platform.platform.IsWeb())
            {
                return;
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
