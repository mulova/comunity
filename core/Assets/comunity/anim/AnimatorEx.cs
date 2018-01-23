using System;
using core;

namespace UnityEngine
{
    public static class AnimatorEx
    {
        public static bool IsPlaying(this Animator anim, int layer)
        {
            if (anim.IsInTransition(layer))
            {
                return true;
            }
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(layer);
            return !state.loop&&state.normalizedTime < 1;
        }

        public static void Rewind(this Animator anim)
        {
            anim.enabled = false;
        }

        public static void ListenForStateEvent(this Animator anim, IAnimStateListener l, object o)
        {
            foreach (AnimState s in anim.GetBehaviours<AnimState>())
            {
                s.Init(l, o);
            }
        }
    }
}

