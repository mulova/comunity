#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace crypto.ex {
	public class stringc
	{
		private byte[] encStore;
		private string val;
		
		public stringc(string val) {
			Value = val;
		}
		
		public static implicit operator stringc(string val) {
			return new stringc(val);
		}
		
		public static implicit operator string(stringc s) {
			return s.Value;
		}

		public string CacheValue {
			get { return val; }
		}
		
		public string Value {
			get {
#if AES
				if (AES.inst.enabled) {
					if (encStore == null) {
						return null;
					}
					return AES.inst.DecryptString(encStore);
				}
#endif
				return val;
			}
			
			set {
#if AES
				if (AES.inst.enabled) {
					encStore = AES.inst.Encrypt(value);
				}
#endif
				val = value;
			}
		}
	}
}
#endif