using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
public class AndroidActivity : IDisposable
{
	private AndroidJavaClass activityCls;
	private AndroidJavaObject activityInst;
	private string name;
	private Dictionary<string, AndroidJavaClass> classes = new Dictionary<string, AndroidJavaClass>();
	private Dictionary<string, AndroidJavaObject> objects = new Dictionary<string, AndroidJavaObject>();

	public AndroidActivity(string activityName = "com.unity3d.player.UnityPlayer") {
		if (Application.isEditor) {
			return;
		}
		activityCls = new AndroidJavaClass(activityName); 
		activityInst = activityCls.GetStatic<AndroidJavaObject>("currentActivity");
	}

	public T Call<T>(string methodName, params object[] args) {
		if (Application.isEditor) {
			return default(T);
		}
		return activityInst.Call<T>(methodName, args);
	}

	public void Call(string methodName, params object[] args) {
		if (!Application.isEditor) {
			activityInst.Call(methodName, args);
		}
	}

	public AndroidJavaClass GetClass(string clsName) {
		AndroidJavaClass c = null;
		if (!classes.TryGetValue(clsName, out c)) {
			c = new AndroidJavaClass(clsName); 
			classes[clsName] = c;
		}
		return c;
	}

	public AndroidJavaObject GetActivity() {
		return activityInst;
	}


	public void Dispose() {
		if (Application.isEditor) {
			return;
		}
		activityCls.Dispose();
		activityInst.Dispose();
		foreach (KeyValuePair<string, AndroidJavaClass> pair in classes) {
			pair.Value.Dispose();
		}
		classes.Clear();
		foreach (KeyValuePair<string, AndroidJavaObject> pair in objects) {
			pair.Value.Dispose();
		}
		objects.Clear();
	}

	public void ShowAlertMessage(string title, string message, string okButton) {
		Call("openAlert", title, message, okButton);
	}

	public void ShowNotification(string title, string message, string iconAction, string textAction) {
		Call("showNotification", title, message, iconAction, textAction);
	}

	public void ShowToast(string message) {
		Call("showToast", message);
	}

	public string GetResourceString(string resClassName, string resVarName) {
		AndroidJavaClass resCls = new AndroidJavaClass(resClassName);
		int resId = resCls.GetStatic<int>(resVarName);
		return Call<string>("getString", resId);
	}
}
#endif