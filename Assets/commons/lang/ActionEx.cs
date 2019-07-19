//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Linq;

namespace System.Ex
{
	public static class ActionEx {
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void Call (this Action handler) {
			if(handler != null) {
				handler();
			}
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void Call<T> (this Action<T> handler, T t) {
			if(handler != null) {
				handler(t);
			}
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void Call<T, U> (this Action<T, U> handler, T t, U u) {
			if(handler != null) {
				handler(t, u);
			}
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void Call<T, U, V>(this Action<T, U, V> handler, T t, U u, V v) {
			if (handler != null) {
				handler(t, u, v);
			}
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void Call<T, U, V, W>(this Action<T, U, V, W> handler, T t, U u, V v, W w) {
			if (handler != null) {
				handler(t, u, v, w);
			}
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void CallAfterRelease(ref Action a) {
			if(a != null) {
				Action b = a;
				a = null;
				b();
			}
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void CallAfterRelease<T> (ref Action<T> a, T t) {
			if(a != null) {
				Action<T> b = a;
				a = null;
				b(t);
			}
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void CallAfterRelease<T, U> (ref Action<T, U> a, T t, U u) {
			if(a != null) {
				Action<T, U> b = a;
				a = null;
				b(t, u);
			}
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void CallAfterRelease<T, U, V>(ref Action<T, U, V> a, T t, U u, V v) {
			if(a != null) {
				Action<T, U, V> b = a;
				a = null;
				b(t, u, v);
			}
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void CallAfterRelease<T, U, V, W>(ref Action<T, U, V, W> a, T t, U u, V v, W w) {
			if(a != null) {
				Action<T, U, V, W> b = a;
				a = null;
				b(t, u, v, w);
			}
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static R[] Call<R>(this Func<R> handler) {
			if (handler == null) return new R[0];
			return
				(from Func<R> f in handler.GetInvocationList()
					select f()).ToArray();
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static R[] Call<R, T>(this Func<T, R> handler, T t) {
			if (handler == null) return new R[0];
			return
				(from Func<T, R> f in handler.GetInvocationList()
					select f(t)).ToArray();
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static R[] Call<R, T, U>(this Func<T, U, R> handler, T t, U u) {
			if (handler == null) return new R[0];
			return
				(from Func<T, U, R> f in handler.GetInvocationList()
					select f(t, u)).ToArray();
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static R[] Call<R, T, U, V>(this Func<T, U, V, R> handler, T t, U u, V v) {
			if (handler == null) return new R[0];
			return
				(from Func<T, U, V, R> f in handler.GetInvocationList()
					select f(t, u, v)).ToArray();
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static R[] Call<R, T, U, V, W>(this Func<T, U, V, W, R> handler, T t, U u, V v, W w) {
			if (handler == null) return new R[0];
			return
				(from Func<T, U, V, W, R> f in handler.GetInvocationList()
					select f(t, u, v, w)).ToArray();
		}
	}
}
