//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

namespace commons {
	public interface EventListener<E>
	{
		void OnEvent(E evt, object data);
	}
}