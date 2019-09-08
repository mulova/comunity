//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using Object = UnityEngine.Object;

namespace mulova.comunity
{
	public interface NamedObj {
		Object Obj { get; }
		string Name { get; }
	}
}

