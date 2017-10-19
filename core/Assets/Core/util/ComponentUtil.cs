//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace core {
	public class ComponentUtil {
		
		/**
		 * @return enabled component. null if there is no enabled component.
		 */
		public static T GetEnabledComponent<T>(GameObject obj) where T:Behaviour {
			foreach (T t in obj.GetComponents<T>()) {
				if (t.enabled) {
					return t;
				}
			}
			return null;
		}
		
		public static void Copy(Component src, Component dst, bool copyProperty) {
			ObjCopy store = new ObjCopy(copyProperty);
			store.ExcludeType("UnityEngine.Component", "UnityEngine.Object");
			store.SetValue(src, dst);
		}
		
		/**
		 * 한쪽에 있는 모든 component를 반대쪽으로 복사한다.
		 */
		public static void Copy(GameObject src, GameObject dst, bool copyProperty) {
			dst.layer = src.layer;
			dst.tag = src.tag;
			dst.isStatic = src.isStatic;
			dst.SetActive(src.activeSelf);
			ObjCopy copy = new ObjCopy(copyProperty);
			copy.ExcludeType("UnityEngine.Component", "UnityEngine.Object");
			
			Component[] comps = src.GetComponents<Component>();
			List<Component> notAdded = new List<Component>();
			foreach (Component c in comps) {
				Component p = dst.GetComponent(c.GetType());
				if (p == null) {
					p = dst.AddComponent(c.GetType());
				}
				/*  RequireComponent 문제로 null일 수도 있다. */
				if (p != null) {
					copy.SetValue(c, p);
				} else {
					notAdded.Add(c);
				}
			}
			
			for (int loop = 0; loop < 10 && notAdded.Count > 0; loop++) {
				for (int i=notAdded.Count-1; i>=0; i--) {
					Component c = notAdded[i];
					Component p = dst.AddComponent(c.GetType());
					if (p != null) {
						copy.SetValue(c, p);
						notAdded.Remove(c);
					}
				}
			}
		}
	}
}

