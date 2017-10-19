#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace crypto.ex {
	public class longc
	{
		private byte[] encStore;
		private byte[] decStore;
		private int encLength;
		private long val;
		
		public longc(long val) {
			Value = val;
		}
		
		public static implicit operator longc(long val) {
			return new longc(val);
		}
		
		public static implicit operator long(longc l) {
			return l.Value;
		}

		public long CacheValue {
			get { return val; }
		}

		public long Value {
			get {
#if AES
				if (AES.inst.enabled) {
					if (encStore == null) {
						return 0L;
					}
					return AES.inst.DecryptLong(encStore, encLength, decStore);
				}
#endif
				return val;
			}
			
			set {
#if AES
				if (AES.inst.enabled) {
					if (encStore == null) {
						encStore = new byte[128];
						decStore = new byte[8];
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