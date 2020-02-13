//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using mulova.commons;
using System.Ex;

namespace mulova.comunity {
	public static class EditorAnimUtil {
		public static string[] GetAnimatorParameters(Animator anim, AnimatorControllerParameterType paramType) {
			if (anim != null) {
				List<string> list = new List<string>();
				AnimatorController ac = anim.runtimeAnimatorController as AnimatorController;
				foreach (AnimatorControllerParameter param in ac.parameters)
				{
					if (param.type == paramType) {
						list.Add(param.name);
					}
				}
				return list.ToArray();
			}
			return new string[0];
		}

		public static string[] GetAnimatorStates(Animator anim) {
			if (anim != null) {
				List<string> list = new List<string>();
				AnimatorController ac = anim.runtimeAnimatorController as AnimatorController;
				foreach (AnimatorControllerLayer layer in ac.layers)
				{
					foreach (ChildAnimatorState state in layer.stateMachine.states)
					{
						list.Add(state.state.name);
					}
				}
				for (int count = 0; count < anim.layerCount; ++count)
				{
				}
				return list.ToArray();
			}
			return new string[0];
		}

		public static float GetAnimationWindowTime() {
			if (AnimationMode.InAnimationMode()) {
				if (EditorWindow.focusedWindow.GetType().Name == "AnimationWindow") {
					object animEditor = EditorWindow.focusedWindow.GetFieldValue<object>("m_AnimEditor");
					object animState = animEditor.GetFieldValue<object>("m_State");
					return animState.GetFieldValue<float>("m_CurrentTime");
				}
			}
			return 0;
		}
	}
}
