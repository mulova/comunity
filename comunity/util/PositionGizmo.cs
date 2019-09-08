//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace mulova.comunity
{
    /**
    * 하위 노드들의 위치를 Gizmo로 표시한다.
    */
    public class PositionGizmo : MonoBehaviour {
        
        public enum GizmoType {
            Cube, Sphere, WireCube, WireSphere
        }
        public GizmoType gizmoType = GizmoType.WireSphere;
        public Vector3 size = new Vector3(1, 1, 1);
        
        void Start() {
            if (Platform.isBuild) {
                Destroy(this);
            }
        }
        
        void OnDrawGizmos() {
            if (enabled) {
                Vector3 pos = transform.position;
                switch (gizmoType) {
                    case GizmoType.Cube:
                        Gizmos.DrawCube(pos, size);
                        break;
                    case GizmoType.Sphere:
                        Gizmos.DrawSphere(pos, size.x);
                        break;
                    case GizmoType.WireCube:
                        Gizmos.DrawWireCube(pos, size);
                        break;
                    case GizmoType.WireSphere:
                        Gizmos.DrawWireSphere(pos, size.x);
                        break;
                }
            }
        }
    }
}
