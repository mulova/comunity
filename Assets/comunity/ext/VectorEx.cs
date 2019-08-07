using UnityEngine.Assertions;

namespace UnityEngine.Ex
{
	public static class VectorEx
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
            return v1.x.ApproximatelyEquals(v2.x)
                && v1.y.ApproximatelyEquals(v2.y)
                && v1.z.ApproximatelyEquals(v2.z);
        }

        public static bool Equals(this Vector3 v1, Vector3 v2, float tolerance)
        {
            return v1.x.Equals(v2.x, tolerance) && v1.y.Equals(v2.y, tolerance) && v1.z.Equals(v2.z, tolerance);
        }

        public static bool Equals(Vector2 v1, Vector2 v2, float tolerance)
        {
            return v1.x.Equals(v2.x, tolerance) && v1.y.Equals(v2.y, tolerance);
        }
    }
}

