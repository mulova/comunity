//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;

namespace comunity {
	public static class MathUtil {
		
		public static double ToDouble(ValueType val) {
			if (val is float) {
				return (double)(float)val;
			} else if (val is int) {
				return (double)(int)val;
			}
			return (double)val;
		}
		
		/// <summary>
		/// min1~max1 사이의 값인 value를 min2~max2사이의 값으로 변환한다.
		/// </summary>
		/// <returns>The minimum max.</returns>
		/// <param name="value">Value.</param>
		/// <param name="min1">Min1.</param>
		/// <param name="max1">Max1.</param>
		/// <param name="min2">Min2.</param>
		/// <param name="max2">Max2.</param>
		public static double Interpolate(double value, double min1, double max1, double min2, double max2) {
            var val = (value - min1) / (max1 - min1);
            return val.Interpolate(0, 1).Interpolate(min2, max2);
        }

        public static float Interpolate(float value, float min1, float max1, float min2, float max2) {
            var val = (value - min1) / (max1 - min1);
			return val.Interpolate(0, 1).Interpolate(min2, max2);
		}
	}
}
