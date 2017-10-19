//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace commons {
	/**
	 * Enum 배치나 이름이 바뀔경우 원래의 Enum 값을 찾기 위해 string, ordinal을 저장한다.
	 * Enum 값 대신 Enum 이름과 ordinal을 저장하여, 
	 * 위치가 이동될 경우 Enum 이름으로 원래의 Enum을 찾고, 
	 * 이름이 바뀔경우 ordinal을 사용하여 원래의 Enum을 얻는다.
	 * 위치가 이동되고 이름이 바뀔경우에는 사용할 수 없다.
	 */
	[Serializable]
	public class EnumWrapper
	{
		private Type enumType;
//		[SerializeField]
		private Enum e;
//		[SerializeField]
		private string enumTypeName;
//		[SerializeField]
		private string name;
//		[SerializeField]
		private int ordinal = -1;
		
		public Enum Enum {
			get {
				if (enumType == null || e == null) {
					if (enumTypeName == null) {
						return null;
					} else if (enumType == null) {
						enumType = Type.GetType(enumTypeName);
						if (enumType == null) {
							return null;
						}
					}
					e = ToEnum(name, ordinal);
					ordinal = (int)Enum.Parse(enumType, e.ToString());
				}
				return e;
			}
			set {
				enumType = value.GetType();
				e = value;
				name = value.ToString();
				ordinal = (int)Enum.Parse(value.GetType(), name);
			}
		}
		
		/**
		 * Serialization용. 직접 사용하면 안된다.
		 */
		public EnumWrapper() {}
		
		public EnumWrapper(Type enumType) {
			this.enumType = enumType;
			this.enumTypeName = enumType.FullName;
		}
		
		private Enum ToEnum(string s, int defaultValue) {
			if (s != null && Enum.IsDefined(enumType, s)) {
				return (Enum)Enum.Parse(enumType, s);
			}
			Enum e = (Enum)Enum.ToObject(enumType, defaultValue);
			if (Enum.IsDefined(enumType, e) == false) {
				e = (Enum)Enum.GetValues(enumType).GetValue(0);
			}
			return e;
		}
		
		public EnumWrapper Clone() {
			return (EnumWrapper)MemberwiseClone();
		}
	}
}