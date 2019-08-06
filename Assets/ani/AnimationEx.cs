#define ANIM_EVENT_WORKAROUND
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using mulova.commons;
using ani;
using comunity;
using System.Ex;

namespace UnityEngine.Ex
{
    public static class AnimationEx {
	    private static WeakHashSet<AnimationClip> registeredClips = new WeakHashSet<AnimationClip>();
        public static readonly Loggerx log = LogManager.GetLogger(typeof(Animation));

	    public static void PlayIgnoreScale( this Animation animation, AnimationClip clip, Action onComplete = null)
	    {
		    animation.FindComponent<IgnoreTimeScale>().Play(clip, onComplete);
	    }

	    public static void PlayIgnoreScale( this Animation animation, string clipName, Action onComplete = null)
	    {
		    animation.FindComponent<IgnoreTimeScale>().Play(clipName, onComplete);
	    }

	    [Conditional("DEBUG"), Conditional("UNITY_EDITOR")]
	    private static void CheckAnim(Animation anim) {
		    if (!Application.isEditor) {
			    return;
		    }
		    // just check the first clip
		    foreach (AnimationState s in anim) {
			    AnimationClip clip = anim.GetClip(s.name);
			    if (!refs.Contains(clip)) {
                    UnityEngine.Debug.LogFormat("{0} is instantiated for the first time", anim.name);
				    refs.Add(clip);
			    }
			    break;
		    }
	    }

	    private static WeakHashSet<AnimationClip> refs = new WeakHashSet<AnimationClip>();
	
	    public static bool IsPlayingEx(this Animation anim, string name) {
		    CheckAnim(anim);
		    return anim.IsPlaying(name);
	    }

	    public static bool Play(this Animation anim, AnimationClip clip) {
		    return anim.Play(clip.name);
	    }

	    public static void Rewind(this Animation anim, AnimationClip clip) {
		    anim.Rewind(clip.name);
	    }

	    public static bool PlayEx(this Animation anim, Action<string> endCallback = null) {
		    return PlayEx(anim, anim.clip, endCallback);
	    }

	    public static bool PlayEx(this Animation anim, AnimationClip clip, Action<string> endCallback = null) {
		    string clipName = clip!=null? clip.name: string.Empty;
		    return anim.PlayEx(clipName, endCallback);
	    }

	    public static bool PlayEx(this Animation anim, string name, Action<string> endCallback = null) {
		    if (anim.isActiveAndEnabled) {
    #if ANIM_EVENT_WORKAROUND
                AnimationClip clip = anim.GetClip(name);
                if (clip != null)
                {
                    if (endCallback != null)
                    {
                        anim.AddOneShotCallback(endCallback);
                        anim.GetComponent<AnimEventReceiver>().Invoke(AnimEventReceiver.END_CALLBACK, clip.length);
                    }
                } else
                {
                    log.Error("Animation Clip {0} is missing", name);
                }
                return anim.Play(name);
    #else
                CheckAnim(anim);
                if (anim[name] == null)
                {
                    endCallback.Call(name);
                    return false;
                } else 
                {
                    if (endCallback != null)
                    {
                        anim.AddOneShotCallback(endCallback);
                    }
                    AddEndEventAtClip(anim, name);
                    return anim.Play(name);
                }
    #endif
		    } else {
			    log.Error("Animation can't be played because {0} is not enabled", anim.transform.GetScenePath());
			    endCallback.Call(name);
			    return false;
		    }
	    }

	    public static void Sample(this Animation anim, AnimationClip clip, float time = 0) {
		    anim.Sample (clip.name, time);
	    }

	    public static void Sample(this Animation anim, string clipName, float time = 0) {
		    anim [clipName].speed = -1;
		    anim [clipName].time = time;
		    anim.Sample();
	    }

	    public static void SetBackward(this Animation anim, AnimationClip clip) {
		    anim[clip.name].speed = -1;
		    anim[clip.name].time = anim[clip.name].length;
		    anim.Sample();
	    }

	    public static void SetForward(this Animation anim, AnimationClip clip) {
		    anim[clip.name].speed = 1;
		    anim[clip.name].time = 0;
		    anim.Sample();
	    }

	    public static void Fastforward(this Animation anim, string clipName) {
		    anim.Play(clipName);
		    anim[clipName].normalizedTime = 1;
		    anim.Sample();
	    }

	    public static void CrossFadeEx(this Animation anim, AnimationClip clip, Action<string> endCallback = null) {
		    anim.CrossFadeEx(clip.name, endCallback);
	    }

	    public static void CrossFadeEx(this Animation anim, string name, Action<string> endCallback = null) {
		    CheckAnim(anim);
		    anim.AddOneShotCallback(endCallback);
		    anim.CrossFade(name);
	    }
	
	    public static void CrossFadeEx(this Animation anim, string name, float time, Action<string> endCallback = null) {
		    CheckAnim(anim);
		    anim.AddOneShotCallback(endCallback);
		    anim.CrossFade(name, time);
	    }

	    public static void SetCallback(this Animation anim, Action<string> endCallback) {
		    AnimEventReceiver receiver = anim.FindComponent<AnimEventReceiver>();
		    receiver.SetCallback(endCallback);
	    }

	    public static void AddOneShotCallback(this Animation anim, Action<string> endCallback) {
		    AnimEventReceiver receiver = anim.FindComponent<AnimEventReceiver>();
		    receiver.AddOneShotCallback(endCallback);
	    }

	    private static void AddEndEventAtClip(Animation anim, string clipName) {
		    // add 'clip end' event to all clips in the animation
		    foreach (AnimationState a in anim) {
			    AnimationClip clip = anim.GetClip(clipName);
			    if (clip != null && !registeredClips.Contains(clip)) {
				    registeredClips.Add(clip);
                    float endTime = clip.length-1f/clip.frameRate;
				    clip.AddStringEvent(AnimEventReceiver.END_CALLBACK, endTime, a.name);
			    }
		    }
	    }

	    public static List<AnimationClip> GetAllClips(this Animation anim) {
		    List<AnimationClip> clips = new List<AnimationClip>(anim.GetClipCount());
		    foreach (AnimationState a in anim) {
			    AnimationClip clip = anim.GetClip(a.name);
			    if (clip != null) {
				    clips.Add(clip);
			    }
		    }
		    return clips;
	    }

	    public static void Skip(this Animation anim) {
		    foreach (AnimationState s in anim) {
			    if (anim.IsPlaying(s.name)) {
				    s.time = s.length-Time.unscaledDeltaTime;
			    }
		    }
	    }
    }
}

