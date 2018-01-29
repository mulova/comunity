#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace crypto.ex {
	public class boolc
	{
		private byte[] encStore;
		private byte[] decStore;
		private int encLength;
		private bool val;
		
		public boolc(bool val) {
			Value = val;
		}
		
		public static implicit operator boolc(bool val) {
			return new boolc(val);
		}
		
		public static implicit operator bool(boolc b) {
			return b.Value;
		}

		public bool CacheValue {
			get { return val; }
		}

		public bool Value {
			get {
#if AES
				if (AES.inst.enabled) {
					if (encStore == null) {
						return false;
					}
					return AES.inst.DecryptBool(encStore, encLength, decStore);
				}
#endif
				return val;
			}
			
			set {
#if AES
				if (AES.inst.enabled) {
					if (encStore == null) {
						encStore = new byte[16];
						decStore = new byte[1];
					}
					encLength = AES.inst.Encrypt(value, encStore);
				}
#endif
				val = value;
			}
		}
	}
}

#endif
