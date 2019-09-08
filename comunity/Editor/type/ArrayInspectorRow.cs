//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;

namespace mulova.comunity
{
	
	public interface ArrayInspectorRow
	{
		int MinLength {
			get;
			set;
		}
		
		int Length {
			get;
			set;
		}
		
		object Get(int col, int row);
		
		void AddRow(params object[] item);
		
		object[] GetRow(int row);
		
		object[] GetDefault();
		
		void Set(List<object[]> rows);
		
		void SetPreset(params object[] preset);
		
		/**
		* @return bool 값이 바뀌었으면 true
		*/
		bool OnInspectorGUI(int i);
	}
}
