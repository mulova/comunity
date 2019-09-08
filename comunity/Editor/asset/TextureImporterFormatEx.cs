using mulova.comunity;
using UnityEditor;

namespace mulova.comunity
{
	public static class TextureImporterFormatEx
	{
		public static bool IsMemberOf(this TextureImporterFormat format, TexFormatGroup group)
		{
			return group.IsGroupOf(format);
		}
	}
}

