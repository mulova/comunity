using System.Collections.Generic;
using System.IO;


using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

using System;
using comunity;

namespace convinity {
	public class ShortcutWindow : TabbedEditorWindow {
		private static int count = 0;
        private static bool showAllTabs;
		protected override void CreateTabs() {
            AddTab(new SceneHistoryTab("History", this), new ShortcutTab("Local", "Library/Shortcut", this), new ShortcutTab("Shared", "Shortcut", this));
//			ShowAllTab(true);
		}

		[MenuItem("Tools/unilova/Etc/Shortcuts")]
		public static void Init() {
			// Get existing open window or if none, make a new one:
			ShortcutWindow window = ScriptableObject.CreateInstance<ShortcutWindow>();
			window.Show();
			count++;
			window.titleContent = new GUIContent("Shortcut"+count);
		}

        protected override void ShowContextMenu()
        {
            // Now create the menu, add items and show it
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Toggle Show All Tabs"), false, ToggleShowAllTabs);
            menu.ShowAsContext();
        }

        private void ToggleShowAllTabs()
        {
            showAllTabs = !showAllTabs;
            ShowAllTab(showAllTabs);
        }
	}
}
