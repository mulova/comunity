//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons
{
	public abstract class BreadthFirstTraversal<N> {
		public abstract N GetChild(N parent, int childIndex);
		public abstract int GetChildCount(N parent);
		/**
		 * @param node
		 * @return false if there is no need to process any further
		 * @author mulova
		 */
		protected abstract bool Process(N node);
		
		public void Traverse(N root) {
			LinkedList<N> queue = new LinkedList<N>();
			queue.AddLast(root);
			while (queue.Count > 0) {
				N current = queue.First.Value;
				queue.RemoveFirst();
				if (Process(current) == false) {
					return;
				}
				for (int i=0; i<GetChildCount(current); i++) {
					queue.AddLast(GetChild(current, i));
				}
			}
		}
	}
}

