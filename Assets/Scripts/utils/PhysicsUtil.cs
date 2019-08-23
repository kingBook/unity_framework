using UnityEngine;
using System.Collections;

public class PhysicsUtil{
	/// <summary>
	/// 返回离射线原点最近的RaycastHit,如果没有找到将返回new RaycastHit()
	/// </summary>
	/// <param name="ray">射线</param>
	/// <param name="layerMask">用于射线计算的LayerMask，如：LayerMask.GetMask("ItemModel")。</param>
	/// <returns></returns>
	public static RaycastHit getClosestRaycastHit(Ray ray,int layerMask){
		RaycastHit result=new RaycastHit();
		RaycastHit[] hits=Physics.RaycastAll(ray,Mathf.Infinity,layerMask);
		float minDistance=float.MaxValue;
		int len=hits.Length;
		for(int i=0;i<len;i++){
			RaycastHit hit=hits[i];
			if(hit.distance<minDistance){
				minDistance=hit.distance;
				result=hit;
			}
		}
		return result;
	}
	
}
