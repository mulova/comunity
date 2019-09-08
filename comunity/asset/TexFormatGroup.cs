//#define OPTIMIZED
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using mulova.commons;

namespace mulova.comunity
{
	public class TexFormatGroup : EnumClass<TexFormatGroup>
	{
		public static readonly TexFormatGroup ASTC = new TexFormatGroup("astc");
		public static readonly TexFormatGroup ETC2 = new TexFormatGroup("etc2");
		public static readonly TexFormatGroup ATC = new TexFormatGroup("atc");
		public static readonly TexFormatGroup PVRTC = new TexFormatGroup("pvrtc");
		public static readonly TexFormatGroup DXT = new TexFormatGroup("dxt");
		public static readonly TexFormatGroup ETC = new TexFormatGroup("etc");
		public static readonly TexFormatGroup AUTO = new TexFormatGroup("auto");

		private TexFormatGroup(string id) : base(id)
		{
		}
		
		public static TexFormatGroup GetBest()
		{
#if OPTIMIZED
			if (SystemInfo.SupportsTextureFormat(TextureFormat.ASTC_RGBA_12x12))
			{
				return ASTC;
			} else if (SystemInfo.SupportsTextureFormat(TextureFormat.ETC2_RGBA8))
			{
				return ETC2;
			} else if (SystemInfo.SupportsTextureFormat(TextureFormat.ATC_RGBA8))
			{
				return ATC;
			} else if (SystemInfo.SupportsTextureFormat(TextureFormat.PVRTC_RGBA4))
			{
				return PVRTC;
			} else if (SystemInfo.SupportsTextureFormat(TextureFormat.DXT5))
			{
				return DXT;
			} else
			{
				return AUTO;
			}
#else
			return AUTO;
#endif
		}
	}
}

