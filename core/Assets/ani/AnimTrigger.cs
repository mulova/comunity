//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using type.ex;

namespace core {
	public class AnimTrigger : MonoBehaviour
	{	
		public Animator animator;
        public AnimTriggerElement[] map = new AnimTriggerElement[0];
		
		void Start() {
			foreach (AnimTriggerElement s in map) {
				s.hash = Animator.StringToHash(s.val);
			}
		}
		
		void OnTriggerEnter(Collider collider)
		{
			string colliderName = collider.name;
			foreach (AnimTriggerElement s in map) {
				if (s.key == colliderName) {
					animator.SetBool(s.hash, true);	
				}
			}
		}
		
		void OnTriggerExit(Collider collider)
		{
			string colliderName = collider.name;
			foreach (AnimTriggerElement s in map) {
				if (s.key == colliderName) {
					animator.SetBool(s.hash, false);	
				}
			}
		}
	}
}

