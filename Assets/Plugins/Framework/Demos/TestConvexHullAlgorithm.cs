using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConvexHullAlgorithm : MonoBehaviour {
    public Transform pointsParent;

    private void Start () {


        Vector3[] vertices=new Vector3[pointsParent.childCount];
        List<int> indices=new List<int>();
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = pointsParent.GetChild(i).position;
            indices.Add(i);
        }

        Plane plane=new Plane(Vector3.back,0);

        ConvexHullAlgorithm.Execute(ref indices, vertices, plane.normal);

        for (int i = 0; i < indices.Count; i++) {
            s_points.Add(vertices[indices[i]]);
        }
    }

    private static List<Vector3> s_points=new List<Vector3>();
    private void OnDrawGizmos () {
#if UNITY_EDITOR
        for (int i = 0; i < s_points.Count; i++) {
            GizmosUtil.DrawPoint(s_points[i]);
            UnityEditor.Handles.Label(s_points[i], string.Format("{0}", i));
            if (i > 0) {
                Gizmos.DrawLine(s_points[i - 1], s_points[i]);

            }
        }
#endif
    }
}
