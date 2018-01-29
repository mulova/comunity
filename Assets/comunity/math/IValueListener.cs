//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace math.ex {
	public interface IValueListener
	{
		void TurnOver(bool end, float value);
		
		void Init();
		
		void ValueChanged(float value);
	}
}