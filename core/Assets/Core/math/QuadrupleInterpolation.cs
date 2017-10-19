#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using UnityEngine;

/**
 *  0     3		<br>
 *  -------		<br>
 *  |\   /| 	<br>
 *  | \ / |		<br>
 *  |  c  |		<br>
 *  | / \ |		<br>
 *  |/   \|		<br>
 *  -------		<br>
 *  1     2		<br>
 *  
 *  plane1  /
 *  plane2  \
 * 
 * Quad를 네 부분으로 나누고 각 영역에 따라서 interpolation rate를 구한다.
 * z를 제외한 2차원 계산을 한다.
 * 
 * @author mulova
 */
public class QuadrupleInterpolation {
	private float	left;
	private float	right;
	private float	bottom;
	private float	top;

	private Vector3[]	v;
	private Vector3		center;

	private Plane			plane1;
	private Plane			plane2;
	float[]				rate		= new float[4];

	private Vector3		impactPoint	= Vector3.zero;
	
	/**
	 * @param center	quad의 중심
	 * @param width		quad의 너비
	 * @param height	quad의 높이
	 */
	public QuadrupleInterpolation(Vector3 center, float width, float height) {
		left	= center.x - width/2;
		right	= center.x + width/2;
		bottom	= center.y - height/2;
		top	= center.y + height/2;
		
		v = new Vector3[4];
		v[0] = new Vector3(left, top, 0);
		v[1] = new Vector3(left, bottom, 0);
		v[2] = new Vector3(right, bottom, 0);
		v[3] = new Vector3(right, top, 0);
		this.center = new Vector3(center.x, center.y, 0);
		
		Vector3 tmp = v[3];
		tmp.z = -10;
		this.plane1 = new Plane(v[1], v[3], tmp);
		tmp = v[2];
		tmp.z = -10;
		this.plane2 = new Plane(v[0], v[2], tmp);
	}


	/**
	 * interpolation에 사용될 point를 선택한다.
	 * 
	 * @param hitPoint
	 * @return
	 */
	public float[] getInterpolationRate(Vector3 hitPoint) {
		impactPoint = hitPoint;
		impactPoint.z = 0f;
		impactPoint.x = Mathf.Clamp(impactPoint.x, left, right);
		impactPoint.y = Mathf.Clamp(impactPoint.y, bottom, top);
		
		this.rate[0] = this.rate[1] = this.rate[2] = this.rate[3] = 0;
		if (impactPoint.x <= left && impactPoint.y >= top) {
			rate[0] = 1;
			return rate;
		} else if (impactPoint.x <= left && impactPoint.y <= bottom) {
			rate[1] = 1;
			return rate;
		} else if (impactPoint.x >= right && impactPoint.y <= bottom) {
			rate[2] = 1;
			return rate;
		} else if (impactPoint.x >= right && impactPoint.y >= top) {
			rate[3] = 1;
			return rate;
		}
		bool up1 = this.plane1.GetSide(impactPoint);
		bool up2 = this.plane2.GetSide(impactPoint);

		Plane plane;
		int i1 = 0;
		int i2 = 0;
		// select points
		if (up1 && !up2) {
			i1 = 0;
			i2 = 1;
			plane = this.plane2;
		} else if (!up1 && !up2) {
			i1 = 1;
			i2 = 2;
			plane = this.plane1;
		} else if (!up1 && up2) {
			i1 = 2;
			i2 = 3;
			plane = this.plane2;
		} else {
			i1 = 3;
			i2 = 0;
			plane = this.plane1;
		}

		Ray ray = new Ray(v[i2], impactPoint-v[i2]);
		float distance = 0;
		bool intersect = plane.Raycast(ray, out distance);
		Vector3 tmp = ray.GetPoint(distance);

		if (intersect) {
			this.rate[i1] = Vector3.Distance(center, tmp) / Vector3.Distance(center, v[i1]);
			this.rate[i2] = 1 - Vector3.Distance(v[i2], impactPoint) / Vector3.Distance(v[i2], tmp);
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
#endif