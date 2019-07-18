//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

namespace System.Collections.Generic
{
	public static class ListEx
	{
		public static void Swap<T>(this IList<T> list, int i, int j)
		{
			T temp = list[i];
			list[i] = list[j];
			list[j] = temp;
		}
		
		public static int GetCount<T>(this IList<T> list)
		{
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}
		
		public static T Get<T>(this IList<T> list, int i)
		{
			if (list == null||i >= list.Count)
			{
				return default(T);
			}
			return list[i];
		}
		
		public static T GetLast<T>(this List<T> list)
		{
			if (list == null||list.Count == 0)
			{
				return default(T);
			}
			return list[list.Count-1];
		}
		
		public static T GetFirst<T>(this List<T> list)
		{
			if (list == null||list.Count == 0)
			{
				return default(T);
			}
			return list[0];
		}
		
		public static T GetMax<T>(this List<T> list, Func<T, float> eval)
		{
			float score = float.MinValue;
			T found = default(T);
			foreach (T t in list)
			{
				float newScore = eval(t);
				if (newScore > score)
				{
					score = newScore;
					found = t;   
				}
			}
			return found;
		}
		
		public static T GetMin<T>(this List<T> list, Func<T, float> eval)
		{
			float score = float.MaxValue;
			T found = default(T);
			foreach (T t in list)
			{
				float newScore = eval(t);
				if (newScore < score)
				{
					score = newScore;
					found = t;   
				}
			}
			return found;
		}
		
		
		public static void ForEachEx<T>(this IList<T> list, Action<T> action)
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
		
		public static void RemoveOne<T>(this IList<T> list, Predicate<T> predicate)
		{
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; ++i)
			{
				if (list[i] != null&&predicate(list[i]))
				{
					list.RemoveAt(i);
					return;
				}
			}
		}
		
		public static void Remove<T>(this IList<T> list, Predicate<T> predicate)
		{
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; ++i)
			{
				if (list[i] != null&&predicate(list[i]))
				{
					list.RemoveAt(i);
				}
			}
		}
		
		/// <summary>
		/// Serialize the specified list and index.
		/// </summary>
		/// <param name="list">List.</param>
		/// <param name="index">Index. (one based)</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> Serialize<T>(this IList<T> list, IList<int> index, int indexBase = 0)
		{
			if (list == null)
			{
				return new List<T>();
			}
			List<T> ordered = new List<T>(list.Count);
			for (int i = 0; i < index.Count; ++i)
			{
				while (ordered.Count < index[i]-indexBase+1)
				{
					ordered.Add(default(T));
				}
				ordered[index[i]-indexBase] = list[i];
			}
			return ordered;
		}
		
		public static void Resize<T>(this List<T> list, int size)
		{
			if (list.Count < size)
			{
				while (list.Count < size)
				{
					list.Add(default(T));
				}
			} else
			{
				for (int i = list.Count-1; i >= size; --i)
				{
					list.RemoveAt(i);
				}
			}
		}
		
		public static void Set<T>(this List<T> list, int i, T val)
		{
			if (list.Count < i+1)
			{
				list.Resize(i+1);
			}
			list[i] = val;
		}

        public static Dictionary<K, V> ToDictionary<K, V>(this ICollection<V> src, Converter<V, K> converter)
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

    }
}
