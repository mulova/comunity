#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;


/**
 * 중심과 사각형의 네 모서리에 상응하는 Animation 5개를 사용해 Blending을 수행한다.
 * 
 * Quad를 네 부분으로 나누고 각 영역에 따라서 interpolation rate를 구한다.
 * 
 *  LT    RT	<br>
 *  -------		<br>
 *  |\   /| 	<br>
 *  | \ / |		<br>
 *  |  M  |		<br>
 *  | / \ |		<br>
 *  |/   \|		<br>
 *  -------		<br>
 *  LB    RB	<br>
 *  
 *  plane1  /
 *  plane2  \
 * 
 * 
 * z를 제외한 2차원 계산을 한다.
 * 
 * @author mulova
 */
public class QuintupleBlending {
	private QuintupleInterpolation interpolation;
	public const int M = QuintupleInterpolation.M;
	public const int LT = QuintupleInterpolation.LT;
	public const int LB = QuintupleInterpolation.LB;
	public const int RB = QuintupleInterpolation.RB;
	public const int RT = QuintupleInterpolation.RT;
	
	private Animation anim;
	private AnimationState[] animState;
	private bool begin = false;
	private Vector3 point;
	
	/**
	 * @param center 중점
	 * @param width 사각형의 너비
	 * @param height 사각형의 
	 */
	public void Init(Vector3 center, float width, float height, Animation anim,
		string middle, string leftTop, string leftBottom, string rightBottom, string rightTop) {
		interpolation = new QuintupleInterpolation(center, width, height);
		this.anim = anim;
		
		animState = new AnimationState[5];
		animState[M] = anim[middle];
		animState[LT] = anim[leftTop];
		animState[LB] = anim[leftBottom];
		animState[RB] = anim[rightBottom];
		animState[RT] = anim[rightTop];
		animState[M].layer = 0;
		animState[LT].layer = 1;
		animState[LB].layer = 2;
		animState[RB].layer = 3;
		animState[RT].layer = 4;
	}
	
	public void Interpolate(Vector3 point) {
		this.point = point;
	}
	
	public void CrossFade(Vector3 point, float fadeTime) {
		anim.CrossFade(animState[M].name, fadeTime);
		Interpolate(point);
	}
	
	public void Update() {
		if (IsActive()) {
			if (!begin) {
				begin = true;
				float[] rate = interpolation.getInterpolationRate(point);
				for (int i=0; i<5; i++) {
					animState[i].weight = rate[i];
					animState[i].enabled = i == 0 || rate[i] > 0;
				}
			}
			float time = animState[M].time;
			
			animState[LT].time = time;
			animState[LB].time = time;
			animState[RB].time = time;
			animState[RT].time = time;
		} else if (animState != null) {
			if (begin) {
				begin = false;
				animState[LT].enabled = false;
				animState[LB].enabled = false;
				animState[RB].enabled = false;
				animState[RT].enabled = false;
			}
		}
	}
	
	public bool IsActive() {
		return animState != null && animState[M].enabled == true && animState[M].time < animState[M].length;
	}
}
#endif