using System;
using UnityEngine;

namespace mulova.comunity
{
    [ExecuteInEditMode]
    public class SyncTrans : InternalScript
    {
        public Transform syncTarget;
        public bool useDelta = true;
        public bool syncX = true;
        public bool syncY = true;
        public bool syncZ = true;

        public Camera _srcCam;
        public Camera srcCam
        {
            get
            {
                if (_srcCam == null)
                {
                    _srcCam = CameraEx.GetCamera(go.layer);
                }
                return _srcCam;
            }
        }
        public Camera _dstCam;
        public Camera dstCam
        {
            get
            {
                if (_dstCam == null)
                {
                    _dstCam = CameraEx.GetCamera(syncTarget.gameObject.layer);
                }
                return _dstCam;
            }
        }
        private Vector3 posDelta;

        void OnEnable()
        {
            SetSyncTarget(syncTarget);
        }

        public void SetSyncTarget(Transform target)
        {
            this.syncTarget = target;
            if (syncTarget != null && useDelta)
            {
                posDelta = transform.position-syncTarget.position;
            }
        }

        void Update()
        {
            if (syncTarget != null)
            {
                if (syncX && syncY && syncZ)
                {
                    transform.position = syncTarget.position+posDelta;
                } else
                {
                    float x = syncX? syncTarget.position.x+posDelta.x : transform.position.x;
                    float y = syncY? syncTarget.position.y+posDelta.y : transform.position.y;
                    float z = syncZ? syncTarget.position.z+posDelta.z : transform.position.z;
                    transform.position = new Vector3(x, y, z);
                }
            }
        }
    }
}

