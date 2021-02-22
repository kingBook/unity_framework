﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 凸包算法
/// </summary>
public class ConvexHullAlgorithm{
	
	private struct VertexData{
		public int index;
		public Vector3 vertex;
	}

	public static void Execute(ref List<int> indices,Vector3[] vertices,Plane plane){
		Quaternion rotation=Quaternion.FromToRotation(plane.normal,Vector3.back);
		
		int indexCount=indices.Count;
		List<int> result=new List<int>();

		List<VertexData> vertexDatas=new List<VertexData>(indexCount);

		for(int i=0;i<indexCount;i++){
			int index=indices[i];
			Vector3 vertex=vertices[index];
			//旋转至切割平面坐标系（从平面上方看向平面）
			vertex=rotation*vertex;

			VertexData vertexData=new VertexData();
			vertexData.index=index;
			vertexData.vertex=vertex;
			vertexDatas.Add(vertexData);
		}

		//找到y值最小的点作为p0，如果有多个则选择最左边的那个
		float minY=float.MaxValue;
		int p0Index=-1;
		for(int i=0;i<indexCount;i++){
			VertexData vertexData=vertexDatas[i];
			if(vertexData.vertex.y<minY){
				minY=vertexData.vertex.y;
				p0Index=i;
			}else if(vertexData.vertex.y==minY){
				if(p0Index>-1){
					if(vertexData.vertex.x<vertexDatas[p0Index].vertex.x){
						p0Index=i;
					}
				}
			}
		}

		VertexData p0=vertexDatas[p0Index];

		List<VertexData> resultVertexDatas=new List<VertexData>();
		resultVertexDatas.Add(p0);
		vertexDatas.RemoveAt(p0Index);//移除p0


		//计算测试点相对于p0的幅角，并按小到大排序
		vertexDatas.Sort((VertexData a,VertexData b)=>{ 
			Vector3 oa=a.vertex-p0.vertex;
			Vector3 ob=b.vertex-p0.vertex;
			float angleA=Mathf.Atan2(oa.y,oa.x);
			float angleB=Mathf.Atan2(ob.y,ob.x);
			if(angleA==angleB){
				return (int)Mathf.Sign(oa.magnitude-ob.magnitude);
			}
			return (int)Mathf.Sign(angleA-angleB);
		});


		resultVertexDatas.Add(vertexDatas[0]);
		resultVertexDatas.Add(vertexDatas[1]);
		//此时 resultVertexDatas 有 p0,p1,p2；而 vertexDatas 移除了p0，vertexDatas[0]为 p1

		for(int i=2,len=vertexDatas.Count;i<len;i++){
			VertexData baseVertex=resultVertexDatas[resultVertexDatas.Count-2];//从p1开始

			Vector3 v1=vertexDatas[i-1].vertex-baseVertex.vertex;
			Vector3 v2=vertexDatas[i].vertex-baseVertex.vertex;
			v1.z=v2.z=0;

			if(Vector3.Cross(v1,v2).z<0){
				resultVertexDatas.RemoveAt(resultVertexDatas.Count-1);
				while(true){
					VertexData baseVertex2=resultVertexDatas[resultVertexDatas.Count-2];
					Vector3 v12=resultVertexDatas[resultVertexDatas.Count-1].vertex-baseVertex2.vertex;
					Vector3 v22=vertexDatas[i].vertex-baseVertex2.vertex;
					v12.z=v22.z=0;
					if(Vector3.Cross(v12,v22).z<0){
						resultVertexDatas.RemoveAt(resultVertexDatas.Count-1);
					}else{
						break;
					}
				}
				resultVertexDatas.Add(vertexDatas[i]);
			}else{
				resultVertexDatas.Add(vertexDatas[i]);
			}
		}

		indices.Clear();
		for(int i=0,len=resultVertexDatas.Count;i<len;i++){
			indices.Add(resultVertexDatas[i].index);
		}
		
	}
}
