//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using Object = UnityEngine.Object;

public interface ObjWrapper {
	Object Obj { get; }
	string Name { get; }
}
