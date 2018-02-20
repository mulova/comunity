using System;
using commons;

namespace comunity
{
	public static class ActionDelayEx
	{
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void Delay(this Action handler) {
			if (handler == null) {
				return;
			}
			Threading.InvokeLater(() => { handler.Call(); });
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void Delay<T>(this Action<T> handler, T t) {
			if (handler == null) {
				return;
			}
			Threading.InvokeLater(() => { handler.Call(t); });
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void Delay<T, U>(this Action<T, U> handler, T t, U u) {
			if (handler == null) {
				return;
			}
			Threading.InvokeLater(() => { handler.Call(t, u); });
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void Delay<T, U, V>(this Action<T, U, V> handler, T t, U u, V v) {
			if (handler == null) {
				return;
			}
			Threading.InvokeLater(() => { handler.Call(t, u, v); });
		}
		
		//[System.Diagnostics.DebuggerStepThroughAttribute]
		public static void Delay<T, U, V, W>(this Action<T, U, V, W> handler, T t, U u, V v, W w) {
			if (handler == null) {
				return;
			}
			Threading.InvokeLater(() => { handler.Call(t, u, v, w); });
		}
	}
}



