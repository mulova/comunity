using System;
using UnityEngine;
using commons;
using UnityEngine.Ex;

namespace comunity
{
    public class Script : MonoBehaviour
    {
        private Loggerx _log;
        private Invoker invoker;
        
        private GameObject _go;
        
        public GameObject go
        {
            get
            {
                if (_go == null)
                {
                    _go = gameObject;
                }
                return _go;
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
        
        protected void InvokeInternal(Action action, float delay)
        {
            Invoke(action.Method.Name, delay);
        }
        
        protected void CancelInvoke(Action action)
        {
            CancelInvoke(action.Method.Name);
        }
        /*
    public void InvokeNextFrame(Action action)
    {
        StartCoroutine(_InvokeNextFrame(action));
    }

    private IEnumerator _InvokeNextFrame(Action action)
    {
        yield return null;
        action();
    }
        */
        /// <summary>
        /// thread safe call.
        /// ignore time-scale
        /// </summary>
        /// <param name="action">Action.</param>
        public void InvokeNextFrame(Action action)
        {
            if (invoker == null)
            {
                invoker = gameObject.GetComponentEx<Invoker>();
            }
            invoker.InvokeLater(action);
        }
        
        
    }
}

