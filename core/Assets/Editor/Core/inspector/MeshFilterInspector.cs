//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using commons;



[CustomEditor(typeof(MeshFilter))]
public class MeshFilterInspector : Editor
{
    private MeshFilter mesh;

    void OnEnable()
    {
        mesh = target as MeshFilter;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (mesh.sharedMesh == null)
        {
            return;
        }
        EditorGUILayout.LabelField("Vertex Count: "+mesh.sharedMesh.vertexCount);
        Transform t = mesh.transform;
        Transform parent = t.parent;
        if (t.position != Vector3.zero||t.localScale != Vector3.one||t.localRotation != Quaternion.identity)
        {
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Translate", GUILayout.ExpandWidth(false)))
            {
                Vector3[] vertices = mesh.sharedMesh.vertices;
                for (int i = 0; i < vertices.Length; ++i)
                {
                    vertices[i] = t.TransformPoint(vertices[i]);
                    if (parent != null)
                    {
                        vertices[i] = parent.InverseTransformPoint(vertices[i]);
                    }
                }
                mesh.sharedMesh.vertices = vertices;
                mesh.sharedMesh.vertices = vertices;
                mesh.sharedMesh.RecalculateBounds();
                mesh.sharedMesh.RecalculateNormals();
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                AssetDatabase.CreateAsset(mesh.sharedMesh, PathUtil.AddFileSuffix(AssetDatabase.GetAssetPath(mesh.sharedMesh), "_clone"));
                CompatibilityEditor.SetDirty(mesh);
                CompatibilityEditor.SetDirty(mesh.sharedMesh);
            }
            if (GUILayout.Button("Clone", GUILayout.ExpandWidth(false)))
            {
                Mesh m = new Mesh();
                m.vertices = mesh.sharedMesh.vertices;
                m.triangles = mesh.sharedMesh.triangles;
                m.bindposes = mesh.sharedMesh.bindposes;
                m.boneWeights = mesh.sharedMesh.boneWeights;
                m.bounds = mesh.sharedMesh.bounds;
                m.colors32 = mesh.sharedMesh.colors32;
                m.normals = mesh.sharedMesh.normals;
                m.tangents = mesh.sharedMesh.tangents;
                for (int i=0; i<m.subMeshCount; ++i)
                {
                    m.SetIndices(m.GetIndices(i), m.GetTopology(i), i);
                }
                m.uv = mesh.sharedMesh.uv;
                m.uv2 = mesh.sharedMesh.uv2;
                m.uv3 = mesh.sharedMesh.uv3;
                m.uv4 = mesh.sharedMesh.uv4;
                mesh.sharedMesh = m;
                m.RecalculateBounds();
                m.RecalculateNormals();
                CompatibilityEditor.SetDirty(mesh);
            }
            GUILayout.EndHorizontal();
        }
    }
}
