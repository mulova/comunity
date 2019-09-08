//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;
using mulova.commons;
using System.Text.Ex;
using UnityEngine.Ex;

namespace mulova.comunity
{
	public static class TexFormatGroupEx
	{
		public static Loggerx log = LogManager.GetLogger(typeof(TexFormatGroup));

		public static TexFormatGroupData ToData(this TexFormatGroup format)
		{
            return TexFormatGroupData.ParseIgnoreCase(format.ToString());
		}

		public static bool IsGroupOf(this TexFormatGroup group, TextureImporterFormat format)
		{
			return group.ToData().IsGroupOf(format);
		}

		public static bool IsGroupOf(this TexFormatGroup group, string format)
		{
			return group.ToData().IsGroupOf(format);
		}

		public static TextureImporterFormat rgba(this TexFormatGroup group)
		{
			return group.ToData().rgba;
		}

		public static TextureImporterFormat rgb(this TexFormatGroup group)
		{
			return group.ToData().rgb;
		}

		public static void Convert(this TexFormatGroup format, string folder)
		{
			log.Debug("Converting texture format to '{0}'", format);
			Regex ignore = new Regex("/Editor/|Assets/Plugins/");
			foreach (string p in EditorAssetUtil.ListAssetPaths(folder, FileType.Image)) {
				if (ignore.IsMatch(p))
				{
					continue;
				}
				log.Debug(p);
				Texture2D t = AssetDatabase.LoadAssetAtPath<Texture2D>(p);
				if (t != null)
				{
					format.Convert(t);
				}
				
			}
		}

		public static void ConvertDependencies(this TexFormatGroup newFormat, string[] assetPaths)
		{
			foreach (string depPath in AssetDatabase.GetDependencies(assetPaths))
			{
				if (depPath.Is(FileType.Image))
				{
					Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(depPath);
					newFormat.Convert(tex);
				}
			}
		}

		public static TextureImporterFormat GetFormatFromLabel(Texture2D tex, TexFormatGroup group, bool alpha)
		{
			string[] labels = AssetDatabase.GetLabels(tex);
			if (labels.IsNotEmpty())
			{
				foreach (string l in labels)
				{
					if (group.IsGroupOf(l))
					{
						return EnumUtil.Parse<TextureImporterFormat>(l);
					}
				}
			}
			return alpha? group.rgba(): group.rgb();
		}
		
		public static void Convert(this TexFormatGroup newFormat, Texture2D tex)
		{
			string path = AssetDatabase.GetAssetPath(tex);
			TextureImporter im = AssetImporter.GetAtPath(path) as TextureImporter;
			if (im == null || tex.format == TextureFormat.Alpha8)
			{
				return;
			}
			bool changed = false;
			if (newFormat == TexFormatGroup.ASTC&&!tex.format.IsASTC())
			{
				im.SetFormat(GetFormatFromLabel(tex, newFormat, tex.format.HasAlpha()));
				changed = true;
			} else if (newFormat == TexFormatGroup.ETC2&&!tex.format.IsETC2())
			{
                im.SetFormat(GetFormatFromLabel(tex, newFormat, tex.format.HasAlpha()));
				changed = true;
			} else if (newFormat == TexFormatGroup.PVRTC&&!tex.format.IsPVRTC())
			{
				if (EditorUserBuildSettings.activeBuildTarget.ToRuntimePlatform().IsIos() &&
#pragma warning disable 0618
                    (im.GetFormat() == TextureImporterFormat.AutomaticCompressed || im.GetFormat() == TextureImporterFormat.AutomaticCrunched)) 
#pragma warning restore 0618
				{
					// automatic is the same as PVRTC
				} else if (tex.format.HasAlpha())
				{
					// PVRTC with alpha downgrades the image quality
//					im.textureFormat = TextureImporterFormat.PVRTC_RGB4;
//					changed = true;
				} else
				{
					im.SetFormat(GetFormatFromLabel(tex, newFormat, tex.format.HasAlpha()));
					changed = true;
				}
			} else if (newFormat == TexFormatGroup.ETC && !tex.format.IsETC())
			{
				if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android &&
#pragma warning disable 0618
					(im.GetFormat() == TextureImporterFormat.AutomaticCompressed || im.GetFormat() == TextureImporterFormat.AutomaticCrunched)) 
#pragma warning restore 0618
				{
					// automatic is the same as ETC
				} else if (tex.format.HasAlpha())
				{
					// alpha is not supported in ETC format
				} else
				{
					im.SetFormat(GetFormatFromLabel(tex, newFormat, tex.format.HasAlpha()));
					changed = true;
				}
#pragma warning disable 0618
			} else if (newFormat == TexFormatGroup.AUTO && (im.GetFormat() != TextureImporterFormat.AutomaticCompressed && im.GetFormat() != TextureImporterFormat.AutomaticCrunched))
#pragma warning restore 0618
			{
				if (tex.format.HasAlpha())
				{
					// compressing alpha
				} else
				{
					im.SetFormat(GetFormatFromLabel(tex, newFormat, tex.format.HasAlpha()));
					changed = true;
				}
			}
            if (!AssetConfig.TEX_NPOT && im.npotScale == TextureImporterNPOTScale.None && !tex.IsPOT())
			{
				im.npotScale = TextureImporterNPOTScale.ToNearest;
				changed = true;
			}
			if (changed)
			{
                im.textureType = TextureImporterType.Default;
				im.isReadable = false;
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
			}
		}
	}
}

