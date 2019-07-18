//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using System.Text;
using System.Text.Ex;

namespace System.Collections.Generic.Ex
{
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
            int count = 0;
            StringBuilder str = new StringBuilder(256);
            foreach (object o in list)
            {
                if (o != null)
                {
                    if (count != 0)
                    {
                        str.Append(separator);
                    }
                    string t = o.ToString();
                    if (t.IsNotEmpty())
                    {
                        str.Append(t);
                        count++;
                    }
                }
            }
            return str.ToString();
        }
	}
}
