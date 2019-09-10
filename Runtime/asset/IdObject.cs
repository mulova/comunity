using System;
using Object = UnityEngine.Object;

namespace mulova.comunity
{
	public class IdObject
	{
		public readonly string id;
		public readonly Object obj;

		public IdObject(string id, Object obj)
		{
			this.id = id;
			this.obj = obj;
		}
	}
}
