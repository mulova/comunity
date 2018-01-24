//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;

namespace commons {
	public abstract class DepthFirstTraversal<N> {
		public abstract N GetChild(N parent, int childIndex);
		public abstract int GetChildCount(N parent);
		/**
		 * @param node
		 * @return false if there is no need to process any further
		 * @author mulova
		 */
		protected abstract bool Process(N node);
		
		public void Traverse(N root) {
			Dictionary<N, N> traversed = new Dictionary<N, N>();
			LinkedList<N> queue = new LinkedList<N>();
			queue.AddFirst(root);
			while (queue.Count > 0) {
				N current = queue.First.Value;
				if (traversed.ContainsKey(current)) {
					queue.RemoveFirst();
					if (Process(current) == false) {
						return;
					}
				} else {
					for (int i=GetChildCount(current)-1; i>=0; i--) {
						queue.AddFirst(GetChild(current, i));
					}
				}
				traversed[current] = current;
			}
		}
	}
	
}
