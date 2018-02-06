#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;

/**
 *  1     4		<br>
 *  -------		<br>
 *  |\   /| 	<br>
 *  | \ / |		<br>
 *  |  0  |		<br>
 *  | / \ |		<br>
 *  |/   \|		<br>
 *  -------		<br>
 *  2     3		<br>
 *  
 *  plane1  /
 *  plane2  \
 * 
 * Quad를 네 부분으로 나누고 각 영역에 따라서 interpolation rate를 구한다.
 * z를 제외한 2차원 계산을 한다.
 * 
 * @author mulova
 */
namespace comunity
{
	public class QuintupleInterpolation {
		public const int M = 0;
		public const int LT = 1;
		public const int LB = 2;
		public const int RB = 3;
		public const int RT = 4;
		
		private float	left;
		private float	right;
		private float	bottom;
		private float	top;
		
		private Vector3[]	v;
		private Vector3		center;
		
		private Plane			plane1;
		private Plane			plane2;
		float[]				rate		= new float[5];
		
		/**
		* @param center	quad의 중심
		* @param width		quad의 너비
		* @param height	quad의 높이
		*/
		public QuintupleInterpolation(Vector3 center, float width, float height) {
			left	= center.x - width/2;
			right	= center.x + width/2;
			bottom	= center.y - height/2;
			top	= center.y + height/2;
			
			v = new Vector3[5];
			v[M] = center;
			v[LT] = new Vector3(left, top, 0);
			v[LB] = new Vector3(left, bottom, 0);
			v[RB] = new Vector3(right, bottom, 0);
			v[RT] = new Vector3(right, top, 0);
			this.center = new Vector3(center.x, center.y, 0);
			
			Vector3 tmp = v[RT];
			tmp.z = -10;
			this.plane1 = new Plane(v[LB], v[RT], tmp);
			tmp = v[RB];
			tmp.z = -10;
			this.plane2 = new Plane(v[LT], v[RB], tmp);
		}
		
		
		/**
		* interpolation에 사용될 point를 선택한다.
		* index0: middle
		* index1: lt
		* index2: lb
		* index3: rb
		* index4: rt
		* 
		* @param 
		* @return
		*/
		public float[] getInterpolationRate(Vector3 blendPoint) {
			blendPoint.z = 0f;
			blendPoint.x = Mathf.Clamp(blendPoint.x, left, right);
			blendPoint.y = Mathf.Clamp(blendPoint.y, bottom, top);
			
			this.rate[M] = this.rate[LT] = this.rate[LB] = this.rate[RB] = this.rate[RT] = 0;
			if (blendPoint.x <= left && blendPoint.y >= top) {
				rate[LT] = 1;
				return rate;
			} else if (blendPoint.x <= left && blendPoint.y <= bottom) {
				rate[LB] = 1;
				return rate;
			} else if (blendPoint.x >= right && blendPoint.y <= bottom) {
				rate[RB] = 1;
				return rate;
			} else if (blendPoint.x >= right && blendPoint.y >= top) {
				rate[RT] = 1;
				return rate;
			}
			bool up1 = this.plane1.GetSide(blendPoint);
			bool up2 = this.plane2.GetSide(blendPoint);
			
			Plane plane;
			int i1 = 0;
			int i2 = 0;
			// select points
			if (up1 && !up2) {
				i1 = LT;
				i2 = LB;
				plane = this.plane2;
			} else if (!up1 && !up2) {
				i1 = LB;
				i2 = RB;
				plane = this.plane1;
			} else if (!up1 && up2) {
				i1 = RB;
				i2 = RT;
				plane = this.plane2;
			} else {
				i1 = LT;
				i2 = RT;
				plane = this.plane2;
			}
			
			Ray ray = new Ray(v[i2], blendPoint-v[i2]);
			float distance = 0;
			bool intersect = plane.Raycast(ray, out distance);
			Vector3 tmp = ray.GetPoint(distance);
			
			if (intersect) {
				this.rate[i1] = Vector3.Distance(center, tmp) / Vector3.Distance(center, v[i1]);
				this.rate[i2] = 1 - Vector3.Distance(v[i2], blendPoint) / Vector3.Distance(v[i2], tmp);
				this.rate[M] = 1-this.rate[i1];
			}
			
			return this.rate;
		}
		
		public float Left {
			get {
				return left;
			}
		}
		
		public float Right {
			get {
				return right;
			}
		}
		
		public float Top {
			get {
				return top;
			}
		}
		
		public float Bottom {
			get {
				return bottom;
			}
		}
		
		public Vector3 getCenter {
			get {
				return center;
			}
		}
	}
}
#endif