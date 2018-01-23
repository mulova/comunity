using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System;
using System.IO;

namespace core {
	public class TextureTrimmer {
		
		[MenuItem("Assets/Trim Texture")]
		public static void TrimTexture() {
			foreach (Object o in Selection.objects) {
				if (typeof(Texture2D).IsAssignableFrom(o.GetType())) {
					Texture2D src = o as Texture2D;
					string srcPath = AssetDatabase.GetAssetPath(src);
					TextureImporter importer = AssetImporter.GetAtPath(srcPath) as TextureImporter;
					bool readable = importer.isReadable;
					if (!readable) {
						importer.isReadable = true;
						AssetDatabase.ImportAsset(srcPath, ImportAssetOptions.ForceUpdate);
					}
					Texture2D dst = GetTrimmed(src);
					File.WriteAllBytes(srcPath, dst.EncodeToPNG());
					if (!readable) {
						importer.isReadable = false;
						AssetDatabase.ImportAsset(srcPath, ImportAssetOptions.ForceUpdate);
					}
				}
			}
		}

		private static Texture2D GetTrimmed(Texture2D tex) {
			Color32[] pixels = tex.GetPixels32();

			bool trim = true;
			int minx = 0;
			for (int w = 0; w<tex.width && trim; w++) {
				for (int h = 0; h<tex.height && trim; h++) {
					Color32 c = GetPixel(pixels, tex.width, w, h);
					if (c.a != 0) {
						trim = false;
						minx = w;
					}
				}
			}

			int maxx = tex.width-1;
			trim = true;
			for (int w = tex.width-1; w>minx && trim; w--) {
				for (int h = 0; h<tex.height && trim; h++) {
					Color32 c = GetPixel(pixels, tex.width, w, h);
					if (c.a != 0) {
						trim = false;
						maxx = w;
					}
				}
			}

			int miny = 0;
			trim = true;
			for (int h = 0; h<tex.height && trim; h++) {
				for (int w = 0; w<tex.width && trim; w++) {
					Color32 c = GetPixel(pixels, tex.width, w, h);
					if (c.a != 0) {
						trim = false;
						miny = h;
					}
				}
			}
			
			int maxy = tex.width-1;
			trim = true;
			for (int h = tex.height-1; h>miny && trim; h--) {
				for (int w = 0; w<tex.width && trim; w++) {
					Color32 c = GetPixel(pixels, tex.width, w, h);
					if (c.a != 0) {
						trim = false;
						maxy = h;
					}
				}
			}

			int width = maxx - minx + 1;
			int height = maxy - miny + 1;
			Color32[] trimmed = new Color32[width*height];
			int i = 0;
			for (int h = miny; h<=maxy; h++) {
				for (int w = minx; w<=maxx; w++) {
					trimmed[i] = GetPixel(pixels, tex.width, w, h);
					i++;
				}
			}
			Texture2D dst = new Texture2D(width, height);
			dst.SetPixels32(trimmed);
			return dst;
		}

		private static Color32 GetPixel(Color32[] pixels, int width, int x, int y) {
			return pixels[width * y + x];
		}
		
		[MenuItem("Assets/Trim Texture", true)]
		public static bool IsTrimTexture() {
			return Selection.activeObject != null && typeof(Texture2D).IsAssignableFrom(Selection.activeObject.GetType());
		}
	}
}
