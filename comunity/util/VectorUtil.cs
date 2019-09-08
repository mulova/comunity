//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;
using mulova.commons;

namespace mulova.comunity {
	
	/**
	 * @author mulova
	 */
	public class VectorUtil {
		private const string REGEX_DOUBLE=@"[+-]?(?:\.[0-9]+|[0-9]+(?:\.[0-9]*)?)";
		private const string REGEX_VECTOR3=@"(?<x>"+REGEX_DOUBLE+@")\,\s*"
			+"(?<y>"+REGEX_DOUBLE+@")\,\s*"
				+"(?<z>"+REGEX_DOUBLE+@")";
		
		//	private static readonly Regex rxDouble = new Regex(REGEX_DOUBLE, RegexOptions.Compiled);
		//	private static readonly Regex rxVector3 = new Regex(REGEX_VECTOR3, RegexOptions.Compiled);
		
		public static Vector3 ToVector3(string str) {
			Match m = Regex.Match(str, REGEX_VECTOR3);
			//		Match m = rxVector3.Match(str);
			Vector3 v = new Vector3(float.NaN, float.NaN, float.NaN);
			if (m.Success) {
				v.x = (float)Convert.ToDouble(m.Groups["x"].Value);
				v.y = (float)Convert.ToDouble(m.Groups["y"].Value);
				v.z = (float)Convert.ToDouble(m.Groups["z"].Value);
			}
			return v;
		}
		
		public static Vector3 CalculateNormal(Vector3 v1, Vector3 v2, Vector3 v3) {
			Vector3 normal = Vector3.Cross(v2-v1, v3-v1);
			normal.Normalize();
			return normal;
		}
		
		/**
		 * @param v1
		 * @param v2
		 * @return
		 */
		public static bool Equals(Vector3 v1, Vector3 v2, Axis axis) {
			if (v1 == v2)
				return true;
			
			bool x = Mathf.Approximately(v1.x, v2.x);
			bool y = Mathf.Approximately(v1.y, v2.y);
			bool z = Mathf.Approximately(v1.z, v2.z);
			if (axis == Axis.NULL)
			{
				return x && y && z;
			}
			
			switch (axis) {
			case Axis.X:
				return y && z;
			case Axis.Y:
				return z && x;
			case Axis.Z:
				return x && y;
			}
			return false;
		}
		
		/**
		 * @param end
		 * @param start
		 * @param axis
		 * @param store
		 * @return
		 */
		public static Vector3 Diff(Vector3 end, Vector3 start, Axis axis) {
			return GetAxisOrthogonal(axis, end - start);
		}
		
		public static float DistanceSq(Vector3 end, Vector3 start, Axis axis) {
			return GetAxisOrthogonal(axis, end - start).sqrMagnitude;
		}
		
		public static float Distance(Vector3 end, Vector3 start, Axis axis) {
			float squared = DistanceSq(end, start, axis);
			return Mathf.Sqrt(squared);
		}
		
		public static Vector3 GetAxisOrthogonal(Axis axis, Vector3 v) {
			switch (axis) {
			case Axis.X:
				v.x = 0;
				break;
			case Axis.Y:
				v.y = 0;
				break;
			case Axis.Z:
				v.z = 0;
				break;
			}
			return v;
		}
		
		public static float GetAxisAligned(Axis axis, Vector3 v) {
			if (axis == Axis.NULL)
				return 0;
			switch (axis) {
			case Axis.X:
				return v.x;
			case Axis.Y:
				return v.y;
			case Axis.Z:
				return v.z;
			}
			return 0;
		}
		
		/**
		 * 
		 * @param axis
		 * @param store	null �̸� �⺻ unit vector�� ��ȯ�Ѵ�.
		 * @return store != null �̸� store, �ƴϸ� unit vector instance
		 */
		public static Vector3 ToUnitVector(Axis axis) {
			switch (axis) {
			case Axis.X:
				return Vector3.right;
			case Axis.Y:
				return Vector3.up;
			case Axis.Z:
				return Vector3.forward;
			}
			return Vector3.zero;
		}
		
		public static Vector3 Divide(Vector3 dividend, Vector3 divisor) {
			Vector3 result = new Vector3();
			result.x = Divide(dividend.x, divisor.x);
			result.y = Divide(dividend.y, divisor.y);
			result.z = Divide(dividend.z, divisor.z);
			return result;
		}

		private static float Divide(float dividend, float divisor) {
			if (divisor == 0) {
				return dividend > 0 ? float.PositiveInfinity: float.NegativeInfinity;
			}
			return dividend / divisor;
		}
	}
	
}