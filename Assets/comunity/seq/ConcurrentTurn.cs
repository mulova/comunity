using UnityEngine;
using System.Collections.Generic;
using System;
using commons;
using System.Collections.Generic.Ex;
using System.Ex;

namespace comunity {
	public class ConcurrentTurn : SeqTurn {
		private List<SeqTurn> list = new List<SeqTurn>();
		private Action callback;
		private int count;
		private bool waitForAll = true;
		
		public ConcurrentTurn(params SeqTurn[] turns) {
			Add(turns);
		}

		public void WaitForAll(bool all) {
			this.waitForAll = all;
		}

		public void Add(params SeqTurn[] turns) {
			list.AddRange(turns);
		}
		
		public void Play(Action callback) {
			this.count = 0;
			this.callback = callback;
			if (list.Count > 0) {
				foreach (SeqTurn t in list) {
					t.Play(OnComplete);
				}
			} else {
				callback.Call();
			}
		}
		public void Skip() {
			foreach (SeqTurn t in list) {
				t.Skip();
			}
		}
		public void Stop() {
			callback = null;
			foreach (SeqTurn t in list) {
				t.Stop();
			}
			list.Clear();
		}
		private void OnComplete() {
			count++;
			if (!waitForAll || count == list.Count) {
				callback.Call();
			}
		}
		
		public override string ToString()
		{
            return list.Join("\n");
		}
	}
}
