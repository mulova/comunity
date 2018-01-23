using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization;
using commons;

namespace core {
	[Serializable]
	public class InputAxisMapData : EnumData {
		public EnumWrapper[] axis = new EnumWrapper[0];
		public EnumWrapper[] trigger = new EnumWrapper[0];
		public EnumWrapper[] evt = new EnumWrapper[0];
		
		public InputState State {
			get { return (InputState)Id; }
			set { Id = value; }
		}
		
		public int Size {
			get { return axis.Length; }
		}
		
		public InputAxisMapData() : base () { }
		
		public InputAxisMapData(Enum e) : base (e) { }
		
		public InputAxis GetAxis(int i) {
			return (InputAxis)axis[i].Enum;
		}
		
		public InputEvent GetEvent(int i) {
			return (InputEvent)evt[i].Enum;
		}
		
		public InputAxisState GetTrigger(int i) {
			return (InputAxisState)trigger[i].Enum;
		}
		
		public void Add(InputAxisMapData toAdd) {
			axis = Add(axis, toAdd.axis);
			trigger = Add(trigger, toAdd.trigger);
			evt = Add(evt, toAdd.evt);
		}
		
		private EnumWrapper[] Add(EnumWrapper[] w1, EnumWrapper[] w2) {
			EnumWrapper[] arr = new EnumWrapper[w1.Length+w2.Length];
			for (int i=0; i<w1.Length; i++) {
				arr[i] = w1[i];
			}
			for (int i=0; i<w2.Length; i++) {
				arr[i+w1.Length] = w2[i].Clone();
			}
			return arr;
		}
	}
}
