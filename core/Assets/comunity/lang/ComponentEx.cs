//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using UnityEngine;
using Object = UnityEngine.Object;

using System.Collections.Generic;
using System;


namespace UnityEngine
{
	public static class ComponentEx
	{
		public static T GetComponentEx<T>(this Component c) where T:Component
		{
			if (c == null) {
				return null;
			}
			return c.gameObject.GetComponentEx<T>();
		}

		/// <summary>
		/// Include inactive
		/// </summary>
		/// <returns>The component in children.</returns>
		/// <param name="c">C.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetComponentInChildrenEx<T>(this Component c) where T:Component
		{
			return c.gameObject.GetComponentInChildrenEx<T>();
		}

		public static void SetActiveEx(this Component c, bool active) {
			if (c != null) {
				c.gameObject.SetActive(active);
			}
		}
	}
}


