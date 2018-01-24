//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Text;

namespace commons {
	
	public class AndPredicate<T> : IPredicate<T> {
		private Predicate<T>[] predicates;
		private string name;
		public string Name {
			get {
				if (predicates.IsEmpty()) {
					return string.Empty;
				}
				if (name == null) {
					StringBuilder str = new StringBuilder();
					str.Append("(").Append(predicates[0].ToString());
					for (int i=1; i<predicates.Length; i++) {
						str.Append(" && ").Append(predicates[i].ToString());
					}
					str.Append(")");
					name = str.ToString();
				}
				return name;
			}
		}
		
		public AndPredicate(params Predicate<T>[] p) {
			this.predicates = p;
		}
		
		public void AddPredicate(Predicate<T> p) {
			ArrayUtil.Add(ref predicates, p);
			name = null;
		}
		
		public bool Accept(T t) {
			foreach (Predicate<T> p in predicates) {
				if (!p(t)) {
					return false;
				}
			}
			return true;
		}
	}
	
	public class OrPredicate<T> : IPredicate<T> {
		private Predicate<T>[] predicates;
		private string name;
		public string Name {
			get {
				if (predicates.IsEmpty()) {
					return string.Empty;
				}
				if (name == null) {
					StringBuilder str = new StringBuilder();
					str.Append("(").Append(predicates[0].ToString());
					for (int i=1; i<predicates.Length; i++) {
						str.Append(" || ").Append(predicates[i].ToString());
					}
					str.Append(")");
					name = str.ToString();
				}
				return name;
			}
		}
		
		public OrPredicate(params Predicate<T>[] p) {
			this.predicates = p;
		}
		
		public void AddPredicate(Predicate<T> p) {
			ArrayUtil.Add(ref predicates, p);
			name = null;
		}
		
		public bool Accept(T t) {
			foreach (Predicate<T> p in predicates) {
				if (p(t)) {
					return true;
				}
			}
			return false;
		}
	}
}


