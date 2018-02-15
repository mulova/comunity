//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using comunity;
using System;

namespace ani {
	public class AnimUtil {
		
		/**
		 * 현재 SkinRenderer의 bone을 지정된 bone으로 교체한다.
		 */
		public static void SetBone(Transform rootBone, params SkinnedMeshRenderer[] renderers) {
			SetBone(rootBone.CreateTransformMap(), renderers);
		}
		
		/**
		 * 현재 SkinRenderer의 bone을 지정된 bone으로 교체한다.
		 */
		public static void SetBone(Dictionary<string, Transform> boneMap, params SkinnedMeshRenderer[] renderers) {
			foreach (SkinnedMeshRenderer r in renderers) {
				Transform[] skinBones = r.bones;
				for (int i=0; i<skinBones.Length; i++) {
					skinBones[i] = boneMap[skinBones[i].name];
				}
				r.bones = skinBones;
			}
		}
		
		public static AnimationClip[] GetAnimationClips(Animation anim) {
			if (anim == null) {
				return new AnimationClip[0];
			}
			List<AnimationClip> list = new List<AnimationClip>();
			foreach (AnimationState s in anim) {
				list.Add(anim.GetClip(s.name));
			}
			return list.ToArray();
		}
		
		
		public static List<string> GetPlayingAnimationList(Animation anim) {
			List<string> playing = new List<string>();
			foreach (AnimationClip c in GetAnimationClips(anim)) {
				if (anim.IsPlaying(c.name)) {
					playing.Add(c.name);
				}
			}
			return playing;
		}

		/**
		 * Animation를 Reset하고 특정 시간의 animation 상태로 설정한다.
		 * Animation 특정상태의 위치를 얻을때 사용한다.
		 */
		public static void Sample(Transform trans, Animation anim, string clipName, float time) {
			anim.Stop();
			trans.position = Vector3.zero;
			trans.rotation = Quaternion.identity;
			AnimationState state = anim[clipName];
			WrapMode wrapMode = state.wrapMode;
			
			// activate the animation state
			state.weight = 1f;
			state.enabled = true;
			state.wrapMode = WrapMode.Clamp; // ensures the value at normalizedTime = 1f is not necessarily the same as normalizedTime = 0f
			
			state.time = time;
			anim.Sample();
			state.wrapMode = wrapMode;
		}
		
		public static Vector3 GetProjectedAxis(Transform root, Transform bone, Vector3 rightAxis)
		{
			Vector3 p = root.InverseTransformDirection(bone.TransformDirection(rightAxis));
			p.y = 0f;
			return p;
		}
	}
}
