using UnityEngine;
using System;

public static class TextureEx
{
	public static bool IsPOT(this Texture tex)
	{
		return Mathf.IsPowerOfTwo(tex.width) && Mathf.IsPowerOfTwo(tex.height);
	}
}
