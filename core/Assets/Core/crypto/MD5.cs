
using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using System;
using Random = UnityEngine.Random;
using System.IO;

namespace crypto.ex {
	public static class MD5
	{
		public static string Digest(string text) {
			UTF8Encoding ue = new UTF8Encoding();
			byte[] bytes = ue.GetBytes(text);
			MemoryStream stream = new MemoryStream(bytes);
			return Digest(stream);
		}
		
		public static string Digest(Stream stream) {
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] data = md5.ComputeHash(stream);
			stream.Close();

			StringBuilder sBuilder = new StringBuilder(data.Length*2+1);
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
		}
	}
}
