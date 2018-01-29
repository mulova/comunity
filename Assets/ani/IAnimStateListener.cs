using UnityEngine;
using System;

namespace ani
{
	public interface IAnimStateListener
	{
		void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
		void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
	}
}

