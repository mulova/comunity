#if OLD_INPUT
using System;
using commons;


namespace comunity {
	[System.Serializable]
	public class InputEventMapping
	{
		public EnumWrapper axis = new EnumWrapper(typeof(InputAxis));
		public EnumWrapper evt = new EnumWrapper(typeof(InputEvent));
		public EnumWrapper trigger = new EnumWrapper(typeof(InputAxisState));
	}
}

#endif