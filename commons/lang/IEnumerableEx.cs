//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System.Collections.Generic;
using System;
using System.Collections;
using commons;

public static class IEnumerableEx
{
	public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
	{
		if (list == null)
		{
			return;
		}
		foreach (T t in list)
		{
			if (t != null)
			{
				action(t);
			}
		}
	}

	public static List<TOutput> ToList<T, TOutput>(this IEnumerable<T> src, Converter<T, TOutput> converter)
	{
		if (src == null)
		{
			return null;
		}
		List<TOutput> dst = new List<TOutput>();
		foreach (T t in src)
		{
			if (t != null)
			{
				dst.Add(converter(t));
			}
		}
		return dst;
	}

	public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<V> src, Converter<V, K> converter)
	{
		if (src == null)
		{
			return null;
		}
		Dictionary<K, V> dst = new Dictionary<K, V>(); 
		foreach (V v in src)
		{
			K k = converter(v);
			if (k != null)
			{
				dst[k] = v;
			}
		}
		return dst;
	}

	public static List<T> Filter<T>(this IEnumerable<T> src, Predicate<T> predicate)
	{
		List<T> dst = new List<T>();
		if (src != null)
		{
			foreach (T t in src)
			{
				if (predicate(t))
				{
					dst.Add(t);
				}
			}
		}
		return dst;
	}

	public static T Find<T>(this IEnumerable<T> src, Predicate<T> predicate)
	{
		if (src != null && predicate != null)
		{
			foreach (T t in src)
			{
				if (predicate(t))
				{
					return t;
				}
			}
		}
		return default(T);
	}

	public static string Join(this IEnumerable list, string separator)
	{
		return StringUtil.Join(separator, list);
	}
}

