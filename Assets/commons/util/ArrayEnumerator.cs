//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace commons {
	/**
	 * Null을 제외하고 enumeration
	 */
	public class ArrayEnumerator<T> : IEnumerator<T> {
		private int i = -1;
		T[] arr;
		
		public ArrayEnumerator(T[] arr) {
			this.arr = arr;
		}
		
		T IEnumerator<T>.Current {
			get {
				return arr[i];
			}
		}
		
		bool System.Collections.IEnumerator.MoveNext ()
		{
			i++;
			while (i < arr.Length && arr[i] == null) {
				i++;
			}
			return i < arr.Length;
		}
		
		void System.Collections.IEnumerator.Reset ()
		{
			i=-1;
		}
		
		object IEnumerator.Current {
			get {
				return arr[i];
			}
		}
		
		void IDisposable.Dispose () {
		}
	}
}