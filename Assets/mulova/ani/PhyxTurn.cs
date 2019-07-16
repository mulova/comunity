using System;
using UnityEngine;
using comunity;

namespace ani {
	public class PhyxTurn : SeqTurn {
		private PhyxObj obj;
		
		public PhyxTurn(PhyxObj obj) {
			this.obj = obj;
		}
		
		public void Rewind() {
			obj.Stop();
		}
		
		public void Play(Action callback) {
			obj.Play(callback);
		}
		
		public void Skip() {
			obj.Skip();
		}

		public void Stop() {
			obj.Stop();
		}
		
		public override string ToString()
		{
			return "Phyx "+obj.name;
		}
	}
}