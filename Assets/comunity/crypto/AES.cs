#if AES
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using mulova.commons;

namespace comunity
{
	public class AES : IDisposable
	{
		public static readonly Loggerx log = LogManager.GetLogger(typeof(AES));
		public bool enabled = true;
		private UnicodeEncoding converter = new UnicodeEncoding();
		private static readonly AES instance = new AES();
		
		public static AES Instance { get { return instance; } }
		
		private AesManaged crypto;
		private AesManaged Crpyto {
			get {
				return crypto;
			}
		}
		
		public AES() {
			crypto = new AesManaged();
			crypto.KeySize = GetMinSize(crypto.LegalKeySizes);
			crypto.BlockSize = GetMinSize(crypto.LegalBlockSizes);
		}
		
		private int GetMinSize(KeySizes[] keySizes) {
			if (keySizes == null || keySizes.Length == 0) {
				return 128;
			}
			int min = int.MaxValue;
			foreach (KeySizes k in keySizes) {
				min = Math.Min(min, k.MinSize);
			}
			return min;
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
		
		public int Encrypt(float src, byte[] enc) {
			return Encrypt(BitConverter.GetBytes(src), enc);
		}
		
		public float DecryptFloat(byte[] src, int srcLength, byte[] dst) {
			Decrypt(src, srcLength, dst);
			return BitConverter.ToSingle(dst, 0);
		}
		
		public int Encrypt(int src, byte[] enc) {
			return Encrypt(BitConverter.GetBytes(src), enc);
		}
		
		public int DecryptInt(byte[] src, int srcLength, byte[] dst) {
			Decrypt(src, srcLength, dst);
			return BitConverter.ToInt32(dst, 0);
		}
		
		public int Encrypt(double src, byte[] enc) {
			return Encrypt(BitConverter.GetBytes(src), enc);
		}
		
		public long DecryptLong(byte[] src, int srcLength, byte[] dst) {
			Decrypt(src, srcLength, dst);
			return BitConverter.ToInt64(dst, 0);
		}
		
		public int Encrypt(long src, byte[] enc) {
			return Encrypt(BitConverter.GetBytes(src), enc);
		}
		
		public double DecryptDouble(byte[] src, int srcLength, byte[] dst) {
			Decrypt(src, srcLength, dst);
			return BitConverter.ToDouble(dst, 0);
		}
		
		public int Encrypt(bool src, byte[] enc) {
			return Encrypt(BitConverter.GetBytes(src), enc);
		}
		
		public bool DecryptBool(byte[] src, int srcLength, byte[] dst) {
			Decrypt(src, srcLength, dst);
			return BitConverter.ToBoolean(dst, 0);
		}
		
		public byte[] Encrypt(string src) {
			return Encrypt(converter.GetBytes(src.ToCharArray()));
		}
		
		public string DecryptString(byte[] src) {
			return converter.GetString(Decrypt(src));
		}
		
		public int Encrypt(byte[] src, byte[] dst) {
			return Encrypt(src, src.Length, dst);
		}
		
		public byte[] Encrypt(byte[] src)
		{
			try
			{
				
				MemoryStream dst = new MemoryStream();
				// Create the streams used for decryption. 
				ICryptoTransform encryptor = crypto.CreateEncryptor(crypto.Key, crypto.IV);
				using (MemoryStream srcBuf = new MemoryStream(src)) {
					using (CryptoStream csEncrypt = new CryptoStream(srcBuf, encryptor, CryptoStreamMode.Read))
					{
						csEncrypt.CopyTo(dst);
					}
					return dst.ToArray();
				}
			}
			catch (Exception e)
			{
				log.Error(e, e.Message);
				return null;
			}
		}
		
		/// <summary>
		/// encrypt src[0] ~ src[srcLength-1] and store at dst
		/// returns encrypted byte array length
		/// </summary>
		/// <param name="src">Source.</param>
		/// <param name="srcLength">Source array length.</param>
		/// <param name="dst">encrypted byte array</param>
		public int Encrypt(byte[] src, int srcLength, byte[] dst)
		{
			int dstLength = 0;
			try
			{
				// Create the streams used for decryption. 
				ICryptoTransform encryptor = crypto.CreateEncryptor(crypto.Key, crypto.IV);
				using (MemoryStream srcBuf = new MemoryStream(src, 0, srcLength)) {
					using (CryptoStream csEncrypt = new CryptoStream(srcBuf, encryptor, CryptoStreamMode.Read))
					{
						dstLength += csEncrypt.Read(dst, 0, dst.Length);
					}
				}
			}
			catch (Exception e)
			{
				log.Error(e, e.Message);
			}
			return dstLength;
		}
		
		public byte[] Decrypt(byte[] src)
		{
			try
			{
				MemoryStream dst = new MemoryStream(src.Length*4);
				// Create the streams used for decryption. 
				ICryptoTransform decryptor = crypto.CreateDecryptor(crypto.Key, crypto.IV);
				using (MemoryStream srcBuf = new MemoryStream(src)) {
					using (CryptoStream csEncrypt = new CryptoStream(srcBuf, decryptor, CryptoStreamMode.Read))
					{
						csEncrypt.CopyTo(dst);
					}
					return dst.ToArray();
				}
			}
			catch (Exception e)
			{
				log.Error(e, e.Message);
				return null;
			}
		}
		
		public int Decrypt(byte[] src, int srcLength, byte[] dst)
		{
			try
			{
				int dstLength = 0;
				ICryptoTransform decryptor = crypto.CreateDecryptor(crypto.Key, crypto.IV);
				using (MemoryStream srcBuf = new MemoryStream(src, 0, srcLength)) {
					using (CryptoStream csDecrypt = new CryptoStream(srcBuf, decryptor, CryptoStreamMode.Read))
					{
						dstLength += csDecrypt.Read(dst, 0, dst.Length);
					}
				}
				return dstLength;
			}
			catch (Exception e)
			{
				log.Error(e, e.Message);
				return 0;
			}
		}
	}
	
}
#endif