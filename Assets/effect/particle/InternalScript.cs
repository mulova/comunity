using System;
using UnityEngine;
using commons;
using comunity;
using UnityEngine.Ex;

namespace effect
{
	public class InternalScript : MonoBehaviour
	{
		private Loggerx _log;
		private Invoker invoker;

		private GameObject _go;
		public GameObject go
		{
			get {
				if (_go == null)
				{
					_go = gameObject;
				}
				return _go;
			}
		}

		private Transform _trans;
		public Transform trans
		{
			get {
				if (_trans == null)
				{
					_trans = transform;
				}
				return _trans;
			}
		}

		public Loggerx log
		{
			get
			{
				if (_log == null)
				{
					_log = LogManager.GetLogger(GetType());
					_log.context = this;
				}
				return _log;
			}
			set
			{
				_log = value;
				_log.context = this;
			}
		}

		public Type logType
		{
			set
			{
				log = LogManager.GetLogger(value);
			}
		}

		public string logName
		{
			set
			{
				log = LogManager.GetLogger(value);
			}
		}

		public void Invoke(Action action, float delay) {
			Invoke(action.Method.Name, delay);
		}

		public void CancelInvoke(Action action) {
			CancelInvoke(action.Method.Name);
		}

		/// <summary>
		/// thread safe call.
		/// ignore time-scale
		/// </summary>
		/// <param name="action">Action.</param>
		public void InvokeNextFrame(Action action)
		{
			if (invoker == null)
			{
				invoker = gameObject.FindComponent<Invoker>();
			}
			invoker.InvokeLater(action);
		}
	}
}
