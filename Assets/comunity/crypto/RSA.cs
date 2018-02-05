#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;


namespace comunity {
	public class RSA : IDisposable
	{
		public static readonly Loggerx log = LogManager.GetLogger(typeof(RSA));
		private UnicodeEncoding converter = new UnicodeEncoding();
		private RSACryptoServiceProvider rsa;
		private RSAParameters encKey;
		private RSAParameters decKey;
		private static readonly RSA instance = new RSA();
		
		public static RSA Instance { get { return instance; } }
		
		private RSACryptoServiceProvider Rsa {
			get {
				if (rsa == null) {
					rsa = new RSACryptoServiceProvider();
					encKey = Rsa.ExportParameters(false);
					decKey = Rsa.ExportParameters(true);
				}
				return rsa;
			}
		}
		
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(Boolean disposing) {
			if (rsa == null) {
				return;
			}
			//		rsa.Dispose(); // .Net 4.5
			rsa = null;
		}
		
		public byte[] Encrypt(float src) {
			return Encrypt(BitConverter.GetBytes(src));
		}
		
		public float DecryptFloat(byte[] src) {
			return BitConverter.ToSingle(Decrypt(src), 0);
		}
		
		public byte[] Encrypt(int src) {
			return Encrypt(BitConverter.GetBytes(src));
		}
		
		public int DecryptInt(byte[] src) {
			return BitConverter.ToInt32(Decrypt(src), 0);
		}
		
		public byte[] Encrypt(double src) {
			return Encrypt(BitConverter.GetBytes(src));
		}
		
		public long DecryptLong(byte[] src) {
			return BitConverter.ToInt64(Decrypt(src), 0);
		}
		
		public byte[] Encrypt(long src) {
			return Encrypt(BitConverter.GetBytes(src));
		}
		
		public double DecryptDouble(byte[] src) {
			return BitConverter.ToDouble(Decrypt(src), 0);
		}
		
		public byte[] Encrypt(bool src) {
			return Encrypt(BitConverter.GetBytes(src));
		}
		
		public bool DecryptBool(byte[] src) {
			return BitConverter.ToBoolean(Decrypt(src), 0);
		}
		
		public byte[] Encrypt(string src) {
			return Encrypt(converter.GetBytes(src.ToCharArray()));
		}
		
		public string DecryptString(byte[] src) {
			return converter.GetString(Decrypt(src));
		}
		
		public byte[] Encrypt(byte[] src, bool doOAEPPadding = false)
		{
			try
			{
				//Import the RSA Key information. This only needs 
				//toinclude the public key information.
				Rsa.ImportParameters(encKey);
				
				//Encrypt the passed byte array and specify OAEP padding.   
				//OAEP padding is only available on Microsoft Windows XP or 
				//later.  
				return Rsa.Encrypt(src, doOAEPPadding);
			}
			//Catch and display a CryptographicException   
			//to the console. 
			catch (CryptographicException e)
			{
				log.Error(e);
				return null;
			}
			
		}
		
		public byte[] Decrypt(byte[] src, bool doOAEPPadding = false)
		{
			try
			{
				//Import the RSA Key information. This needs 
				//to include the private key information.
				Rsa.ImportParameters(decKey);
				
				//Decrypt the passed byte array and specify OAEP padding.   
				//OAEP padding is only available on Microsoft Windows XP or 
				//later.  
				return Rsa.Decrypt(src, doOAEPPadding);
			}
			//Catch and display a CryptographicException   
			//to the console. 
			catch (CryptographicException e)
			{
				log.Error(e);
				return null;
			}
		}
	}
}

#endif