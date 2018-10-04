//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using commons;

namespace commons
{
	public static class ArrayEx {
		public static Loggerx log = LogManager.GetLogger(typeof(ArrayEx));
		public static Random rand = new Random();
		
		public static string[] ToStringArr<T>(this T[] items) {
			string[] str = new string[items.Length];
			for (int i=0; i<items.Length; i++) {
				str[i] = items[i].ToString();
			}
			return str;
		}
		
		public static T Get<T>(this T[] arr, int i) {
			if (arr != null && i<arr.Length && i>=0) {
				return arr[i];
			}
			return default(T);
		}
		
		//Fisher-Yates shuffle
		public static void Shuffle<T>(this T[] arr) {
			int n = arr.Length;
			while (n-- > 1) {
				int k = rand.Next(n+1);
				T value = arr [k];
				arr [k] = arr [n];
				arr [n] = value;
			}
		}
		
		public static void Shift<T>(T[] arr, int shift) {
			if (shift > 0) {
				shift = Math.Min(shift, arr.Length);
				for (int i = arr.Length-1-shift; i >= 0; i--) {
					arr[i+shift] = arr[i];
				}
				for (int i = 0; i < shift; i++) {
					arr[i] = default(T);
				}
			} else {
				shift = Math.Max(shift, -arr.Length);
				for (int i = 0; i < arr.Length+shift; i++) {
					arr[i] = arr[i-shift];
				}
				for (int i = arr.Length+shift; i < arr.Length; i++) {
					arr[i] = default(T);
				}
			}
		}
		
		public static T[] CloneEx<T>(this T[] src) where T:ICloneable {
			T[] dst = new T[src.Length];
			for (int i=0; i<src.Length; ++i) {
				if (src[i] != null) {
					dst[i] = (T)src[i].Clone();
				}
			}
			return dst;
		}
		
		public static T[] Add<T>(this T[] arr, T t) {
			if (arr == null) {
				arr = new T[0];
			}
			Array.Resize<T>(ref arr, arr.Length+1);
			arr[arr.Length-1] = t;
			return arr;
		}
		
		public static T[] Insert<T>(this T[] arr, int index, params T[] t) {
			T[] newArr = new T[arr.Length+t.Length];
			if (index > 0) {
				Array.Copy (arr, 0, newArr, 0, index);
			}
			Array.Copy (t, 0, newArr, index, t.Length);
			if (index < arr.Length) {
				Array.Copy (arr, index, newArr, index+t.Length, arr.Length-index);
			}
			return newArr;
		}
		
		public static D[] Convert<S, D>(this S[] src) where D:S {
			if (src == null) {
				return null;
			}
			D[] dst = new D[src.Length];
			for (int i=0; i<src.Length; ++i) {
				dst[i] = (D)src[i];
			}
			return dst;
		}
		
		public static D[] Convert<S, D>(this S[] src, Converter<S, D> converter) {
			if (src == null) {
				return null;
			}
			D[] dst = new D[src.Length];
			for (int i=0; i<src.Length; ++i) {
				dst[i] = converter(src[i]);
			}
			return dst;
		}
		
		public static bool Contains<T>(this T[] arr, T o) {
			if (arr == null) {
				return false;
			}
			int index = Array.IndexOf(arr, o);
			return index >= 0;
		}
		
		public static T[] Remove<T>(this T[] arr, T o) {
			int index = Array.IndexOf(arr, o);
			if (index >= 0) {
				return Remove(arr, index);
			}
			return arr;
		}
		
		public static T[] Remove<T>(this T[] arr, int index) {
			return arr.Remove(index, 1);
		}
		
		public static T[] Remove<T>(this T[] arr, int index, int count) {
			T[] newArr = new T[arr.Length-count];
			if (index > 0) {
				Array.Copy (arr, 0, newArr, 0, index);
			}
			if (index < arr.Length-count) {
				Array.Copy (arr, index+count, newArr, index, arr.Length-index-count);
			}
			return newArr;
		}
		
		public static bool IsEmpty<T>(this T[] arr) {
			return arr == null || arr.Length == 0;
		}
		
		public static bool IsNotEmpty<T>(this T[] arr) {
			return !arr.IsEmpty();
		}
		
		public static T Rand<T>(this T[] arr) {
			return arr[rand.Next(arr.Length)];
		}
		
		public static void ForEach<T>(this T[] arr, Action<T> action) {
			if (arr == null || action == null) {
				return;
			}
			foreach (T t in arr) {
				if (t != null) {
					action(t);
				}
			}
		}

		public static void ForEachIndex<T>(this T[] arr, Action<T, int> action)
		{
			if (arr == null || action == null)
			{
				return;
			}
			for (int i=0; i<arr.Length; ++i)
			{
				if (arr[i] != null)
				{
					action(arr[i], i);
				}
			}
		}

		public static void Fill<T>(this T[] arr, T val) {
			if (arr == null) {
				return;
			}
			for (int i=0; i<arr.Length; ++i) {
				arr[i] = val;
			}
		}
		
		public static int FindIndex<T>(this T[] arr, T val) {
			if (arr != null) {
				for (int i=0; i<arr.Length; ++i) {
					if (arr[i].EqualsEx(val)) {
						return i;
					}
				}
			}
			return -1;
		}
		
		public static int FindIndex<T>(this T[] arr, Predicate<T> predicate) {
			if (arr != null) {
				for (int idx = 0; idx < arr.Length; ++idx) {
					if (arr[idx] != null && predicate (arr [idx])) {
						return idx;
					}
				}
			}
			return -1;
		}
		
		public static T[] Resize<T>(this T[] arr, int size) {
			if (arr == null)
			{
				return new T[size];
			} else 
			{
				if (arr.Length < size) {
					T[] newArr = new T[size];
					Array.Copy(arr, newArr, arr.Length);
					return newArr;
				} else {
					for (int i=arr.Length; i < size; ++i)
					{
						arr[i] = default(T);
					}
					return arr;
				}
			}
		}
	}
}
