//----------------------------------------------
// Unity3D common libraries and eidtor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using mulova.commons;
using mulova.comunity;

namespace UnityEngine.Ex 
{
	public static class QuaternionEx {
		
		public static readonly Quaternion Y_45 = Quaternion.AngleAxis(45, Vector3.up);
		public static readonly Quaternion Y_90 = Quaternion.AngleAxis(90, Vector3.up);
		public static readonly Quaternion Y_135 = Quaternion.AngleAxis(135, Vector3.up);
		public static readonly Quaternion Y_180 = Quaternion.AngleAxis(180, Vector3.up);
		public static readonly Quaternion Y_225 = Quaternion.AngleAxis(225, Vector3.up);
		public static readonly Quaternion Y_MINUS_90 = Quaternion.AngleAxis(-90, Vector3.up);
		
		
		public static bool IsValid(this Quaternion q) {
			if (q.x == 0 && q.y == 0 && q.z == 0 && q.w == 0)
				return false;
			return !(float.IsNaN(q.x) || float.IsNaN(q.x) || float.IsNaN(q.x) || float.IsNaN(q.x));
		}
		
		
		public static bool ApproximatelyEquals(this Quaternion first,  Quaternion second) {
			return (Mathf.Approximately(first.x, second.x) && Mathf.Approximately(first.y, second.y)
				&& Mathf.Approximately(first.z, second.z) && Mathf.Approximately(first.w, second.w)) ||
				(Mathf.Approximately(first.x, -second.x) && Mathf.Approximately(first.y, -second.y)
					&& Mathf.Approximately(first.z, -second.z) && Mathf.Approximately(first.w, -second.w));
		}

		public static bool ApproximatelyEquals(this Quaternion first,  Quaternion second, float delta) {
			return Mathf.Abs(first.x - second.x) < delta
				&& Mathf.Abs(first.y - second.y) < delta
				&& Mathf.Abs(first.z - second.z) < delta
				&& Mathf.Abs(first.w - second.w) < delta;
		}
		
		/**
		 * 지정된 축에 관한 회전만 남겨둔다.
		 * @param rotation 분할할 회전값
		 * @param axis 회전 분할의 기준축
		 * @param baseDirection 앞쪽의 기준이 되는 방향
		 * @param store     rotation과 같은 개체일 수 있다. 
		 * @return
		 */
		private static Quaternion AlignAxis(this Quaternion rotation, Axis axis, Vector3 baseDirection) {
			Vector3 direction = rotation*baseDirection;
            direction = VectorUtil.GetAxisOrthogonal(axis, direction);
			if (direction == Vector3.zero) {
				return Quaternion.identity;
			} else {
				direction.Normalize();
				return Quaternion.LookRotation(direction, VectorUtil.ToUnitVector(axis));
			}
		}
		
		
		/**
		 * 축 회전과, 축 회전을 제외한 회전으로 나눈다. remain * aligned 으로 분해된다.
		 * @param rotation  
		 * @param axis      분해할 회전 축.
		 * @param baseDirection 회전의 기준 방향
		 * @param[out] aligned 제시된 축을 사용한 회전
		 * @param[out] remain 축 회전을 제외한 회전. 원래 회전을 복원하려면 remain * aligned. null일 수 있다.
		 * @return aligned
		 */
		public static void Decompose(this Quaternion rotation, Axis axis, Vector3 baseDirection, out Quaternion axisAligned, out Quaternion remain) {
			axisAligned = AlignAxis(rotation, axis, baseDirection);
			remain = rotation * Quaternion.Inverse(axisAligned);
		}
		
		/**
		 * 축 회전과, 축 회전을 제외한 회전으로 나눈다. aligned * remain 으로 분해된다.
		 * @param rotation  
		 * @param axis      분해할 회전 축.
		 * @param baseDirection 회전의 기준 방향
		 * @param[out] aligned 제시된 축을 사용한 회전
		 * @param[out] remain 축 회전을 제외한 회전. 원래 회전을 복원하려면 remain * aligned. null일 수 있다.
		 * @return aligned
		 */
		public static void Decompose2(this Quaternion rotation, Axis axis, Vector3 baseDirection, out Quaternion aligned, out Quaternion remain) {
			aligned = AlignAxis(rotation, axis, baseDirection);
			Quaternion inverse = Quaternion.Inverse(aligned);
			remain = inverse * rotation;
		}
		
		
		/**
		 * 지정된 축을 기준으로 한 회전을 구한다.
		 * @param start local start direction. no need to be normalized
		 * @param end   local end direction. no need to be normalized
		 * @param axis
		 * @param store
		 * @return
		 */
		public static Quaternion GetRotation(Vector3 start,  Vector3 end, Axis axis) {
			Vector3 v1 = VectorUtil.GetAxisOrthogonal(axis, start);
			Vector3 v2 = VectorUtil.GetAxisOrthogonal(axis, end);
			if (v1 == Vector3.zero || v2 == Vector3.zero) {
				return Quaternion.identity;
			} else {
				Vector3 up = VectorUtil.ToUnitVector(axis);
				Quaternion q1 = Quaternion.LookRotation(v1, up);
				Quaternion q2 = Quaternion.Inverse(Quaternion.LookRotation(v2, up));
				return q2 * q1;
			}
		}
	}
}
