using System;
using UnityEngine.Ex;

namespace UnityEngine.Ex
{
	[Serializable]
    public struct TransformData
    {
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public bool local;

        public TransformData(Transform t, bool local = true)
        {
            this.local = local;
            this.pos = Vector3.zero;
            this.rot = Quaternion.identity;
            this.scale = Vector3.one;
            CopyFrom(t);
        }

        public void CopyFrom(Transform t)
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
        }

        public void CopyTo(Transform t)
        {
            if (local)
            {
                t.localPosition = this.pos;
                t.localRotation = this.rot;
                t.localScale = this.scale;
            } else
            {
                t.position = this.pos;
                t.rotation = this.rot;
                throw new NotImplementedException();
//                this.scale = t.lossyScale;
            }
        }

        public bool IsSame(Transform t)
        {
            if (local)
            {
                return t.localPosition.ApproximatelyEquals(pos)
                        && t.localRotation.ApproximatelyEquals(rot)
                        && t.localScale.ApproximatelyEquals(scale);
            } else
            {
                return t.position.ApproximatelyEquals(pos)
                        && t.rotation.ApproximatelyEquals(rot)
                        && t.lossyScale.ApproximatelyEquals(scale);
            }
        }
    }
}

