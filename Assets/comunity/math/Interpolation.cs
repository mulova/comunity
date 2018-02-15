#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace comunity {
	public class Interpolation {
		public const bool INCREASE = true;
		public const bool DECREASE = false;
		
		protected float delay;
		protected float[] intervals;
		protected float[] values;
		protected WrapMode wrapMode = WrapMode.Clamp;
		protected bool enabled = true;
		protected float speed = 1.0f;
		protected float unit;
		
		protected int fromIndex;
		protected int toIndex;
		protected float ratio; 
		protected float interval;
		protected float now;
		protected float value;
		
		private LinkedList<IValueListener> listeners;
		
		private bool changed;
		private float oldValue;
		
		private bool initialization;
		
		public Interpolation() {
		}
		
		/**
		 * @param timeIntervals
		 *            time intervals. such as { 0.1f, 0.2f, 0.1f }
		 * @param value
		 *            one more array element than timeIntervals. such as { 1f, 3f, 2f, 4f }
		 */
		public Interpolation(float[] timeIntervals, float[] value) {
			if (timeIntervals[0] == 0)
				throw new Exception();
			Init(timeIntervals, value);
		}
		
		public float Delay {
			set { delay = value; }
			get { return delay; }
		}
		
		public WrapMode Wrap {
			set { wrapMode = value; }
			get { return wrapMode; }
		}
		
		public float Unit {
			set { this.unit = value; }
			get { return unit; }
		}
		
		public float Speed {
			set
			{
				if (speed <= 0)
				{
					throw new Exception("Must be positive");
				}
				this.speed = value;
			}
			get { return speed; }
		}
		
		public bool Enabled {
			get { return enabled; }
			set { this.enabled = value; }
		}
		
		public void Init(float[] interval, float[] value) {
			if (interval.Length != value.Length - 1)
				throw new Exception("value Length should be lengther than interval");
			this.intervals = interval;
			this.values = value;
			Reset();
		}
		
		public void AddListener(IValueListener listener) {
			if (listeners == null) {
				listeners = new LinkedList<IValueListener>();
			}
			listeners.AddLast(listener);
		}
		
		public void RemoveListener(IValueListener listener) {
			if (listeners == null)
				return;
			listeners.Remove(listener);
		}
		
		public bool IsIncrease() {
			return fromIndex < toIndex;
		}
		
		public float GetValue() {
			return value;
		}
		
		private void UpdateValue() {
			oldValue = value;
			value = Interpolate(values[fromIndex], values[toIndex], now*ratio);
			changed = value != oldValue;
			if (changed) {
				DispatchValueChanged(value);
			}
		}
		
		public float Interpolate(float v1, float v2, float ratio) {
			float v = (v2 - v1) * ratio + v1; 
			if (unit != 0) {
				v -= v % unit;
			}
			return v;
		}
		
		
		public void Reset() {
			initialization = true;
			changed = true;
			now = 0;
			fromIndex = -1;
			toIndex = 0;
			MoveBoundary();
			UpdateValue();
			DispatchInit();
			initialization = false;
		}
		
		public void ResetLast() {
			initialization = true;
			now = 0;
			fromIndex = values.Length - 2;
			toIndex = values.Length - 1;
			MoveBoundary();
			UpdateValue();
			DispatchInit();
			initialization = false;
		}
		
		public int From {
			get { return fromIndex; }
		}
		
		public int To {
			get { return toIndex; }
		}
		
		private void NextInterval() {
			fromIndex++;
			toIndex++;
			if (toIndex < values.Length)
				return;
			if (!initialization)
				DispatchTurnOver(true, values[fromIndex]);
			switch (Wrap) {
			case WrapMode.Loop:
				fromIndex = 0;
				toIndex = 1;
				break;
			case WrapMode.Clamp:
				now = 0;
				fromIndex = values.Length - 1;
				toIndex = values.Length - 2;
				if (!initialization) {
					Enabled = false;
				}
				break;
			case WrapMode.PingPong:
				fromIndex = values.Length - 1;
				toIndex = values.Length - 2;
				break;
			}
		}
		
		private void PreviousInterval() {
			fromIndex--;
			toIndex--;
			if (toIndex >= 0)
				return;
			if (!initialization)
				DispatchTurnOver(false, values[0]);
			switch (Wrap) {
			case WrapMode.Loop:
				fromIndex = 0;
				toIndex = 1;
				break;
			case WrapMode.Clamp:
				now = 0;
				fromIndex = 0;
				toIndex = 1;
				if (!initialization) {
					UpdateValue();
					Enabled = false;
				}
				break;
			case WrapMode.PingPong:
				fromIndex = 0;
				toIndex = 1;
				break;
			}
		}
		
		public void Reverse() {
			int temp = fromIndex;
			fromIndex = toIndex;
			toIndex = temp;
			now = interval - now;
		}
		
		public void SetDirection(bool increase) {
			if (IsIncrease() != increase)
				Reverse();
		}
		
		public void SetTime(float time) {
			Reset();
			Update(time);
		}
		
		private void DispatchTurnOver(bool end, float value) {
			if (this.listeners != null) {
				foreach (IValueListener listener in listeners) {
					listener.TurnOver(end, value);
				}
			}
		}
		
		private void DispatchValueChanged(float value) {
			if (this.listeners != null) {
				foreach (IValueListener listener in listeners) {
					listener.ValueChanged(value);
				}
			}
		}
		
		private void DispatchInit() {
			if (this.listeners != null) {
				foreach (IValueListener listener in this.listeners) {
					listener.Init();
				}
			}
		}
		
		public bool IsChanged() {
			return this.changed;
		}
		
		public void Update(float time) {
			if (Enabled) {
				if (delay > 0) {
					delay -= time;
					if (delay > 0)
						return;
				}
				this.now += time * Speed;
				while (this.now >= this.interval && Enabled) {
					this.now -= this.interval;
					MoveBoundary();
				}
			}
			UpdateValue();
		}
		
		private void MoveBoundary() {
			if (IsIncrease())
				NextInterval();
			else
				PreviousInterval();
			
			if (IsIncrease()) {
				interval = intervals[fromIndex];
			} else {
				interval = intervals[toIndex];
			}
			if (interval == 0) {
				ratio = 1;
			} else {
				ratio = 1 / interval;
			}
		}
	}
}
#endif