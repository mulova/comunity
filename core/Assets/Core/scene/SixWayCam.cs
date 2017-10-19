//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013-2014 mulova@gmail.com
//----------------------------------------------
using UnityEngine;

namespace core {
	public class SixWayCam : MonoBehaviour
	{
		public Camera front;
		public Camera back;
		public Camera left;
		public Camera right;
		public Camera up;
		public Camera down;
		
		public void Sync() {
			if (front == null) {
				front = gameObject.GetComponentEx<Camera>();
			}
			front.rect = new Rect(       1/3f,    0, 1/3f, 1/2f);
			CreateCam(ref back,   "back", 2/3f, 1/2f, -transform.forward);
			CreateCam(ref left,   "left",   0,    0, -transform.right);
			CreateCam(ref right, "right", 2/3f,   0, transform.right);
			CreateCam(ref up,       "up", 1/3f, 1/2f, transform.up);
			CreateCam(ref down,   "down",    0, 1/2f, -transform.up);
		}
		
		private Camera CreateCam(ref Camera cam, string name, float left, float top, Vector3 dir) {
			if (cam == null) {
				GameObject obj = new GameObject(name);
				obj.transform.parent = front.transform;
				cam = obj.AddComponent<Camera>();
			}
			Vector3 pos = front.transform.position;
			cam.rect = new Rect(left, top, 1/3f, 1/2f);
			cam.transform.position = pos;
			cam.transform.LookAt(pos+dir);
			
			Component[] comps = front.GetComponents(typeof(MonoBehaviour));
			foreach (Component c0 in comps) {
				if (c0 is SixWayCam) continue;
				MonoBehaviour c = (MonoBehaviour) c0;
				MonoBehaviour c2 = (MonoBehaviour)cam.gameObject.GetComponentEx(c.GetType());
                ComponentUtil.Copy(c, c2, true);
				//			c2.enabled = c.enabled;
			}
			return cam;
			
		}
	}
}

