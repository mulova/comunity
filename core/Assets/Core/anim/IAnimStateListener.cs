using UnityEngine;
using System;

namespace core
{
	public interface IAnimStateListener
	{
		void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
		void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
	}
}

