using System.IO;
using UnityEngine;

namespace mulova.comunity
{
	public struct TexData
	{
        public bool mipmap;
        public TextureWrapMode wrapMode;
        public FilterMode filterMode;
        public bool linear;
        public bool crunch;

        public void Read(Stream s)
        {
            byte bits = (byte)s.ReadByte();
            mipmap = (bits & 0x01) != 0;
            linear = (bits & 0x02) != 0;
            crunch = (bits & 0x04) != 0;
            wrapMode = (TextureWrapMode)s.ReadByte();
            filterMode = (FilterMode)s.ReadByte();
        }

        public void Write(Stream s)
        {
            byte bits = 0;
            if (mipmap) { bits |= 0x01; }
            if (linear) { bits |= 0x02; }
            if (crunch) { bits |= 0x04; }
            s.WriteByte(bits);
            s.WriteByte((byte)wrapMode);
            s.WriteByte((byte)filterMode);
        }

        public static string GetPath(string texPath)
        {
            return texPath+".txd";
        }

        public static TexData Load(string texPath)
        {
            TexData data = new TexData();
#if !UNITY_WEBGL
            string path = GetPath(texPath);
            if (File.Exists(path))
            {
                Stream s = new FileStream(path, FileMode.Open, FileAccess.Read) ;
                data.Read(s);
                s.Close();
            } else
            {
                // default value
                data.mipmap = false;
                data.filterMode = FilterMode.Bilinear;
                data.wrapMode = TextureWrapMode.Clamp;
                data.linear = false;
                data.crunch = false;
            }
#endif
            return data;
        }

        public static void Save(string texPath, TexData data)
        {
#if !UNITY_WEBGL
            string path = GetPath(texPath);
            Stream s = new FileStream(path, FileMode.Create, FileAccess.Write);
            data.Write(s);
            s.Close();
#endif
        }
	}
}
