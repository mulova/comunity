using comunity;
using UnityEditor;

namespace comunity
{
	public static class TextureImporterFormatEx
	{
		public static bool IsMemberOf(this TextureImporterFormat format, TexFormatGroup group)
		{
			return group.IsGroupOf(format);
		}
	}
}

