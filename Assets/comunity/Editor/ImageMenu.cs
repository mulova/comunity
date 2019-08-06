using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine;
using mulova.commons;
using System.Text.Ex;

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

		[MenuItem("Assets/Image/Half Maxsize", false, 190)]
		static void HalfMaxSize()
		{
			EditorAssetUtil.ForEachSelection(p => {
				var size = TextureUtil.GetImageSize(p);
				if (size != null)
				{
					int maxSize = Mathf.NextPowerOfTwo(Mathf.Max(size[0], size[1])) / 2;
					TextureImporter im = AssetImporter.GetAtPath(p) as TextureImporter;
					if (im.maxTextureSize > maxSize)
					{
						im.SetMaxTextureSize(maxSize);
					}
				}
			}, FileType.Image);
		}

		[MenuItem("Assets/Image/RemoveAlpha", false, 1)]
		static void RemoveAlpha()
		{
			EditorAssetUtil.ForEachSelection(p => {
				TextureUtil.RemoveAlphaChannel(p);
			}, FileType.Image);
		}

		[MenuItem("Assets/Image/ToJPEG", false, 1)]
        static void ConvertToJpeg()
        {
            EditorAssetUtil.ForEachSelection(p => {
                TextureUtil.ToJpeg(p);
            }, FileType.Image);
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
            EditorAssetUtil.ForEachSelection(p => {
                TextureUtil.ToPng(p);
            }, FileType.Image);
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
            EditorAssetUtil.ForEachSelection(p => {
                Texture t = AssetDatabase.LoadAssetAtPath<Texture>(p);
                if (t != null)
                {
                    TextureUtil.Scale(t, 0.5f);
                }
            }, FileType.Image);
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
            EditorAssetUtil.ForEachSelection(p => {
                Texture t = AssetDatabase.LoadAssetAtPath<Texture>(p);
                if (t != null)
                {
                    TextureUtil.ScaleToImporterSizeRespectRatio(t);
                }
            }, FileType.Image);
        }
        
        [MenuItem("Assets/Image/Resize to ImporterSize", true)]
        static bool IsImporterSizeIgnore()
        {
            return IsEditorValid();
        }
        
        [MenuItem("Assets/Image/Resize to ImporterSize", false, 10)]
        public static void ImporterSizeIgnore()
        {
            EditorAssetUtil.ForEachSelection(p => {
                Texture t = AssetDatabase.LoadAssetAtPath<Texture>(p);
                if (t != null)
                {
                    TextureUtil.ScaleToImporterSize(t);
                }
            }, FileType.Image);
        }
        
        [MenuItem("Assets/Image/To ETC1", false, 20)]
        public static void SplitChannel4ETC()
        {
            EditorAssetUtil.ForEachSelection(p => {
                Texture t = AssetDatabase.LoadAssetAtPath<Texture>(p);
                if (t != null)
                {
                    TextureUtil.SplitChannel4ETC(t);
                }
            }, FileType.Image);
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
            EditorAssetUtil.ForEachSelection(p => {
                Texture2D t = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
                if (t != null)
                {
                    TextureUtil.Dither4444(t);
                }
            }, FileType.Image);
        }
        
        [MenuItem("Assets/Image/RGBA4444 with FloydSteinberg", false, 102)]
        public static void FloydSteinberg4444()
        {
            EditorAssetUtil.ForEachSelection(p => {
                Texture t = AssetDatabase.LoadAssetAtPath<Texture>(p);
                if (t != null)
                {
                    TextureUtil.FloydSteinberg4444(t);
                }
            }, FileType.Image);
        }

		[MenuItem("Assets/Image/Print ImageSize", false, 103)]
		public static void PrintImageSize()
		{
			EditorAssetUtil.ForEachSelection(p => {
				var size = TextureUtil.GetImageSize(p);
				if (size != null)
				{
					Debug.LogFormat("{0}: {1}x{2}", p, size[0], size[1]);
				}
			}, FileType.Image);
		}

    }
}

