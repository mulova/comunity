using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;
using editor.ex;
using System.Text;
using commons;
namespace core
{
	[CustomPropertyDrawer(typeof(EventListAttribute))]
	public class EventListDrawer : PropertyDrawerBase
	{
		protected override int GetLineCount()
		{
			return 1;
		}

		protected override void DrawGUI(GUIContent label)
		{
			EventListAttribute attr = attribute as EventListAttribute;
			string[] events = EventListDrawer.GetIds(attr.listPath);
			string eventId = prop.stringValue;
			if (PopupNullable(GetLineRect(0), prop.name, ref eventId, events)) {
				prop.stringValue = eventId;
			}
		}

		public static string[] GetIds(string assetPath) {
			List<string> lines = new List<string>();
			TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
			if (asset != null) {
                LineParser parser = new LineParser(false);
				lines = parser.Parse(asset.bytes, Encoding.UTF8);
			} else {
				lines = new List<string>();
			}
			return lines.ToArray();
		}
	}
}

