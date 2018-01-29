//#define ANDROID_LOCALE_FIX
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
public static class Android
{
	public static string GetCountry() {
#if ANDROID_LOCALE_FIX
		using (AndroidJavaClass cls = new AndroidJavaClass("java.util.Locale")) {
			using (AndroidJavaObject locale = cls.CallStatic<AndroidJavaObject>("getDefault")) {
				return locale.Call<string>("getCountry");
			}
		}
#else
		return string.Empty;
#endif
	}
}
#endif