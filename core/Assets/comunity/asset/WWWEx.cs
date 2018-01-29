//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using Object = UnityEngine.Object;
using comunity;

namespace UnityEngine
{
	public static class WWWEx
	{
		public static void DisposeEx(this WWW www)
		{
			if (www.error.IsEmpty())
			{
				if (www.assetBundle != null)
				{
					www.assetBundle.Unload(false);
				}
			}
			www.Dispose();
		}

		public static void SetCallback(this WWW www, Action callback)
		{
			Threading.inst.StartCoroutine(SetCallbackImpl(www, callback));
		}

		public static IEnumerator SetCallbackImpl(this WWW www, Action callback)
		{
			yield return www;
			callback.Call();
		}

		public static T GetAsset<T>(this WWW www) where T:class
		{
			if (www.assetBundle != null)
			{
				return www.assetBundle.mainAsset as T;
			} else if (www.textureNonReadable != null)
			{
				return www.textureNonReadable as T;
			} else if (www.bytes != null)
			{
				return www.bytes as T;
			} else
			{
				return default(T);
			}
		}

		public static T[] GetAssets<T>(this WWW www) where T:Object
		{
			if (www.assetBundle != null)
			{
				// TODOM use async
				Object[] arr = www.assetBundle.LoadAllAssets<T>();
				return arr.Convert<Object, T>();
			} else
			{
				return new T[0];
			}
		}
	}
}
