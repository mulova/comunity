//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System;

namespace mulova.comunity {
	/**
	 * 지정된 시간후에 ShowType event를 수행한다.
	 */
	[Serializable]
	public class TimerData {
		
		public MethodCall method; // with params
		public Action callback; // without params
		public float duration = 1;
		public bool repeat;
		public bool enabled = true;

		public TimerData() {}

		public TimerData(float duration, Action callback) {
			this.duration = duration;
			this.callback = callback;
		}
	}
}

