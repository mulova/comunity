using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using mulova.commons;

namespace comunity
{
	public class TexFormatGroupData : EnumClass<TexFormatGroupData>
	{
		public static readonly TexFormatGroupData ASTC = new TexFormatGroupData("ASTC", 
            TextureImporterFormat.ASTC_RGBA_4x4,
            TextureImporterFormat.ASTC_RGBA_5x5,
            TextureImporterFormat.ASTC_RGBA_6x6,
            TextureImporterFormat.ASTC_RGBA_8x8,
            TextureImporterFormat.ASTC_RGBA_10x10,
            TextureImporterFormat.ASTC_RGBA_12x12,
            TextureImporterFormat.ASTC_RGB_4x4,
            TextureImporterFormat.ASTC_RGB_5x5,
            TextureImporterFormat.ASTC_RGB_6x6,
            TextureImporterFormat.ASTC_RGB_8x8,
            TextureImporterFormat.ASTC_RGB_10x10,
            TextureImporterFormat.ASTC_RGB_12x12
            ) { rgba = TextureImporterFormat.ASTC_RGBA_8x8, rgb = TextureImporterFormat.ASTC_RGB_8x8};
		public static readonly TexFormatGroupData ETC2 = new TexFormatGroupData("ETC2",
            TextureImporterFormat.ETC2_RGBA8,
            TextureImporterFormat.ETC2_RGB4,
            TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA,
            TextureImporterFormat.EAC_R,
            TextureImporterFormat.EAC_R_SIGNED,
            TextureImporterFormat.EAC_RG,
            TextureImporterFormat.EAC_RG_SIGNED
	        ) { rgba = TextureImporterFormat.ETC2_RGBA8, rgb = TextureImporterFormat.ETC2_RGB4};
		public static readonly TexFormatGroupData ATC = new TexFormatGroupData("ATC",
			TextureImporterFormat.ETC_RGB4,
			TextureImporterFormat.ETC2_RGBA8
			) { rgba = TextureImporterFormat.ETC2_RGBA8, rgb = TextureImporterFormat.ETC_RGB4 };
		public static readonly TexFormatGroupData PVRTC = new TexFormatGroupData("PVRTC",
			TextureImporterFormat.PVRTC_RGB2,
			TextureImporterFormat.PVRTC_RGB4,
			TextureImporterFormat.PVRTC_RGBA2,
			TextureImporterFormat.PVRTC_RGBA4
			) { rgba = TextureImporterFormat.PVRTC_RGBA4, rgb = TextureImporterFormat.PVRTC_RGB4 };
		public static readonly TexFormatGroupData DXT = new TexFormatGroupData("DXT",
			TextureImporterFormat.DXT1,
			TextureImporterFormat.DXT1Crunched,
			TextureImporterFormat.DXT5,
			TextureImporterFormat.DXT5Crunched
			) { rgba = TextureImporterFormat.DXT5Crunched, rgb = TextureImporterFormat.DXT1Crunched};
		public static readonly TexFormatGroupData ETC = new TexFormatGroupData("ETC",
			TextureImporterFormat.ETC_RGB4
            ) { rgba = TextureImporterFormat.Automatic, rgb = TextureImporterFormat.ETC2_RGB4 };
		public static readonly TexFormatGroupData AUTO = new TexFormatGroupData("AUTO",
            TextureImporterFormat.Automatic
#pragma warning disable 0618
            ) { rgba = TextureImporterFormat.AutomaticCrunched, rgb = TextureImporterFormat.AutomaticCrunched};
#pragma warning restore 0618

		public TextureImporterFormat rgb { get; private set; }
		public TextureImporterFormat rgba { get; private set; }
		public readonly TextureImporterFormat[] formats;
		private HashSet<string> formatNames;

		private TexFormatGroupData(string id, params TextureImporterFormat[] formats) : base(id)
		{
			this.formats = formats;
		}

		public bool IsGroupOf(TextureImporterFormat format)
		{
			foreach (var f in formats)
			{
				if (f == format)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsGroupOf(string format)
		{
			if (formatNames == null)
			{
				formatNames = new HashSet<string>();
				foreach (TextureImporterFormat f in formats)
				{
					formatNames.Add(f.ToString());
				}
			}
			return formatNames.Contains(format);
		}
	}
}

