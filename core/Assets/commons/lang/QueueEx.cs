//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System.Collections.Generic;
using System;

public static class QueueEx
{
	public static Queue<T> Enqueue<T>(this Queue<T> queue, IEnumerable<T> items) {
		if (items != null) {
			foreach (T i in items) {
				queue.Enqueue(i);
			}
		}
		return queue;
	}
}
