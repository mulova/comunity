using UnityEngine;
using System;
using System.Collections;
using commons;
using System.Ex;

[RequireComponent(typeof(Animation))]
public class IgnoreTimeScale : MonoBehaviour
{
    public static readonly Loggerx log = LogManager.GetLogger(typeof(Animation));
	public bool ignorePause = true;
	private Animation anim;

	public Animation GetAnimation() {
		if (anim == null) {
			anim = GetComponent<Animation>();
		}
		return anim;
	}

	void OnEnable() {
		if (GetAnimation().clip != null && GetAnimation().playAutomatically) {
			Play(GetAnimation().clip);
		}
	}

	public void Play(AnimationClip clip, Action onComplete = null)
	{
		if (clip != null) {
			StartCoroutine(PlayConstantSpeed(clip.name, onComplete));
		} else {
            log.context = this;
			log.Error("Missing animation");
			onComplete.Call();
		}
	}
	
	public void Play(string clipName, Action onComplete = null)
	{
		StartCoroutine(PlayConstantSpeed(clipName, onComplete));
	}
	
	private IEnumerator PlayConstantSpeed(string clipName, Action onComplete = null)
	{
		Animation anim = GetAnimation();
		AnimationState _currState = anim[clipName];
		if (_currState != null) {
			bool isPlaying = true;
			float _progressTime = 0F;
			float _timeAtLastFrame = 0F;
			float _timeAtCurrentFrame = 0F;
			float deltaTime = 0F;
			
			
			anim.Play(clipName);
			
			_timeAtLastFrame = Time.realtimeSinceStartup;
			while (isPlaying) 
			{
				if (Time.timeScale != 0 || ignorePause) {
					_timeAtCurrentFrame = Time.realtimeSinceStartup;
					deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
					_timeAtLastFrame = _timeAtCurrentFrame; 
					
					_progressTime += deltaTime;
					_currState.normalizedTime = _progressTime / _currState.length; 
					anim.Sample ();
					
					
					if (_progressTime >= _currState.length) 
					{
						if(_currState.wrapMode != WrapMode.Loop)
						{
							isPlaying = false;
						}
						else
						{
							_progressTime = 0.0f;
						}
					}
				}
				
				yield return new WaitForEndOfFrame();
			}
			yield return null;
			if(onComplete != null)
			{
				onComplete();
			} 
		} else {
            log.context = this;
			log.Error("Missing animation");
			onComplete.Call();
		}
	}
}
