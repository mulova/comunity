using System;
using comunity;
using commons;

/// <summary>
/// Send Message to AudioManager
/// </summary>
public class AudioBridge {
	public const string VOID_EVENT_ID = "_AudioGroup_Void";
	public const string FLOAT_EVENT_ID = "_AudioGroup_Float";
	public const string BOOL_EVENT_ID = "_AudioGroup_Bool";
	public const string CONTROL_EVENT_ID = "_AudioGroup_Control";

	public const string BGM_ID = "bgm";
    public const string SFX_ID = "sfx";
	public const string STOP  = "stop";

	public static void Play(string id) {
		Messenger<object>.Broadcast(VOID_EVENT_ID, id);
	}
	public static void Play(object evt, float val) {
		Messenger<object, float>.Broadcast(FLOAT_EVENT_ID, evt.ToText ().ToLower(), val);
	}
	public static void Play(object evt, bool val) {
		Messenger<object, bool>.Broadcast(BOOL_EVENT_ID, evt.ToText ().ToLower(), val);
	}
	public static void Play(object control, string groupName) {
		Messenger<object, object>.Broadcast(CONTROL_EVENT_ID, control, groupName);
	}
}

