using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;
using commons;

namespace comunity
{
    public static class ImageMenu
    {
        private static bool IsEditorValid()
        {
            if (!TextureUtil.IsValid()||Selection.objects.IsEmpty())
            {
                return false;
            }
            foreach (Object o in Selection.objects)
            {
                if (!(o is Texture))
                {
                    return false;
                }
            }
            return true;
        }
        
        [MenuItem("Assets/Image/ToJPEG", false, 1)]
        static void ConvertToJpeg()
        {
            foreach (Object o in Selection.objects)
            {
                Texture t = o as Texture;
                string src = AssetDatabase.GetAssetPath(t);
                TextureUtil.ToJpeg(src);
            }
        }
        
        [MenuItem("Assets/Image/ToJPEG", true)]
        static bool IsConvertToJpeg()
        {
            if (IsEditorValid())
            {
                foreach (Object o in Selection.objects)
                {
                    Texture t = o as Texture;
                    string path = AssetDatabase.GetAssetPath(t);
                    if (path.EndsWithIgnoreCase(".jpg"))
                    {
                        return false;
                    }
                }
                return true;
            } else
            {
                return false;
            }
        }
        
        [MenuItem("Assets/Image/ToPNG", false, 2)]
        static void ConvertToPng()
        {
            foreach (Object o in Selection.objects)
            {
                Texture t = o as Texture;
                string src = AssetDatabase.GetAssetPath(t);
                TextureUtil.ToPng(src);
            }
        }
        
        [MenuItem("Assets/Image/ToPNG", true)]
        static bool IsConvertToPng()
        {
            if (!IsEditorValid())
            {
                return false;
            }
            foreach (Object o in Selection.objects)
            {
                Texture t = o as Texture;
                string path = AssetDatabase.GetAssetPath(t);
                if (path.EndsWithIgnoreCase(".png"))
                {
                    return false;
                }
            }
            return true;
        }
        
        [MenuItem("Assets/Image/ScaleDown", false, 5)]
        public static void ScaleDown()
        {
            foreach (Object o in Selection.objects)
            {
                TextureUtil.Scale(o as Texture, 0.5f);
            }
        }
        
        [MenuItem("Assets/Image/ScaleDown", true)]
        static bool IsScaleDown()
        {
            return IsEditorValid();
        }
        
        [MenuItem("Assets/Image/Resize to ImporterSize Respect Ratio", true)]
        static bool IsImporterSize()
        {
            return IsEditorValid();
        }
        
        [MenuItem("Assets/Image/Resize to ImporterSize Respect Ratio", false, 15)]
        public static void ImporterSize()
        {
            foreach (Object o in Selection.objects)
            {
                TextureUtil.ScaleToImporterSizeRespectRatio(o as Texture);
            }
        }
        
        [MenuItem("Assets/Image/Resize to ImporterSize", true)]
        static bool IsImporterSizeIgnore()
        {
            return IsEditorValid();
        }
        
        [MenuItem("Assets/Image/Resize to ImporterSize", false, 10)]
        public static void ImporterSizeIgnore()
        {
            foreach (Object o in Selection.objects)
            {
                TextureUtil.ScaleToImporterSize(o as Texture);
            }
        }
        
        [MenuItem("Assets/Image/To ETC1", false, 20)]
        public static void SplitChannel4ETC()
        {
            foreach (Object o in Selection.objects)
            {
                TextureUtil.SplitChannel4ETC(o as Texture);
            }
        }
        
        [MenuItem("Assets/Image/To ETC1", true)]
        public static bool IsSplitAlpha()
        {
            if (!IsEditorValid())
            {
                return false;
            }
            foreach (Object o in Selection.objects)
            {
                string path = AssetDatabase.GetAssetPath(o);
                if (!path.EndsWithIgnoreCase(".png"))
                {
                    return false;
                }
            }
            return true;
        }
        
        [MenuItem("Assets/Image/Dither4444", false, 101)]
        static void Dither4444()
        {
            foreach (Object o in Selection.objects)
            {
                Texture2D t = o as Texture2D;
                if (t != null)
                {
                    TextureUtil.Dither4444(t);
                }
            }
        }
        
        [MenuItem("Assets/Image/RGBA4444 with FloydSteinberg", false, 102)]
        public static void FloydSteinberg4444()
        {
            foreach (Object o in Selection.objects)
            {
                Texture t = o as Texture;
                TextureUtil.FloydSteinberg4444(t);
            }
        }
    }
}

