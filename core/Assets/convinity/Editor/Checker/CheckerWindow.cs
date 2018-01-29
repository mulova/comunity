//----------------------------------------------
// Unity3D common libraries and editor tools
// License: http://opensource.org/licenses/GPL-3.0
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using comunity;


namespace convinity {
	public class CheckerWindow : TabbedEditorWindow
	{
		[MenuItem("Tools/unilova/Etc/Checker")]
		static public void CreateCheckerWindow()
		{
			EditorWindow.GetWindow<CheckerWindow>("Checker");
		}
		
		protected override void CreateTabs() {
			AddTab(new BackupTab(this));
			AddTab(new PerformanceTab(this));
			AddTab(new VisibilityCheckTab(this));
		}
	}
}
