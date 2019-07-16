//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;

namespace comunity {
	public static class MathUtil {
		public static bool IsBetween(float f, float min, float max) {
			return f >= min && f <= max;
		}
		
		public static int Clamp(int v, int min, int max) {
			if (v <= min) {
				return min;
			}
			if (v >= max) {
				return max;
			}
			return v;
		}

		public static long Clamp(long v, long min, long max) {
			if (v <= min) {
				return min;
			}
			if (v >= max) {
				return max;
			}
			return v;
		}
		
		public static double Clamp(double v, double min, double max) {
			if (v <= min) {
				return min;
			}
			if (v >= max) {
				return max;
			}
			return v;
		}
		
		public static float Clamp(float v, float min, float max) {
			if (v <= min) {
				return min;
			}
			if (v >= max) {
				return max;
			}
			return v;
		}
		
		public static double Interpolate(double interpolate, double min, double max) {
			double diff = max-min;
			double inter = diff*interpolate;
			return min+inter;
		}
		
		public static float Interpolate(float interpolate, float min, float max) {
			float diff = max-min;
			float inter = diff*interpolate;
			return min+inter;
		}
		
		/**
		 * @digit 소수점 자리수
		 */
		public static double Round(double d, int digit) {
			double pow = Math.Pow(10, digit);
			return Math.Round(d*pow)/pow;
		}
		
		public static double ToDouble(ValueType val) {
			if (val is float) {
				return (double)(float)val;
			} else if (val is int) {
				return (double)(int)val;
			}
			return (double)val;
		}
		
		public static T Random<T>(params T[] arr) {
			return arr[UnityEngine.Random.Range(0, arr.Length)];
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
			return Interpolate(Clamp((value - min1) / (max1 - min1), 0, 1), min2, max2);
		}

		public static float Interpolate(float value, float min1, float max1, float min2, float max2) {
			return Interpolate(Clamp((value - min1) / (max1 - min1), 0, 1), min2, max2);
		}

        public static bool ApproximatelyEquals(float f1, float f2)
        {
            float d = f1-f2;
            return d <= Mathf.Epsilon && d >= -Mathf.Epsilon;
        }
	}
}
