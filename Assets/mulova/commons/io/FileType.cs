//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons
{
    [Flags]
    public enum FileType
    {
        // unity format
        Material = 1,
        Text = 2,
        Prefab = 4,
        Anim = 8,
        Model = 16,
        Image = 32,
        Audio = 64,
        Video = 128,
        Scene = 256,
        ScriptableObject = 512,
        // unity don't recognize
        Asset = 1024,
        Zip = 2048,
        Meta = 4096,
        Script= 8192,
        All = 16383,
    }

    public static class FileTypeEx
    {
        public static readonly FileType UNITY_SUPPORTED = FileType.Material|FileType.Text|FileType.Prefab|FileType.Anim|FileType.Model|FileType.Image|FileType.Audio;
        public static readonly FileType[] ALL = (FileType[])Enum.GetValues(typeof(FileType));
        public const string ASSET_BUNDLE = ".ab";

        private static string[][] EXT = new string[][] {
            new string[] { "" },
            new string[] { ".mat" },
            new string[] { ".txt", ".bytes", ".csv" },
            new string[] { ".prefab" },
            new string[] { ".anim" },
            new string[] { ".fbx" },
            new string[] { ".png", ".jpg", ".dds", ".tga", ".tiff", ".tif", ".psd" },
            new string[] { ".ogg", ".mp3", ".wav" },
            new string[] { ".mp4" },
            new string[] { ".unity" },
            new string[] { ASSET_BUNDLE },
            new string[] { ".asset" },
            new string[] { ".zip" },
            new string[] { ".meta" },
        };

        public static FileType GetFileType(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return FileType.All;
            }
            for (int i = 1; i < EXT.Length; i++)
            {
                foreach (string ext in EXT[i])
                {
                    if (path.EndsWithIgnoreCase(ext))
                    {
                        return (FileType)(1<<(i-1));
                    }
                }
            }
            return FileType.All;
        }

        public static string[] GetExt(this FileType fileType)
        {
            if (fileType == FileType.All)
            {
                return EXT[0];
            }
            List<string> list = new List<string>();
            for (int i = 1; i < EXT.Length; ++i)
            {
                if (((1<<(i-1))&(int)fileType) != 0)
                {
                    list.AddRange(EXT[i]);
                }
            }
            return list.ToArray();
        }
    }
}

