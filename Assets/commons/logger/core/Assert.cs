using System;

namespace commons
{
	public static class Assert
	{
		public static readonly Loggerx log = LogManager.GetLogger(typeof(Assert));

		public delegate bool IsTrueFunc();

		public static bool IsEnabled()
		{
			return true;
		}

		public static void Fail(object context, string format, System.Exception ex, params object[] param)
		{
			if (!IsEnabled())
			{
				return;
			}
            log.context = context;
			log.Error(ex, format, param);
		}

		public static void Fail(object context, string format, params object[] param)
		{
			Fail(context, format, null, param);
		}

		
		public static void IsNull(object o)
		{
			if (!IsEnabled())
			{
				return;
			}
			IsTrue(o == null, "Not Null. {0}", o);
		}

		
		public static void IsNull(object o, string format, params object[] param)
		{
			if (!IsEnabled())
			{
				return;
			}
			IsTrue(o == null, format, param);
		}

		
		public static void IsNotNull(object o)
		{
			if (!IsEnabled())
			{
				return;
			}
			IsTrue(o != null, "Null");
		}

		
		public static void IsNotNull(object o, string format, params object[] param)
		{
			if (!IsEnabled())
			{
				return;
			}
			IsTrue(o != null, format, param);
		}

		
		public static void IsTrue(bool b)
		{
			if (!IsEnabled())
			{
				return;
			}
			IsTrue(b, "Fail");
		}

		
		public static void IsTrue(Object context, IsTrueFunc func)
		{
			if (!IsEnabled())
			{
				return;
			}
			if (func() == false)
			{
				Fail(null, "Fail");
			}
		}

		
		public static void IsTrue(bool b, string format, params object[] param)
		{
			if (!IsEnabled())
			{
				return;
			}
			if (b == false)
			{
				Fail(null, format, param);
			}
		}

		public static void IsFalse(bool b)
		{
			if (!IsEnabled())
			{
				return;
			}
			IsFalse(b, "Fail");
		}

		
		public static void IsFalse(bool b, string format, params object[] param)
		{
			if (!IsEnabled())
			{
				return;
			}
			if (b == true)
			{
				Fail(null, format, param);
			}
		}

		public static void AreEqual(object s1, object s2)
		{
			if (!IsEnabled())
			{
				return;
			}
			if (s1.Equals(s2) == false)
			{
				Fail(null, "Not same {0} <-> {1}", s1, s2);
			}
		}

		public static void AreEqual(double d1, double d2, double tolerance)
		{
			if (!IsEnabled())
			{
				return;
			}
			if (Math.Abs(d1-d2) > tolerance)
			{
				Fail(null, "Expected {0}, but {1}", d1, d2);
			}
		}

		public static void AreEqual(int i1, int i2)
		{
			if (!IsEnabled())
			{
				return;
			}
			if (i1 != i2)
			{
				Fail(null, "Expected {0}, but {1}", i1, i2);
			}
		}

		
		public static void IsSame(object o1, object o2)
		{
			if (!IsEnabled())
			{
				return;
			}
			if (!System.Object.ReferenceEquals(o1, o2))
			{
				Fail(null, "Reference Not Same. {0}, {1}", o1, o2);
			}
		}
	}
}

