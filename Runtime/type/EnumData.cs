//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace mulova.commons {
	/**
	 * Enum index를 기반으로 하는 배열을 저장할 경우 사용하는 base class.
	 * EnumDataInspector와 함께 사용된다.
	 */
	[System.Serializable]
	public class EnumData
	{
//		[SerializeField]
		private EnumWrapper id = new EnumWrapper();
		
		/**
		 * Serialization용. 직접 호출 금지.
		 */
		public EnumData () { }
		
		/**
		 * @param e enum id
		 */
		public EnumData(Enum e) {
			id = new EnumWrapper(e.GetType());
			Id = e;
		}
		
		public override sealed string ToString() {
			Enum e = Id;
			if (e == null) {
				return "";
			}
			return e.ToString();
		}
		
		public Enum Id {
			get {
				return id.Enum;
			}
			set {
				id.Enum = value;
			}
		}
	}
}