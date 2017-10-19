using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using commons;

namespace core {
	public class Seq : SeqTurn {
		public delegate void Act(Action callback);
		private Queue<ConcurrentTurn> queue = new Queue<ConcurrentTurn>();
		private ConcurrentTurn tail;
		private ConcurrentTurn current;
		private bool skip;
		private int animLayer;
		private readonly ConcurrentTurn NO_SKIP = new ConcurrentTurn(new AnimTurn());
        public static readonly Loggerx log = LogManager.GetLogger(typeof(Seq));
        private TimerControl timer;
		private bool errorTolerant;

		public Seq(bool errorTolerant) {
			this.errorTolerant = errorTolerant;
		}
		
		public void Add(SeqTurn turn) {
			if (tail != null) {
				Next();
			}
			AddConcurrent(turn);
		}

		public void Add(Seq.Act act) {
			if (act != null) {
				Add(new ActionTurn(act));
			} else {
				log.Info("Null argument");
			}
		}

		public void Add(Action act) {
			if (act != null) {
				Add(new ActionTurn(act));
			} else {
				log.Info("Null argument");
			}
		}
		
		public void AddConcurrent(SeqTurn turn) {
			if (tail == null) {
				Next();
			}
			if (turn != null) {
				tail.Add(turn);
			} else {
				log.Info("Null argument");
			}
		}

		public void AddConcurrent(Seq.Act act) {
			AddConcurrent(new ActionTurn(act));
		}
		
		public void AddConcurrent(Action act) {
			AddConcurrent(new ActionTurn(act));
		}

		public void Delay(float delay)
		{
			if (delay <= 0)
			{
				return;
			}
			if (timer == null) {
				GameObject go = new GameObject("timer");
				timer = go.AddComponent<TimerControl>();
			}
			Add(a=>{
				TimerData timerData = new TimerData(delay, a);
				timer.Add(timerData);
				timer.Begin();
			});
			Next();
		}
		
		public void AddNoSkip() {
			queue.Enqueue(NO_SKIP);
			tail = null;
		}

		/// <summary>
		/// Callback is called after the current action ends.
		/// </summary>
		public void ClearSequence() {
			queue.Clear();
			CleanUp();
			if (timer != null) {
				timer.Stop();
			}
			tail = null;
		}

		private void CleanUp() {
			if (timer != null && timer.gameObject != null) {
				timer.gameObject.DestroyEx();
				timer = null;
			}
		}

		public void Next() {
			tail = new ConcurrentTurn();
			queue.Enqueue(tail);
		}
		
		private Action callback;
		public void Play(Action callback = null) {
			log.Debug("Sequence Start({0})", queue.Count);
			this.skip = false;
			this.callback = callback;
			PlayNext();
		}
		
		public bool IsPlaying() {
			return current != null;
		}
		
		public void Skip() {
			skip = true;
			if (current != null) {
				current.Skip();
			}
		}

		/// <summary>
		/// All the remaining sequences are cleared and callback is not called.
		/// </summary>
		public void Stop() {
			callback = null;
			if (current != null) {
				current.Stop();
			}
			current = null;
			ClearSequence();
		}

		public bool IsEmpty() {
			return queue.Count == 0;
		}
		
		private void PlayNext() {
			if (queue.Count > 0) {
				try {
					current = queue.Dequeue();
					if (current == NO_SKIP) {
						skip = false;
						PlayNext();
					} else {
						log.Debug("Play {0}", current);
						current.Play(PlayNext);
						if (skip && current != null) {
							current.Skip();
						}
					}
				} catch (Exception ex) {
					log.Error(ex);
					if (!errorTolerant) {
						throw ex;
					}
					PlayNext();
				}
			} else {
				log.Debug("Sequence End");
				CleanUp();
				Action c = callback;
				current = null;
				callback = null;
				c.Call();
			}
		}
	}

	public interface SeqTurn {
		void Play(Action callback);
		void Skip();
		void Stop();
	}
}
