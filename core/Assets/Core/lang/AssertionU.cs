//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;
using System.Diagnostics;
using Object = UnityEngine.Object;
using commons;

namespace core
{
	public static class AssertionU
	{

		public static void AssertEquals(Vector2 f1, Vector2 f2)
		{
			if (!Assert.IsEnabled())
			{
				return;
			}
            Assert.AreEqual(f1.x, f2.x, Mathf.Epsilon);
            Assert.AreEqual(f1.y, f2.y, Mathf.Epsilon);
		}

		
		public static void AssertEquals(Vector3 f1, Vector3 f2)
		{
			if (!Assert.IsEnabled())
			{
				return;
			}
            Assert.AreEqual(f1.x, f2.x, Mathf.Epsilon);
            Assert.AreEqual(f1.y, f2.y, Mathf.Epsilon);
            Assert.AreEqual(f1.z, f2.z, Mathf.Epsilon);
		}

		
		public static void AssertEquals(double d1, double d2)
		{
			if (!Assert.IsEnabled())
			{
				return;
			}
            Assert.AreEqual(d1, d2, Mathf.Epsilon);
		}
	}
}

