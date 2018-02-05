#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;

namespace comunity {
	// (int)System.DateTime.Now.Ticks
	/**
	 * Generate random at every NextXXX() method call if the RandomRefreshInterval is EveryTime
	 * Or Generate random value only when Update() is called
	 */
	public class RandomGenerator
	{
		public readonly long seed;
		private PseudoRandom seedGen;
		private Dictionary<string, RandomEntry> rands = new Dictionary<string, RandomEntry>();
		
		public RandomGenerator(long seed) {
			this.seed = seed;
			seedGen = new PseudoRandom(seed); 
		}
		
		public void AddKey(string key, RandomRefreshInterval interval) {
			rands[key] = new RandomEntry(new PseudoRandom(seedGen.nextLong()), interval);
		}
		
		public long GetSeed() {
			return seed;
		}
		
		public long nextLong(string key) {
			return (long)((NextDouble(key)*2-1)*long.MaxValue);
		}
		
		private RandomEntry GetRandom(string key) {
			RandomEntry r = null;
			if (!rands.TryGetValue(key, out r)) {
				AddKey(key, RandomRefreshInterval.EveryTime);
				return rands[key];
			}
			return r;
		}
		
		/**
		 * @param code
		 * @return 0~1 사이 값을 반환한다.
		 */
		public double NextDouble(string key) {
			RandomEntry r = GetRandom(key);
			if (r.interval == RandomRefreshInterval.EveryTime) {
				return r.rand.nextDouble();
			} else {
				return r.val;
			}
		}
		
		public double NextDouble(string key, float min, float max) {
			double i = (NextDouble(key)*(max-min+1))+min;
			if (i >= max) {
				return max;
			}
			return i;
		}
		
		/**
		 * @param code
		 * @return 0 ~ 1
		 */
		public float NextFloat(string key) {
			return (float)NextDouble(key);
		}
		
		/**
		 * @param code
		 * @param min inclusive
		 * @param max inclusive
		 * @return
		 */
		public float NextFloat(string key, float min, float max) {
			return (NextFloat(key)*(max-min))+min;
		}
		
		/**
		 * @param code
		 * @param min inclusive
		 * @param max inclusive
		 * @return
		 */
		public int NextInt(string key, int min, int max) {
			int i = (int)(NextDouble(key)*(max-min+1))+min;
			if (i == max+1) {
				return max;
			}
			return i;
		}
		
		public bool NextBoolean(string key) {
			return NextDouble(key) <= 0.5;
		}
		
		public void Update() {
			foreach (KeyValuePair<string, RandomEntry> entry in rands) {
				/*  EveryTime 은 update()와 관계없이 nextDouble()을 호출할때마다 갱신된다. */
				if (entry.Value.interval != RandomRefreshInterval.EveryTime) {
					entry.Value.Update();
				}
			}
		}
	}
	
	class RandomEntry {
		public PseudoRandom rand;
		public RandomRefreshInterval interval;
		public double val;
		
		public RandomEntry(PseudoRandom rand, RandomRefreshInterval interval) {
			this.rand = rand;
			this.interval = interval;
			Update();
		}
		
		public void Update() {
			this.val = rand.nextDouble();
		}
	}
}

#endif