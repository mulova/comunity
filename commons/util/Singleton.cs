//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

namespace commons {
	public class Singleton<T> where T:new() {
		public static Loggerx log = LogManager.GetLogger(typeof(Singleton<T>));

		private static T singleton;
		
		public static T inst {
			get {
				if (singleton == null)
				{
					lock(typeof(T)) {
						if (singleton == null) {
							singleton = new T();
						}
					}
				}
				return singleton;
			}
		}

		public Singleton() {
			if (singleton != null) {
				LogManager.GetLogger(typeof(T)).Error("Singleton");
			}
		}

		public void Dispose()
		{
			singleton = default(T);
		}
	}
}
