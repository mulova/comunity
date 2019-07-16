using System;

namespace commons
{
	public static class Assert
	{
		public static readonly Loggerx log = LogManager.GetLogger(typeof(Assert));

		public delegate bool IsTrueFunc();

		public static void Fail(object context, string format, System.Exception ex, params object[] param)
		{
            log.context = context;
			log.Error(ex, format, param);
            UnityEngine.Assertions.Assert.IsTrue(false);
        }

		public static void Fail(object context, string format, params object[] param)
		{
			Fail(context, format, null, param);
		}

		
		public static void IsNull(object o)
		{
			IsTrue(o == null, "Not Null. {0}", o);
		}

		
		public static void IsNull(object o, string format, params object[] param)
		{
			IsTrue(o == null, format, param);
		}

		
		public static void IsNotNull(object o)
		{
			IsTrue(o != null, "Null");
		}

		
		public static void IsNotNull(object o, string format, params object[] param)
		{
			IsTrue(o != null, format, param);
		}

		
		public static void IsTrue(bool b)
		{
			IsTrue(b, "Fail");
		}

		
		public static void IsTrue(Object context, IsTrueFunc func)
		{
            UnityEngine.Assertions.Assert.IsTrue(func(), "fail");
		}

		
		public static void IsTrue(bool b, string format, params object[] param)
		{
            UnityEngine.Assertions.Assert.IsTrue(b, string.Format(format, param));
        }

		public static void IsFalse(bool b)
		{
			IsFalse(b, "Fail");
		}

		
		public static void IsFalse(bool b, string format, params object[] param)
		{
            UnityEngine.Assertions.Assert.IsFalse(b, string.Format(format, param));
		}

		public static void AreEqual(object expected, object actual)
		{
            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
		}

		public static void AreEqual(double exptected, double actual, double tolerance)
		{
            if (Math.Abs(exptected-actual) > tolerance)
            {
                UnityEngine.Assertions.Assert.AreEqual(exptected, actual);
			}
		}

		public static void AreEqual(int expected, int actual)
		{
            UnityEngine.Assertions.Assert.AreEqual(expected, actual);
		}

		
		public static void IsSame(object expected, object actual)
		{
            if (!System.Object.ReferenceEquals(expected, actual))
			{
				Fail(null, "Reference Not Same. {0}, {1}", expected, actual);
			}
		}
	}
}

