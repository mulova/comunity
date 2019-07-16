using UnityEngine.Assertions;
using comunity;
using UnityEngine;

namespace comunity
{
	public static class Vector3Ex
	{
		public static Vector3 Divide(this Vector3 v, float divisor)
		{
			Assert.AreNotEqual(divisor, 0);
			return new Vector3(v.x/divisor, v.y/divisor, v.z/divisor);
		}

		public static Vector3 Divide(this Vector3 v, Vector3 divisor)
		{
			Assert.AreNotEqual(divisor.x, 0);
			Assert.AreNotEqual(divisor.y, 0);
			Assert.AreNotEqual(divisor.z, 0);
			return new Vector3(v.x/divisor.x, v.y/divisor.y, v.z/divisor.z);
		}

		public static Vector3 Mult(this Vector3 v, Vector3 m)
		{
			return Vector3.Scale(v, m);
		}

		public static Vector3 Mult(this Vector3 v, float scale)
		{
			return new Vector3(v.x*scale, v.y*scale, v.z*scale);
		}

        public static bool ApproximatelyEquals(this Vector3 v1, Vector3 v2)
        {
            return MathUtil.ApproximatelyEquals(v1.x, v2.x)
                && MathUtil.ApproximatelyEquals(v1.y, v2.y)
                && MathUtil.ApproximatelyEquals(v1.z, v2.z);
        }
	}
}

