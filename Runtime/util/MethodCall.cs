//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Reflection;
using mulova.commons;
using mulova.unicore;
using UnityEngine;
using UnityEngine.Ex;
using ILogger = mulova.commons.ILogger;

namespace mulova.comunity
{
    public class MethodCall : MonoBehaviour
	{
		public static ILogger log = LogManager.GetLogger(typeof(MethodCall));
		public static readonly BindingFlags FLAGS = (BindingFlags.Public|BindingFlags.Instance|BindingFlags.FlattenHierarchy)&~BindingFlags.SetProperty&~BindingFlags.GetProperty;
		public MonoBehaviour target;
		public string methodName;
		public object methodParam;
		/// <summary>
		/// Make target's GameObject active if it is inactive
		/// </summary>
		public bool forceActive;

		void Awake()
		{
			if (Platform.isEditor)
			{
				if (target != null&&!string.IsNullOrEmpty(methodName))
				{
					MethodInfo method = target.GetType().GetMethod(methodName, FLAGS);
					if (method == null)
					{
						method = target.GetType().GetMethod(methodName, new Type[] { typeof(object) });
						if (method == null)
						{
							throw new UnityException("No "+target.GetType().Name+"."+methodName+"()");
						}
					}
				}
			}
		}

		public void InvokeMethod()
		{
			if (forceActive&&!target.gameObject.activeSelf)
			{
				target.gameObject.SetActive(true);
			}
			if (Platform.isEditor&&!target.gameObject.activeInHierarchy)
			{
				log.Error("{0} is Inactive", target.transform.GetScenePath());
			}
			if (methodParam != null)
			{
				log.Debug("[{0}] {1}({2})", target, methodName, methodParam);
				target.SendMessage(methodName, methodParam, SendMessageOptions.RequireReceiver);
			} else
			{
				log.Debug("[{0}] {1}()", target, methodName);
				target.SendMessage(methodName, gameObject, SendMessageOptions.RequireReceiver);
			}
		}

		public void OnClick()
		{
			InvokeMethod();
		}
	}
}
