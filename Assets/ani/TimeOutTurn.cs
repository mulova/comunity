using UnityEngine;
using System;
using comunity;
using mulova.commons;
using System.Ex;

namespace ani {
	public class TimeOutTurn : SeqTurn {
		private Action callback;
		private float delay;
		private Action beginAction;
		private Coroutine coroutine;
		
		public TimeOutTurn(Action a, float delay) {
			this.beginAction = a;
			this.delay = delay;
		}
		
		public void Rewind() {
			if (coroutine != null) {
				Threading.inst.StopCoroutine(coroutine);
				coroutine = null;
			}
		}
		
		public void Play(Action callback) {
			beginAction();
			this.callback = callback;
			if (delay > 0) {
				coroutine = Threading.inst.Delay(delay, OnComplete);
			} else {
				OnComplete();
			}
		}
		
		public void Skip() {
			Rewind();
			OnComplete();
		}

		public void Stop() {
			Rewind();
			callback = null;
		}
		
		private void OnComplete() {
			coroutine = null;
			callback.Call();
		}
		
		public override string ToString()
		{
			return string.Format("delay {0:N1}", delay);
		}
	}
}
