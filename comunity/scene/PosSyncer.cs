using UnityEngine;
using System.Collections;
using UnityEngine.Ex;

namespace mulova.comunity
{
	public class PosSyncer : InternalScript
    {
        public Camera srcCam;
        public Transform[] objs;

        [ContextMenu("Install")]
        void Install()
        {
            foreach (Transform t in objs)
            {
                ScreenPosSync s = t.FindComponent<ScreenPosSync>();
                Camera dstCam = CameraEx.GetCamera(t.gameObject.layer);
                if (s.srcObj == null)
                {
                    s.srcObj = srcCam.gameObject.CreateChild(t.name+"_sync").transform;
                }
                // set the src position based on the destination object position
                Vector3 screenPos = dstCam.WorldToScreenPoint(t.position);
                Vector3 srcPos = srcCam.ScreenToWorldPoint(screenPos);
                float distance = Vector3.Distance(dstCam.transform.position, t.position);
                srcPos += srcCam.transform.forward*distance; // depth
                s.srcObj.position = srcPos;
            }
        }
	}
}