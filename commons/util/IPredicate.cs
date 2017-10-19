//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

namespace commons {
	public interface IPredicate<T> {
		string Name { get; }
		bool Accept(T t);
	}
	
	public abstract class AbstractPredicate<T> : IPredicate<T> {
		private readonly string name;
		
		public abstract bool Accept(T t);
		
		public AbstractPredicate() {
			this.name = GetType().Name;
		}
		
		public AbstractPredicate(string name) {
			this.name = name;
		}
		
		public string Name { get { return name; } }
		public override string ToString() { return name; }
	}
}

