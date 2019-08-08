﻿using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Text;
using mulova.commons;
namespace comunity
{
	[CustomPropertyDrawer(typeof(EventListAttribute))]
	public class EventListDrawer : PropertyDrawerBase
	{
		protected override int GetLineCount(SerializedProperty p)
		{
			return 1;
		}

		protected override void DrawGUI(SerializedProperty p)
        {
			EventListAttribute attr = attribute as EventListAttribute;
			string[] events = EventListDrawer.GetIds(attr.listPath);
			string eventId = p.stringValue;
			if (PopupNullable(GetLineRect(0), p.name, ref eventId, events)) {
				p.stringValue = eventId;
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
