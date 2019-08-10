//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

namespace UnityEngine.Ex {
	public static class Matrix4x4Ex {
		public static Quaternion GetRotation(this Matrix4x4 m) {
			// Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
			Quaternion q = new Quaternion();
			q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] + m[1,1] + m[2,2] ) ) / 2; 
			q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] - m[1,1] - m[2,2] ) ) / 2; 
			q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] + m[1,1] - m[2,2] ) ) / 2; 
			q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] - m[1,1] + m[2,2] ) ) / 2; 
			q.x *= Mathf.Sign( q.x * ( m[2,1] - m[1,2] ) );
			q.y *= Mathf.Sign( q.y * ( m[0,2] - m[2,0] ) );
			q.z *= Mathf.Sign( q.z * ( m[1,0] - m[0,1] ) );
			return q;
		}

		public static Vector3 GetPosition(this Matrix4x4 m) {
			return m.GetColumn(3);
		}
	}
}
