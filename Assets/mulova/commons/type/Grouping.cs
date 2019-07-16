#if FULL
//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace commons {
	/**
	 * 분류가 배타적이지 않은 경우, 여러 분류의 값을 동시에 가질 수 있게 한다.<br>
	 * 상속받는 class는 public static T[] values() method를 제공해야 한다.
	 * 예) 
	 * public static T[] values() {
	 *		return values(T.class);
	 * }
	 * @author mulova
	 */
	public class Grouping<T> : ICloneable {
		protected static DualKeyHashMap<Type, string, Grouping<T>> pool = new DualKeyHashMap<Type, string, Grouping<T>>();
		protected string name;
		protected int ordinal;
		protected uint groups;
		protected uint filter	= 0xFFFFFFFF;
		protected readonly bool modifiable;
		private readonly int groupId;
		
		/**
		 * For serialization's purpose only
		 */
		public Grouping(): this("") {
		}
		
		/**
		 * 수정 가능한 group.
		 */
		public Grouping(string name) {
			this.name = name;
			this.modifiable = true;
			this.ordinal = 0;
			this.groupId = 0;
		}
		
		
		public Grouping(string name, params Grouping<T>[] group) {
			this.name = name;
			this.modifiable = false;
			this.groupId = -1;
			if (group != null && group.Length > 0) {
				foreach ( Grouping<T> g in group) {
					this.groups |= g.groups;
					this.filter &= g.filter;
				}
			}
			Pooling();
		}
		
		
		protected Grouping(string name,  params uint[] group) {
			this.name = name;
			this.modifiable = false;
			this.groupId = -1;
			foreach (uint i in group) {
				this.groups |= i;
			}
			Pooling();
		}
		
		
		/**
		 * Filter가 지정되지 않는다. 즉
		 * 
		 * @param radix2
		 *            radix 2 number
		 */
		protected Grouping(string name0,  string radix2) {
			this.name = name0;
			this.modifiable = false;
			this.groupId = -1;
			this.groups = ParseByRadix(radix2, 2);
			Pooling();
		}
		
		/**
		 * @param name0
		 * @param groupRadix2
		 *            radix 2 number
		 * @param filterRadix2
		 * 			radix 2 number. bit가 0 이면 지워진다.
		 */
		protected Grouping(string name0,  string groupRadix2,  string filterRadix2) {
			this.name = name0;
			this.modifiable = false;
			this.groups = ParseByRadix(groupRadix2, 2);
			this.filter = ParseByRadix(filterRadix2, 2);
			this.groupId = -1;
			Pooling();
		}
		
		protected Grouping(string name,  int groupId) {
			this.groupId = groupId;
			this.name = name;
			this.modifiable = false;
			Pooling();
			this.groups = (uint) Mathf.Pow(2, ordinal);
			this.filter = 0xFFFFFFFF;
			
			Dictionary<string, Grouping<T>> g = pool[GetType()];
			foreach (Grouping<T> entry in g.Values) {
				if (entry.groupId == groupId) {
					filter &= ~entry.groups;
				}
			}
			foreach (Grouping<T> entry in g.Values) {
				if (entry.groupId == groupId) {
					entry.filter = filter;
				}
			}
		}
		
		private void Pooling() {
			if (pool.Get(GetType(), name) == null){
				pool.Put(GetType(), name, this);
				ordinal = pool[GetType()].Count-1;
			}
		}
		
		/**
		 * 현재 group이 super group에 포함되는지 여부를 판단한다.<br>
		 * @param superGroup 
		 * @return superGroup이 현재 group의 충분조건일때(group을 포함할때) true.
		 */
		public bool IsMemberOf( Grouping<T> superGroup) {
			if (superGroup.groups == 0)
				return this.groups == 0;
			return (this.groups & superGroup.groups) == this.groups;
		}
		
		/**
		 * 현재 group이 sub group을 포함하는지 판단한다.<br>
		 * @param subGroup 
		 * @return 이 group이 subGroup의 충분조건일 경우(subGroup을 모두 가지고 있을 경우) true.
		 */
		public bool Has( Grouping<T> subGroup) {
			if (subGroup.groups == 0)
				return this.groups == 0;
			return (this.groups | subGroup.groups) == this.groups;
		}
		
		/**
		 * @param group 현재 group이 동일한 그룹일 경우. 즉 필요 충분조건.
		 * @return
		 */
		public bool Is( Grouping<T> group) {
			return this.groups == group.groups;
		}
		
		
		/**
		 * Group과 merge한다.
		 * @param group
		 * @author mulova
		 */
		public void Set(params Grouping<T>[] group) {
			if (!this.modifiable)
				throw new Exception("Not modifiable");
			foreach (Grouping<T> g in group) {
				Set0(g);
			}
		}
		
		/**
		 * 수정 가능 여부를 판단하지 않는다.
		 * @param group merge할 group
		 * @author mulova
		 */
		protected void Set0( Grouping<T> group) {
			this.groups &= group.filter;
			this.groups |= group.groups;
		}
		
		
		/**
		 * clear only specified group
		 * @param group
		 * @author mulova
		 */
		public void Reset(params Grouping<T>[] group) {
			if (!this.modifiable)
				throw new Exception("Not modifiable");
			foreach (Grouping<T> g in group) {
				this.groups &= ~g.groups;
			}
		}
		
		public void Copy(Grouping<T> orig) {
			groups = orig.groups;
			filter = orig.filter;
		}
		
		public bool Intersects(Grouping<T> group) {
			return (this.groups & group.groups) != 0x0;
		}
		
		public uint Groups {
			get { return this.groups; }
		}
		
		
		public uint Filter {
			get { return this.filter; }
		}
		
		public string Name {
			get { return this.name; }
		}
		
		public static ICollection<Grouping<T>> values() {
			return pool.Values(typeof(T));
		}
		
		public string GetGroupNames() {
			return GetGroupNames(pool.Values(GetType()), this);
		}
		/**
		 * 
		 * @param groups
		 * @param g
		 * @return
		 * @author mulova
		 */
		public static string GetGroupNames(ICollection<Grouping<T>> groups, Grouping<T> g) {
			if (groups == null)
				return null;
			StringBuilder buf = new StringBuilder();
			foreach (Grouping<T> group in groups) {
				if (g.Has(group)) {
					buf.Append(group.name).Append(" ");
				}
			}
			return buf.ToString();
		}
		
		public override string ToString() {
			return this.name;
		}
		
		public override bool Equals(object obj) {
			if (obj == null)
				return false;
			if (obj.GetType() != GetType())
				return false;
			Grouping<T> that = (Grouping<T>)obj;
			return this.groups == that.groups
				&& this.filter == that.filter
					&& this.ordinal == that.ordinal;
		}
		
		public override int GetHashCode() {
			return ordinal;
		}
		
		/**
		 * 모든 unmodifiable group을 singleton으로 사용하기 위해 저장하고 있다.
		 * @param clazz
		 * @param name
		 * @return
		 */
		public static Grouping<T> valueOf(string name) {
			return pool.Get(typeof(T), name);
		}
		
		public Grouping<T> Clone() {
			Grouping<T> g = new Grouping<T>();
			g.name = this.name;
			g.ordinal = this.ordinal;
			g.groups = this.groups;
			g.filter = this.filter;
			return g;
		}
		
		object ICloneable.Clone() {
			return this.Clone();
		}
		
		/**
		 * 2진수, 16진수 등의 숫자를 공백을 제거하고 parsing한다.
		 * @param number
		 */
		public static uint ParseByRadix(string number, int radix) {
			StringBuilder buf = new StringBuilder();
			for (int i = 0; i < number.Length; i++) {
				char c = number[i];
				if (c != ' ')
					buf.Append(c);
			}
			string str = buf.ToString();
			return Convert.ToUInt32(str, radix);
		}
	}
}
#endif