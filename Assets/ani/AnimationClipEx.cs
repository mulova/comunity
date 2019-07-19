using System.Collections.Generic;

namespace UnityEngine.Ex
{
    public static class AnimationClipEx {
	    public static List<AnimationEvent> GetEvents(this AnimationClip clip, string funcName) {
		    List<AnimationEvent> events = new List<AnimationEvent>();
		    if (clip.events != null) {
			    foreach (AnimationEvent e in clip.events) {
				    if (e.functionName == funcName) {
					    events.Add(e);
				    }
			    }
		    }
		    return events;
	    }

	    public static AnimationEvent GetEvent(this AnimationClip clip, string funcName) {
		    return GetEvent(clip, funcName, string.Empty);
	    }

	    public static AnimationEvent GetEvent(this AnimationClip clip, string funcName, string param) {
		    if (clip.events != null) {
			    foreach (AnimationEvent e in clip.events) {
				    if (e.functionName == funcName && e.stringParameter == param) {
					    return e;
				    }
			    }
		    }
		    return null;
	    }

	    public static void AddStringEvent(this AnimationClip clip, string funcName, float time, string param) {
		    AnimationEvent evt = new AnimationEvent();
		    evt.functionName = funcName;
		    evt.time = time;
		    evt.stringParameter = param;
		    clip.AddEvent(evt);
	    }

	    public static void AddEvent(this AnimationClip clip, string funcName, float time) {
		    AnimationEvent evt = new AnimationEvent();
		    evt.functionName = funcName;
		    evt.time = time;
		    clip.AddEvent(evt);
	    }
    }
}
