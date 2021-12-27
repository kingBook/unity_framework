using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriangulationAlgorithm : MonoBehaviour {

    public Transform pointsParent;

    private void Start () {


        Vector3[] vertices = new Vector3[pointsParent.childCount];
        List<int> indices = new List<int>();
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] = pointsParent.GetChild(i).position;
            indices.Add(i);
        }

        Plane plane = new Plane(Vector3.back, 0);

        ConvexHullAlgorithm.Execute(ref indices, vertices, plane.normal);

        indices.Reverse();//反转列表，逆时针变顺时针

        /*Vector3[] vertices2=new Vector3[indices.Count];
        for(int i=0;i<indices.Count;i++){
            Vector3 vertex=vertices[indices[i]];
            vertex.z=0;
            vertices2[i]=vertex;
        }
        indices=TriangulationAlgorithm.WidelyTriangleIndex(vertices2);*/

        TriangulationAlgorithm.WidelyTriangleIndex(vertices, ref indices, plane);

        for (int i = 0; i < indices.Count; i++) {
            int index = indices[i];
            s_points.Add(vertices[index]);
        }



    }

    private static List<Vector3> s_points = new List<Vector3>();
    private void OnDrawGizmos () {
        //画三角形
        for (int i = 0; i < s_points.Count; i += 3) {
            Gizmos.DrawLine(s_points[i], s_points[i + 1]);
            Gizmos.DrawLine(s_points[i + 1], s_points[i + 2]);
            Gizmos.DrawLine(s_points[i + 2], s_points[i]);

            //查看三角顶点顺序
            /*UnityEditor.Handles.Label(s_points[i],"A");
            UnityEditor.Handles.Label(s_points[i+1],"B");
            UnityEditor.Handles.Label(s_points[i+2],"C");
            break;*/
        }

        /*
         //画线
         for(int i=0;i<s_points.Count;i++){
            GizmosUtil.DrawPoint(s_points[i]);
            UnityEditor.Handles.Label(s_points[i],string.Format("{0}",i));
            if(i>0){
                Gizmos.DrawLine(s_points[i-1],s_points[i]);
                
            }
        }*/

    }
}
