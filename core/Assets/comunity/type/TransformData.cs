using System;
using UnityEngine;

namespace comunity
{
    public struct TransformData
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public bool local;

        public TransformData(Transform t, bool local)
        {
            if (local)
            {
                this.pos = t.localPosition;
                this.rot = t.localRotation;
                this.scale = t.localScale;
            } else
            {
                this.pos = t.position;
                this.rot = t.rotation;
                this.scale = t.lossyScale;
            }
            this.local = local;
        }

        public void Apply(Transform t)
        {
            t.localPosition = this.pos;
            t.localRotation = this.rot;
            t.localScale = this.scale;
        }

        public bool IsSame(Transform t)
        {
            if (local)
            {
                return Vector3Ex.ApproximatelyEquals(t.localPosition, pos)
                        && QuaternionEx.ApproximatelyEquals(t.localRotation, rot)
                        && Vector3Ex.ApproximatelyEquals(t.localScale, scale);
            } else
            {
                return Vector3Ex.ApproximatelyEquals(t.position, pos)
                        && QuaternionEx.ApproximatelyEquals(t.rotation, rot)
                        && Vector3Ex.ApproximatelyEquals(t.lossyScale, scale);
            }
        }
    }
}

