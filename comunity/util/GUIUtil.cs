//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System;

namespace mulova.comunity {
	public enum RectAlign{
		CENTER,
		LEFT,
		RIGHT,
		UP,
		BOTTOM
	}
	
	public class GUIUtil {
		
		/// <summary>
		/* / 문자열이 비어있는지 검사한다. */
		/// </summary>
		public static bool IsEmpty (string sendMessage)
		{
			return sendMessage==null || sendMessage.Trim().Length==0;
		}
		
		/**
		 * Scale the UI based on the base resolution of UI.
		 */
		public static void Scale(int baseWidth, int baseHeight) {
			float horizRatio = Screen.width / baseWidth;
			float vertRatio = Screen.height / baseHeight;
			
			GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (horizRatio, vertRatio, 1));
		}
		
		public static int Popup(int selection, string[] items, string title, int titleWidth) {
			GUILayout.BeginHorizontal();
			GUILayout.Label(title, GUILayout.Width(titleWidth));
			int sel = GUILayout.SelectionGrid(selection, items, items.Length>4? 4: items.Length);
			GUILayout.EndHorizontal();
			return sel;
		}
		
		public static float Slider(float currentValue, float min, float max, string title, int titleWidth) {
			GUILayout.BeginHorizontal();
			GUILayout.Label(title, GUILayout.Width(titleWidth));
			float sel = GUILayout.HorizontalSlider(currentValue, min, max);
			GUILayout.EndHorizontal();
			return sel;
		}
		
		/// <summary>
		/* / 현 화면의 중간 위치로 Position x,y를 설정한다. */
		/// </summary>
		public static Rect Center(ref Rect rect) {
			rect.x = (Screen.width-rect.width)/2;
			rect.y = (Screen.height-rect.height)/2;
			return rect;
		}
		
		public static Rect StretchToScreen(ref Rect rect) {
			rect.x = 0;
			rect.y = 0;
			rect.width = Screen.width;
			rect.height = Screen.height;
			return rect;
		}
		
		public static Rect CenterBottom(ref Rect rect) {
			rect.x = (Screen.width-rect.width)/2;
			rect.y = (Screen.height-rect.height);
			return rect;
		}
		
		public static Vector2 Resize(Vector3 src, Vector2 maxSize) {
			float width = Math.Min(maxSize.x, src.x);
			float height = Math.Min(maxSize.y, src.y);
			float ratio = Math.Min(width/src.x, height/src.y);
			return new Vector2(src.x*ratio, src.y*ratio);
		}
	}
}


