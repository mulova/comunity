using System;
using System.Collections.Generic;
using UnityEngine;
using mulova.commons;

#if UNITY_ANDROID
namespace etc
{
	public static class AndroidAudioManager
	{
		public static readonly ILogger log = LogManager.GetLogger(typeof(Android));
		public const int STREAM_VOICE_CALL = 0;
		/* The audio stream for system sounds */
		public const int STREAM_SYSTEM = 1;
		/* The audio stream for the phone ring and message alerts */
		public const int STREAM_RING = 2;
		/* The audio stream for music playback */
		public const int STREAM_MUSIC = 3;
		/* The audio stream for alarms */
		public const int STREAM_ALARM = 4;
		/* The audio stream for notifications */
		public const int STREAM_NOTIFICATION = 5;
		/* @hide The audio stream for phone calls when connected on bluetooth */
		public const int STREAM_BLUETOOTH_SCO = 6;
		/* @hide The audio stream for enforced system sounds in certain countries (e.g camera in Japan) */
		public const int STREAM_SYSTEM_ENFORCED = 7;
		/* @hide The audio stream for DTMF tones */
		public const int STREAM_DTMF = 8;
		/* @hide The audio stream for text to speech (TTS) */
		public const int STREAM_TTS = 9;
		//	public const int USE_DEFAULT_STREAM_TYPE = Integer.MIN_VALUE;

		//    Whether to include ringer modes as possible options when changing volume.
		public const int FLAG_ALLOW_RINGER_MODES = 2;
		//    Whether to play a sound when changing the volume.
		public const int FLAG_PLAY_SOUND = 4;
		//    Removes any sounds/vibrate that may be in the queue, or are playing (related to changing volume).
		public const int FLAG_REMOVE_SOUND_AND_VIBRATE = 8;
		// Show a toast containing the current volume.
		public const int FLAG_SHOW_UI = 1;
		//    Whether to vibrate if going into the vibrate ringer mode.
		public const int FLAG_VIBRATE = 16;

		public const int AUDIOFOCUS_GAIN = 1;

		public const int RINGER_MODE_SILENT = 0;
		public const int RINGER_MODE_VIBRATE = 1;
		public const int RINGER_MODE_NORMAL = 2;
		public const int VIBRATE_TYPE_RINGER = 0;
		public const int VIBRATE_TYPE_NOTIFICATION = 1;
		public const int VIBRATE_SETTING_OFF = 0;
		public const int VIBRATE_SETTING_ON = 1;
		public const int VIBRATE_SETTING_ONLY_SILENT = 2;

		private static string AUDIO_SERVICE;

		private static AndroidJavaObject GetAudioManager(AndroidActivity a)
		{
			try
			{
				if (AUDIO_SERVICE == null)
				{
					AndroidJavaClass context = a.GetClass("android.content.Context"); 
					AUDIO_SERVICE = context.GetStatic<string>("AUDIO_SERVICE");
				}
				// AudioManager am = (AudioManager)context.getSystemService(Context.AUDIO_SERVICE);
				return a.Call<AndroidJavaObject>("getSystemService", AUDIO_SERVICE);
			} catch (Exception ex)
			{
				log.Error(ex);
				return null;
			}
		}

		public static void Set(AndroidActivity a, string methodName, params object[] param)
		{
			if (Application.isEditor)
			{
				return;
			}
			try
			{
				using (AndroidJavaObject audioManager = GetAudioManager(a))
				{
					audioManager.Call(methodName, param);
				}
			} catch (Exception ex)
			{
				log.Error(ex);
			}
		}

		public static T Get<T>(AndroidActivity a, string methodName, T defValue, params object[] param)
		{
			if (Application.isEditor)
			{
				return defValue;
			}
			try
			{
				using (AndroidJavaObject audioManager = GetAudioManager(a))
				{
					return audioManager.Call<T>(methodName, param);
				}
			} catch (Exception ex)
			{
				log.Error(ex);
				return defValue;
			}
		}


		/// <summary>
		/// Returns the current volume index for a particular stream.
		/// Parameters:
		/// streamType The stream type whose volume index is returned.
		/// Returns:
		/// The current volume index for the stream.
		/// See also:
		/// 	getStreamMaxVolume(int)
		/// 	setStreamVolume(int,int,int)
		/// </summary>
		/// <returns>The volume.</returns>
		/// <param name="audioType">AndroidAudioSystem.STREAM_</param>
		public static int GetVolume(AndroidActivity a, int audioType)
		{
			try
			{
				return Get<int>(a, "getStreamVolume", 8, audioType);
			} catch (Exception ex)
			{
				log.Error(ex);
				return 0;
			}
		}

		public static void SetVolume(AndroidActivity a, int audioType, int volume)
		{
			try
			{
				Set(a, "setStreamVolume", audioType, volume, 0);
			} catch (Exception ex)
			{
				log.Error(ex);
			}
		}

		/// <summary>
		/// Sets the ringer mode.
		/// Silent mode will mute the volume and will not vibrate. Vibrate mode will mute the volume and vibrate. Normal mode will be audible and may vibrate according to user settings.
		/// Parameters:
		/// See also:
		/// getRingerMode()
		/// </summary>
		/// <param name="a">activity</param>
		/// <param name="ringerMode">The ringer mode, one of RINGER_MODE_NORMAL, RINGER_MODE_SILENT, or RINGER_MODE_VIBRATE.</param>
		public static void SetRingerMode(AndroidActivity a, int ringerMode)
		{
			// setRingerMode(AudioManager.RINGER_MODE_NORMAL);
			try
			{
				Set(a, "setRingerMode", ringerMode);
			} catch (Exception ex)
			{
				log.Error(ex.Message, ex);
			}
		}
	}	
}


#endif