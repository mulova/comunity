#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace comunity {
	public class doublec
	{
		private byte[] encStore;
		private byte[] decStore;
		private int encLength;
		private double val;
		
		public doublec(double val) {
			Value = val;
		}
		
		public static implicit operator doublec(double val) {
			return new doublec(val);
		}
		
		public static implicit operator double(doublec d) {
			return d.Value;
		}

		public double CacheValue {
			get { return val; }
		}

		public double Value {
			get {
#if AES
				if (AES.inst.enabled) {
					if (encStore == null) {
						return 0;
					}
					return AES.inst.DecryptDouble(encStore, encLength, decStore);
				}
#endif
				return val;
			}
			
			set {
#if AES
				if (AES.inst.enabled) {
					if (encStore == null) {
						encStore = new byte[32];
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
