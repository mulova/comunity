//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System;

/// <summary>
/// Run the actions at the specified time.
/// </summary>
public class ActionTimer : MonoBehaviour
{
	private List<TimerEntry> times = new List<TimerEntry>();
	private float time;
	private int index = int.MaxValue;
	
	public void Add(float time, Action func) {
		this.times.Add(new TimerEntry(time, func));
	}
	
	public void AddLast(float timeAdded, Action func) {
		Add(GetLastTime()+timeAdded, func);
	}
	
	public float GetLastTime() {
		return times.Count>0? times[times.Count-1].time: 0;
	}
	
	public void Clear() {
		times.Clear();
	}
	
	public void Stop() {
		time = int.MaxValue;
		index = times.Count;
		enabled = false;
	}
	
	public void Begin() {
		time = 0;
		index = 0;
		enabled = true;
	}
	
	void Update() {
		time += Time.deltaTime;
		while (index < times.Count && time >= times[index].time) {
			times[index].call();
			++index;
		}
		if (index >= times.Count) {
			Stop();
		}
	}
	
	private class TimerEntry : IComparable<TimerEntry> {
		public float time;
		public Action call;
		
		public TimerEntry(float time, Action call) {
			this.time = time;
			this.call = call;
		}

		public int CompareTo(TimerEntry other) {
			float delta = this.time - other.time;
			if (delta < 0) {
				return -1;
			} else if (delta == 0) {
				return 0;
			} else {
				return 1;
			}
		}
	}
}
