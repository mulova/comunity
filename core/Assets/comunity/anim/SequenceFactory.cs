using System;
using System.Collections.Generic;
using commons;

namespace core {
	public class SequenceFactory : SingletonBehaviour<SequenceFactory>
	{
		private MultiMap<string, Seq> seqs = new MultiMap<string, Seq>();

		public Seq Create(string id, bool errorTolerant) {
			Seq seq = new Seq(errorTolerant);
			seqs.Add(id, seq);
			return seq;
		}

		void OnDisable() {
			Dispose();
		}

		public void Dispose() {
			foreach (string id in seqs.Keys) {
				Dispose(id);
			}
			seqs.Clear();
		}

		public void Dispose(string id) {
			List<Seq> slot = seqs[id];
			if (slot.IsNotEmpty()) {
				foreach (Seq s in slot) {
					s.Stop();
				}
			}
			seqs.Remove(id);
		}
	}
}

 