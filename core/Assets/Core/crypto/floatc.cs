#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace crypto.ex {
	public class floatc
	{
		private byte[] encStore;
		private byte[] decStore;
		private int encLength;
		private float val;
		
		public floatc(float val) {
			Value = val;
		}
		
		public static implicit operator floatc(float val) {
			return new floatc(val);
		}
		
		public static implicit operator float(floatc f) {
			return f.Value;
		}

		public float Value {
			get {
#if AES
				if (AES.inst.enabled) {
					if (encStore == null) {
						return 0;
					}
					return AES.inst.DecryptFloat(encStore, encLength, decStore);
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