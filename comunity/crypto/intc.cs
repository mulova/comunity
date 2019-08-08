#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace comunity {
	public class intc
	{
		private byte[] encStore;
		private byte[] decStore;
		private int encLength;
		private int val;
		
		public intc(int val) {
			Value = val;
		}
		
		public static implicit operator intc(int val) {
			return new intc(val);
		}
		
		public static implicit operator int(intc i) {
			return i.Value;
		}

		public int CacheValue {
			get { return val; }
		}

		public int Value {
			get {
#if AES
				if (AES.inst.enabled) {
					if (encStore == null) {
						return 0;
					}
					return AES.inst.DecryptInt(encStore, encLength, decStore);
				}
#endif
				return val;
			}
			
			set {
#if AES
				if (AES.inst.enabled) {
					if (encStore == null) {
						encStore = new byte[16];
						decStore = new byte[4];
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