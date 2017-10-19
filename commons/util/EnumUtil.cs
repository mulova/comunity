//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace commons {
	public static class EnumUtil {
		// <T> where T : struct, IComparable, IConvertible, IFormattable
		private static MultiKeyMap<Type, string, object> enums = new MultiKeyMap<Type, string, object>();
		private static Loggerx log = LogManager.GetLogger(typeof(Enum));
		
		public static T[] Values<T>() where T : struct, IComparable, IConvertible, IFormattable {
			return (T[])Enum.GetValues(typeof(T));
		}
		
		public static T[] ValuesExclude<T>(T exclude) where T : struct, IComparable, IConvertible, IFormattable {
			string excludeStr = exclude.ToString();
			List<T> list = new List<T>();
			foreach (T e in (T[])Enum.GetValues(typeof(T))) {
				if (e.ToString() != excludeStr) {
					list.Add(e);
				}
			}
			return list.ToArray();
		}
		
		
		public static string[] ToStrings<T>() where T : struct, IComparable, IConvertible, IFormattable {
			T[] arr = (T[])Enum.GetValues(typeof(T));
			string[] str = new string[arr.Length];
			for (int i=0; i<str.Length; i++) {
				str[i] = arr[i].ToString();
			}
			return str;
		}
		
		public static T Parse<T>(string s) where T : struct, IComparable, IConvertible, IFormattable {
			return Parse(s, default(T));
		}
		
		public static T Parse<T>(string s, T defaultValue) where T : struct, IComparable, IConvertible, IFormattable{
			return (T)Parse(typeof(T), s, defaultValue);
		}

		public static T ParseIgnoreCase<T>(string s, T defaultValue) where T : struct, IComparable, IConvertible, IFormattable {
			foreach (T t in Enum.GetValues(typeof(T))) {
				if (s.EqualsIgnoreCase(t.ToString())) {
					return t;
				}
			}
			return defaultValue;
		}

		/// <summary>
		/// No Exception.
		/// Use caching to get enum fast from string
		/// </summary>
		/// <returns>The enum.</returns>
		/// <param name="s">enum name</param>
		/// <param name="defaultValue">Default value.</param>
		public static object Parse(Type type, string s, object defVal) {
			if (s.IsEmpty()) 
			{
				return defVal;
			}
			if (enums.Contains(type, s)) {
				return enums.Get(type, s);
			}
			if (IsEnum(type, s)) {
				object t = Enum.Parse(type, s);
				enums[type,s] = t;
				return t;
			} else {
				log.Warn("No enum {0}.{1}", type.FullName, s);
				return defVal;
			}
		}

		public static object Parse(Type type, string s) {
			return Parse(type, s, GetOne(type));
		}

		private static Dictionary<Type, object> firstValues = new Dictionary<Type, object>();
		public static object GetOne(Type type) {
			if (firstValues.ContainsKey(type)) {
				return firstValues[type];
			}
			object o = Enum.GetValues(type).GetValue(0);
			firstValues[type] = o;
			return o;
		}
		
		public static bool IsEnum<T>(string s) where T : struct, IComparable, IConvertible, IFormattable {
			return IsEnum(typeof(T), s);
		}

		public static bool IsEnum(Type type, string s) {
			if (s.IsEmpty()) {
				return false;
			}
			return Enum.IsDefined(type, s);
		}
		
		public static int Size<T>() where T : struct, IComparable, IConvertible, IFormattable {
			return Enum.GetValues(typeof(T)).Length;
		}
		
		public static string GetDescription<T>(Enum e) where T : struct, IComparable, IConvertible, IFormattable {
			Type type = e.GetType();
			MemberInfo[] ms = type.GetMember(e.ToString());
			if(ms!=null && ms.Length>0) {
				object[] attrs = ms[0].GetCustomAttributes(typeof(DescriptionAttribute),false);
				if(attrs!=null && attrs.Length>0) {
					return ((DescriptionAttribute)attrs[0]).Description;
				}
			}
			return e.ToString();
		}
	}
}
