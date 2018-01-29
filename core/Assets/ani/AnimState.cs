using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ani
{
    public class AnimState : StateMachineBehaviour
    {
        private IAnimStateListener listener;

        public void Init(IAnimStateListener l, object o)
        {
            this.listener = l;
            Init(o);
        }

        protected virtual void Init(object o)
        {
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (listener != null)
            {
                listener.OnStateEnter(animator, stateInfo, layerIndex);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (listener != null)
            {
                listener.OnStateExit(animator, stateInfo, layerIndex);
            }
        }
    }
}

