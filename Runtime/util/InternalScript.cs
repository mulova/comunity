using System;
using UnityEngine;
using mulova.commons;
using UnityEngine.Ex;
using ILogger = mulova.commons.ILogger;

namespace mulova.comunity
{
	public class LogBehaviour : MonoBehaviour
	{
		private ILogger _log;
		private Invoker invoker;

		public ILogger log
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
