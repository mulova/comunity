//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace commons {
	public class ArrayUtil {
		public static Loggerx log = LogManager.GetLogger(typeof(ArrayUtil));
		public static T[] Clone<T>(T[] source) {
			if (source == null) {
				return null;
			}
			T[] dest = (T[])Array.CreateInstance(typeof(T), source.Length);
			Array.Copy(source, dest, source.Length);
			return dest;
		}

		public static void Add<T>(ref T[] arr, T val) {
			Array.Resize<T>(ref arr, arr.Length+1);
			arr[arr.Length-1] = val;
		}
		
		/**
		 * arr을 재정렬한다.
		 * arr item의  ToString() 값을 names 배열에 제공된 순서대로 맞추어 정렬하여 반환한다.
		 */
		public static T[] Arrange<T>(T[] arr, string[] names) {
			T[] rows = new T[names.Length];
			Dictionary<string, T> map = new Dictionary<string, T>();
			if (arr != null) {
				foreach (T r in arr) {
					if (r != null) {
						map[r.ToString()] = r;
					}
				}
			}
			for (int i=0; i<names.Length; i++) {
				if (map.ContainsKey(names[i])) {
					rows[i] = map[names[i]];
				}
			}
			return rows;
		}
		
		/**
		 * Enum을 index로 사용하는 array의 크기가 바뀌었을 경우, 즉 Enum이 추가/삭제되었을 경우 array를 재배열한다.
		 * 각 array item의 ToString() 은 enum의 이름과 동일해야 한다.
		 */
		public static bool ResizeEnumIndexedArray<E, D>(ref D[] data) {
			if (data == null) {
				data = new D[0];
				return true;
			}
			E[] enums = (E[])Enum.GetValues(typeof(E));
			int size = 0;
			Dictionary<E, int> indexMap = new Dictionary<E, int>();
			bool changed = false;
			foreach (E e in enums) {
				int i = (int)Enum.Parse(typeof(E), e.ToString());
				indexMap[e] = i;
				if (i >= 0) {
					size++;
					if (changed==false && (i>= data.Length || data[i] == null || data[i].ToString() != e.ToString())) {
						changed = true;
					}
				}
			}
			changed |= data == null || size != data.Length;
			
			if (changed) {
				D[] newData = new D[size];
				/*  기존의 data를 먼저 채워넣는다. */
				foreach (D d in data) {
					try {
						E e =(E)Enum.Parse(typeof(E), d.ToString());
						if (indexMap.ContainsKey(e)) {
							int i = indexMap[e] ;
							newData[i] = d;
						}
					} catch (Exception ex) {
						log.Error(ex, "{0}.{1}", d.GetType().FullName, d);
					}
				}
				
				/*  빈 데이터를 채운다. */
				foreach (E e in enums) {
					int i = indexMap[e];
					if (i >= 0 && newData[i] == null) {
						newData[i] = (D)typeof(D).GetConstructor(new Type[] { typeof(E)}).Invoke(new object[] {e});
					}
				}
				
				data = newData;
			}
			return changed;
		}
		
		public static double[] Linearize(double[][] table) {
			int length = 0;
			for (int i = 0; i < table.Length; i++) {
				length += table[i].Length;
			}
			double[] arr = new double[length];
			int index = 0;
			for (int i = 0; i < table.Length; i++) {
				Array.Copy(table[i], 0, arr, index, table[i].Length);
				index += table[i].Length;
			}
			return arr;
		}
		
		public static int[] Convert(float[] arr) {
			int[] intArr = new int[arr.Length];
			for (int i = 0; i < intArr.Length; i++) {
				intArr[i] = (int)arr[i];
			}
			return intArr;
		}
		
		public static int[] Convert(double[] arr) {
			int[] intArr = new int[arr.Length];
			for (int i = 0; i < intArr.Length; i++) {
				intArr[i] = (int)arr[i];
			}
			return intArr;
		}
		
		public static double[][] createDoubleArray(int row, int col) {
			double[][] arr = new double[row][];
			for (int i=0; i<row; i++) {
				arr[i] = new double[col];
			}
			return arr;
		}
		
		public static double[] ToDouble(float[] f) {
			double[] d = new double[f.Length];
			for (int i = 0; i < d.Length; i++) {
				d[i] = f[i];
			}
			return d;
		}
		
		public static float[] ToFloat(double[] d) {
			float[] f = new float[d.Length];
			for (int i = 0; i < f.Length; i++) {
				f[i] = (float) d[i];
			}
			return f;
		}
		
		public static int[] ToInt(double[] d) {
			int[] arr = new int[d.Length];
			for (int i = 0; i < arr.Length; i++) {
				arr[i] = (int) d[i];
			}
			return arr;
		}
		
		public static double[][] ToDouble(float[][] f) {
			double[][] d = new double[f.Length][];
			for (int i = 0; i < d.Length; i++) {
				d[i] = ToDouble(f[i]);
			}
			return d;
		}
		
		public static float[][] ToFloat(double[][] d) {
			float[][] f = new float[d.Length][];
			for (int i = 0; i < f.Length; i++) {
				f[i] = ToFloat(d[i]);
			}
			return f;
		}
		
		public static int[][] ToInt(double[][] d) {
			int[][] arr = new int[d.Length][];
			for (int i = 0; i < arr.Length; i++) {
				arr[i] = ToInt(d[i]);
			}
			return arr;
		}
		
		public static void Shift(double[] arr, int shift) {
			if (shift > 0) {
				shift = Math.Min(shift, arr.Length);
				for (int i = arr.Length-1-shift; i >= 0; i--) {
					arr[i+shift] = arr[i];
				}
				for (int i = 0; i < shift; i++) {
					arr[i] = 0;
				}
			} else {
				shift = Math.Max(shift, -arr.Length);
				for (int i = 0; i < arr.Length+shift; i++) {
					arr[i] = arr[i-shift];
				}
				for (int i = arr.Length+shift; i < arr.Length; i++) {
					arr[i] = 0;
				}
			}
		}
		
		/**
		 * 범위를 넘어서면 마지막 index를 반환한다.
		 * @param arr
		 * @param key
		 * @return
		 * @author mulova
		 */
		public static int BinarySearch(double[] arr, double key) {
			int i = Array.BinarySearch(arr, key);
			if (i < 0) {
				i = -i-1;
				if (i >= arr.Length) {
					i = arr.Length-1;
				} else if (i <= 0) {
					i = 0;
				}
			}
			return i;
		}
		
		public static bool Equals(Array a1, Array a2) {
			if (a1 == null) {
				return a2 == null;
			}
			if (a1.Length != a2.Length) {
				return false;
			}
			for (int i=0; i<a1.Length; i++) {
				if (a1.GetValue(i) == null) {
					if (a2.GetValue(i) != null) {
						return false;
					}
				} else if (!a1.GetValue(i).Equals(a2.GetValue(i))) {
					return false;
				}
			}
			return true;
		}
		
		public static object[] ToObject<T>(T[] values) {
		return Array.ConvertAll<T, object>(values, delegate(T t) {return (object) t;});
			//		ValueType[] dst = new ValueType[values.Length];
			//		for (int i=0; i<values.Length;i++) {
			//			dst[i] = values[i];
			//		}
			//		Array.
			//		return dst;
		}
	}
}






/*
using System;
using System.Collections.Generic;
using UnityEngine;
public class ArrayUtil {
	public static T[] Copy<T>(T[] source) {
		if (source == null) {
			return null;
		}
		T[] dest = (T[])Array.CreateInstance(typeof(T), source.Length);
		Array.Copy(source, dest, source.Length);
		return dest;
	}

	public static bool Resize<T>(ref T[] source, int length) {
		if (source.Length == length) {
			return false;
		}
		T[] dest = (T[])Array.CreateInstance(typeof(T), length);
		Array.Copy(source, dest, Math.Min(length, source.Length));
		source = dest;
		return true;
	}

//	 * arr을 재정렬한다.
//	 * arr item의  ToString() 값을 names 배열에 제공된 순서대로 맞추어 정렬하여 반환한다.
	public static T[] Arrange<T>(T[] arr, string[] names) {
		T[] rows = new T[names.Length];
		Dictionary<string, T> map = new Dictionary<string, T>();
		if (arr != null) {
			foreach (T r in arr) {
				if (r != null) {
					map[r.ToString()] = r;
				}
			}
		}
		for (int i=0; i<names.Length; i++) {
			if (map.ContainsKey(names[i])) {
				rows[i] = map[names[i]];
			}
		}
		return rows;
	}

//	 * Enum을 index로 사용하는 array의 크기가 바뀌었을 경우, 즉 Enum이 추가/삭제되었을 경우 array를 재배열한다.
//	 * 각 array item의 ToString() 은 enum의 이름과 동일해야 한다.
	public static bool ResizeEnumIndexedArray<E, D>(ref D[] data) {
		if (data == null) {
			data = new D[0];
			return true;
		}
		E[] enums = (E[])Enum.GetValues(typeof(E));
		int size = 0;
		Dictionary<E, int> indexMap = new Dictionary<E, int>();
		bool changed = false;
		foreach (E e in enums) {
			int i = (int)Enum.Parse(typeof(E), e.ToString());
			indexMap[e] = i;
			if (i >= 0) {
				size++;
				if (changed==false && (i>= data.Length || data[i] == null || data[i].ToString() != e.ToString())) {
					changed = true;
				}
			}
		}
		changed |= data == null || size != data.Length;

		if (changed) {
			D[] newData = new D[size];
			// 기존의 data를 먼저 채워넣는다.
			foreach (D d in data) {
				try {
					E e =(E)Enum.Parse(typeof(E), d.ToString());
					if (indexMap.ContainsKey(e)) {
						int i = indexMap[e] ;
						newData[i] = d;
					}
				} catch (Exception ex) {
					Debug.Log(typeof(E).FullName+"."+d.ToString() +": "+ ex.Message);
				}
			}

			// 빈 데이터를 채운다.
			foreach (E e in enums) {
				int i = indexMap[e];
				if (i >= 0 && newData[i] == null) {
					newData[i] = (D)typeof(D).GetConstructor(new Type[] { typeof(E)}).Invoke(new object[] {e});
				}
			}

			data = newData;
		}
		return changed;
	}

	public static string[] ToString(Array items) {
		string[] str = new string[items.Length];
		for (int i=0; i<items.Length; i++) {
			str[i] = items.GetValue(i).ToString();
		}
		return str;
	}

	public static double[] linearize(double[][] table) {
		int length = 0;
		for (int i = 0; i < table.Length; i++) {
			length += table[i].Length;
		}
		double[] arr = new double[length];
		int index = 0;
		for (int i = 0; i < table.Length; i++) {
			Array.Copy(table[i], 0, arr, index, table[i].Length);
			index += table[i].Length;
		}
		return arr;
	}

	public static int[] convert(float[] arr) {
		int[] intArr = new int[arr.Length];
		for (int i = 0; i < intArr.Length; i++) {
			intArr[i] = (int)arr[i];
		}
		return intArr;
	}

	public static int[] convert(double[] arr) {
		int[] intArr = new int[arr.Length];
		for (int i = 0; i < intArr.Length; i++) {
			intArr[i] = (int)arr[i];
		}
		return intArr;
	}

	public static double[][] createDoubleArray(int row, int col) {
		double[][] arr = new double[row][];
		for (int i=0; i<row; i++) {
			arr[i] = new double[col];
		}
		return arr;
	}

	public static double[] toDouble(float[] f) {
		double[] d = new double[f.Length];
		for (int i = 0; i < d.Length; i++) {
			d[i] = f[i];
		}
		return d;
	}

	public static float[] toFloat(double[] d) {
		float[] f = new float[d.Length];
		for (int i = 0; i < f.Length; i++) {
			f[i] = (float) d[i];
		}
		return f;
	}

	public static int[] toInt(double[] d) {
		int[] arr = new int[d.Length];
		for (int i = 0; i < arr.Length; i++) {
			arr[i] = (int) d[i];
		}
		return arr;
	}

	public static double[][] toDouble(float[][] f) {
		double[][] d = new double[f.Length][];
		for (int i = 0; i < d.Length; i++) {
			d[i] = toDouble(f[i]);
		}
		return d;
	}

	public static float[][] toFloat(double[][] d) {
		float[][] f = new float[d.Length][];
		for (int i = 0; i < f.Length; i++) {
			f[i] = toFloat(d[i]);
		}
		return f;
	}

	public static int[][] toInt(double[][] d) {
		int[][] arr = new int[d.Length][];
		for (int i = 0; i < arr.Length; i++) {
			arr[i] = toInt(d[i]);
		}
		return arr;
	}

	public static void shift(double[] arr, int shift) {
		if (shift > 0) {
			shift = Math.Min(shift, arr.Length);
			for (int i = arr.Length-1-shift; i >= 0; i--) {
				arr[i+shift] = arr[i];
			}
			for (int i = 0; i < shift; i++) {
				arr[i] = 0;
			}
		} else {
			shift = Math.Max(shift, -arr.Length);
			for (int i = 0; i < arr.Length+shift; i++) {
				arr[i] = arr[i-shift];
			}
			for (int i = arr.Length+shift; i < arr.Length; i++) {
				arr[i] = 0;
			}
		}
	}

//	 * 범위를 넘어서면 마지막 index를 반환한다.
	public static int binarySearch(double[] arr, double key) {
		int i = Array.BinarySearch(arr, key);
		if (i < 0) {
			i = -i-1;
			if (i >= arr.Length) {
				i = arr.Length-1;
			} else if (i <= 0) {
				i = 0;
			}
		}
		return i;
	}

	public static bool Equals(Array a1, Array a2) {
		if (a1 == null) {
			return a2 == null;
		}
		if (a1.Length != a2.Length) {
			return false;
		}
		for (int i=0; i<a1.Length; i++) {
			if (a1.GetValue(i) == null) {
				if (a2.GetValue(i) != null) {
					return false;
				}
			} else if (!a1.GetValue(i).Equals(a2.GetValue(i))) {
				return false;
			}
		}
		return true;
	}

	public static object[] ToObject<T>(T[] values) {
		return Array.ConvertAll<T, object>(values, delegate(T t) {return (object) t;});
		//		ValueType[] dst = new ValueType[values.Length];
		//		for (int i=0; i<values.Length;i++) {
		//			dst[i] = values[i];
		//		}
		//		Array.
		//		return dst;
	}
}
*/