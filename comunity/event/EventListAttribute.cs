using UnityEngine;

namespace comunity
{
	public class EventListAttribute : PropertyAttribute {
		
		public const string EVENT_DESC = "Assets/inspector_settings/event/event_id.bytes";
		
		public string listPath = EVENT_DESC;
		
		public EventListAttribute() { }
		
		public EventListAttribute(string filePath) {
			this.listPath = "Assets/inspector_settings/event/"+filePath;
		}
	}
}
 