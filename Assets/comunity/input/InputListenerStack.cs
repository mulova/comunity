using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using commons;

namespace comunity
{
	public class InputListenerStack : SingletonBehaviour<InputListenerStack>
	{
		private WeakStack<InputListener> stack = new WeakStack<InputListener> ();
		private InputListener defaultListener;

		public void OnButton(KeyCode button, ButtonState state)
		{
			InputListener l = GetStackTop ();
			if (l != null) {
				try {
					l.OnButton (button, state);
				} catch (Exception ex) {
					log.Error (ex);
				}
			}
		}
		
		public void Clear()
		{
			defaultListener = null;
			stack.Clear ();
		}
		
		public void Push(InputListener l)
		{
			InputListener stackTop = GetStackTop ();
			if (stackTop != null) {
				stackTop.OnFocus (false, l);
			}
			if (!stack.Contains (l)) {
				stack.Push (l);
			} else {
				log.Warn ("{0} is already in InputListenerStack", l);
			}
		}
		
		public void Pop(InputListener l)
		{
			if (stack.Peek () == l) {
				stack.Pop ();
				InputListener stackTop = GetStackTop ();
				if (stackTop != null) {
					stackTop.OnFocus (true, l);
				}
			} else if (!stack.IsEmpty ()) {
				// Remove element in the middle of the stack
				log.Warn ("Stack Top: {0}, request:{1}", stack.Peek (), l);
				Stack<InputListener> backup = new Stack<InputListener> ();
				while (!stack.IsEmpty() && stack.Peek() != l) {
					backup.Push (stack.Pop ());
				}
				stack.Pop ();
				while (backup.NotEmpty()) {
					stack.Push (backup.Pop ());
				}
			} else {
				log.Warn ("no stack pop for {0}", l);
			}
		}
		
		public void SetDefaultListener(InputListener l)
		{
			defaultListener = l;
		}
		
		public InputListener GetStackTop()
		{
			return stack.IsEmpty () ? defaultListener : stack.Peek ();
		}
		
		public Object GetActiveTopmost()
		{
			foreach (commons.WeakReference<InputListener> e in stack) {
				Object o = e.Target as Object;
				if (e != null) {
					MonoBehaviour c = o as MonoBehaviour;
					if (c != null) {
						if (c.enabled) {
							return c;
						}
					} else {
						GameObject go = o as GameObject;
						if (go != null && go.activeInHierarchy) {
							return go;
						}
					}
				}
			}
			return defaultListener as Object;
		}
		
		public void SetAllFocus(bool focus, object triggerObj)
		{
			foreach (InputListener l in stack) {
				l.OnFocus (focus, triggerObj);
			}
		}
		
		public void ForEach(Predicate<InputListener> predicate)
		{
            List<commons.WeakReference<InputListener>> copy = new List<commons.WeakReference<InputListener>> (stack);
            foreach (commons.WeakReference<InputListener> r in copy) {
				InputListener l = r.Target;
				if (l != null) {
					if (!predicate (l)) {
						return;
					}
				}
			}
		}
		
		public bool Contains(InputListener l)
		{
			foreach (InputListener listener in stack) {
				if (listener == l) {
					return true;
				}
			}
			return false;
		}
	}
}

