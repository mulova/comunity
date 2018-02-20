#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

using System.IO;

namespace comunity {
	public class Rijndael : IDisposable
	{
		public static readonly Loggerx log = LogManager.GetLogger(typeof(Rijndael));
		private UnicodeEncoding converter = new UnicodeEncoding();
		private RijndaelManaged crypto;
		private static readonly Rijndael instance = new Rijndael();
		
		public static Rijndael Instance { get { return instance; } }
		
		private RijndaelManaged Crpyto {
			get {
				return crypto;
			}
		}
		
		public Rijndael() {
			crypto = new RijndaelManaged();
		}
		
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(Boolean disposing) {
			if (crypto == null) {
				return;
			}
			crypto.Clear();
			crypto = null;
		}
		
		public byte[] Encrypt(float src) {
			return Encrypt(BitConverter.GetBytes(src));
		}
		
		public float DecryptFloat(byte[] src) {
			int length = 0;
			return BitConverter.ToSingle(Decrypt(src, out length), 0);
		}
		
		public byte[] Encrypt(int src) {
			return Encrypt(BitConverter.GetBytes(src));
		}
		
		public int DecryptInt(byte[] src) {
			int length = 0;
			return BitConverter.ToInt32(Decrypt(src, out length), 0);
		}
		
		public byte[] Encrypt(double src) {
			return Encrypt(BitConverter.GetBytes(src));
		}
		
		public long DecryptLong(byte[] src) {
			int length = 0;
			return BitConverter.ToInt64(Decrypt(src, out length), 0);
		}
		
		public byte[] Encrypt(long src) {
			return Encrypt(BitConverter.GetBytes(src));
		}
		
		public double DecryptDouble(byte[] src) {
			int length = 0;
			return BitConverter.ToDouble(Decrypt(src, out length), 0);
		}
		
		public byte[] Encrypt(bool src) {
			return Encrypt(BitConverter.GetBytes(src));
		}
		
		public bool DecryptBool(byte[] src) {
			int length = 0;
			return BitConverter.ToBoolean(Decrypt(src, out length), 0);
		}
		
		public byte[] Encrypt(string src) {
			return Encrypt(converter.GetBytes(src.ToCharArray()));
		}
		
		public string DecryptString(byte[] src) {
			int length = 0;
			return converter.GetString(Decrypt(src, out length), 0, length);
		}
		
		public byte[] Encrypt(byte[] src)
		{
			try
			{
				// Create the streams used for decryption. 
				ICryptoTransform encryptor = crypto.CreateEncryptor(crypto.Key, crypto.IV);
				using (MemoryStream encryptBuf = new MemoryStream()) {
					using (CryptoStream csEncrypt = new CryptoStream(encryptBuf, encryptor, CryptoStreamMode.Write))
					{
						csEncrypt.Write(src, 0, src.Length);
						csEncrypt.FlushFinalBlock();
						csEncrypt.Close();
					}
					encryptBuf.Close();
					return encryptBuf.ToArray();
				}
			}
			catch (Exception e)
			{
				log.Error(e);
				return null;
			}
		}
		
		private byte[] buf = new byte[1024];
		public byte[] Decrypt(byte[] src, out int length)
		{
			try
			{
				ICryptoTransform decryptor = crypto.CreateDecryptor(crypto.Key, crypto.IV);
				using (MemoryStream srcStream = new MemoryStream(src)) {
					using (CryptoStream csDecrypt = new CryptoStream(srcStream, decryptor, CryptoStreamMode.Read))
					{
						length = csDecrypt.Read(buf, 0, buf.Length);
					}
					if (length == buf.Length) {
						throw new InvalidOperationException();
					}
				}
				return buf;
			}
			catch (Exception e)
			{
				log.Error(e);
				length = 0;
				return null;
			}
		}
	}
}

#endif