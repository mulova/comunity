using System;
using UnityEngine;
using comunity;

namespace UnityEditor
{
	public static class TextureImporterFormatEx
	{
		public static bool IsMemberOf(this TextureImporterFormat format, TexFormatGroup group)
		{
			return group.IsGroupOf(format);
		}
	}
}

