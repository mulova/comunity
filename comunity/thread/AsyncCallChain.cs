//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace mulova.comunity
{
	/// <summary>
	/// Queued Delegate Runner
	/// </summary>
	public class AsyncCallChain : MonoBehaviour
	{
		private Queue<AsyncCall> queue = new Queue<AsyncCall>();
		private AsyncCall current;
		
		public void Enqueue(AsyncCall action) {
			queue.Enqueue(action);
		}
		
		void Update() {
			if (current != null) {
				if (current.IsOver()) {
					current = null;
				}
			}
			if (queue.Count > 0 && current == null) {
				current = queue.Dequeue();
				current.Begin();
			}
		}
	}
}


