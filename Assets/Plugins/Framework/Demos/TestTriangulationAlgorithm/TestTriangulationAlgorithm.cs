using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriangulationAlgorithm : MonoBehaviour{
	
	public Transform pointsParent;

	private void Start() {
		
		
		Vector3[] vertices=new Vector3[pointsParent.childCount];
		List<int> indices=new List<int>();
		for(int i=0;i<vertices.Length;i++){
			vertices[i]=pointsParent.GetChild(i).position;
			indices.Add(i);
		}

		Plane plane=new Plane(Vector3.back,0);
		
		ConvexHullAlgorithm.Execute(ref indices,vertices,plane);

		indices.Reverse();
		//TriangulationAlgorithm.WidelyTriangleIndex(vertices,ref indices,plane);

		List<Vector3> vertices2=new List<Vector3>();
		List<int> indices2=new List<int>();
		for(int i=0;i<indices.Count;i++){
			Vector3 vertex=vertices[indices[i]];
			vertex.z=0;
			vertices2.Add(vertex);
			indices2.Add(i);
		}
		indices=TriangulationAlgorithm.WidelyTriangleIndex(vertices2,indices2);
		
		for(int i=0;i<indices.Count;i++){
			int index=indices[i];
			s_points.Add(vertices2[index]);
		}
	}

	private static List<Vector3> s_points=new List<Vector3>();
	private void OnDrawGizmos(){
		for(int i=0;i<s_points.Count;i+=3){
			Gizmos.DrawLine(s_points[i],s_points[i+1]);
			Gizmos.DrawLine(s_points[i+1],s_points[i+2]);
			Gizmos.DrawLine(s_points[i+2],s_points[i]);
		}
	
		/*for(int i=0;i<s_points.Count;i++){
			GizmosUtil.DrawPoint(s_points[i]);
			UnityEditor.Handles.Label(s_points[i],string.Format("{0}",i));
			if(i>0){
				Gizmos.DrawLine(s_points[i-1],s_points[i]);
				
			}
		}*/

	}
}
