using System;
using UnityEngine;

namespace core {
	public class ActionTurn : SeqTurn {
		private Seq.Act action;
		private Action callback;

		private Action immediateAction;
		
		public ActionTurn(Seq.Act action) {
			this.action = action;
		}

		public ActionTurn(Action action) {
			this.immediateAction = action;
		}
		
		public void Play(Action callback) {
			if (action != null) {
				this.callback = callback;
				action(OnEnd);
			} else {
				immediateAction.Call();
				callback.Call();
			}
		}

		private void OnEnd() {
			this.callback.Call();
			Stop();
		}
		
		public void Skip() {
			OnEnd();
		}

		public void Stop() {
			this.callback = null;
		}

		public override string ToString()
		{
			if (action != null && action.Target != null) {
				return string.Format("{0}.{1}()", action.Target.GetType().Name, action.Method.Name);
			} else if (immediateAction != null && immediateAction.Target != null) {
				return string.Format("{0}.{1}()", immediateAction.Target.GetType().Name, immediateAction.Method.Name);
			} else {
				return "-";
			}
		}
	}
}