//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System;



namespace comunity {
	public class Threading : SingletonBehaviour<Threading> {
		private ActionQueue queue = new ActionQueue();

		void Update() {
			queue.Update ();
		}

		public Coroutine Delay(float delay, Action callback) 
		{
			if (delay <= 0)
			{
				callback();
				return null;
			} else {
				return StartCoroutine(DelayCo(delay, callback));
			}
		}

		private IEnumerator DelayCo(float delay, Action callback)
		{
			yield return new WaitForSeconds(delay);
			callback();
		}

		public static void InvokeLater(Action action) {
			if (action != null) {
				inst.queue.Add(action);
			}
		}
	}
}

