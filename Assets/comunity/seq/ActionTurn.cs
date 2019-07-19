using System;
using System.Ex;
using commons;

namespace comunity {
	public class ActionTurn : SeqTurn {
		private Seq.Act action;
		private Action callback;

		public ActionTurn(Seq.Act action) {
			this.action = action;
		}

		public void Play(Action callback) {
			if (action != null) {
				this.callback = callback;
				action(OnEnd);
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
			} else {
				return "-";
			}
		}
	}
}