#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace math.ex {
	[System.Serializable]
	public class Curve {
		public float min = 0;
		public float max = 1;
		public float time = 1;
		public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1, 1);
		
		
		public float GetValue(float x) {
			return min + curve.Evaluate(x/time)*(max-min);
		}
		
	}
	
}
#endif