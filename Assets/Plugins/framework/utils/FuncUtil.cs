using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 函数工具类
/// </summary>
public class FuncUtil{
	
	/// <summary>
	/// 返回包围盒的角点列表
	/// </summary>
	/// <param name="boundsCenter">包围盒的中心</param>
	/// <param name="boundsExtents">Bounds.extents</param>
	/// <returns></returns>
	public static Vector3[] GetBoundsCorners(Vector3 boundsCenter,Vector3 boundsExtents){
		Vector3[] vertices=new Vector3[8];
		//左下后
		vertices[0]=boundsCenter+Vector3.Scale(boundsExtents,new Vector3(-1,-1,-1));
		//左上后
		vertices[1]=boundsCenter+Vector3.Scale(boundsExtents,new Vector3(-1,1,-1));
		//右上后
		vertices[2]=boundsCenter+Vector3.Scale(boundsExtents,new Vector3(1,1,-1));
		//右下后
		vertices[3]=boundsCenter+Vector3.Scale(boundsExtents,new Vector3(1,-1,-1));

		//左下前
		vertices[4]=boundsCenter+Vector3.Scale(boundsExtents,new Vector3(-1,-1,1));
		//左上前
		vertices[5]=boundsCenter+Vector3.Scale(boundsExtents,new Vector3(-1,1,1));
		//右上前
		vertices[6]=boundsCenter+Vector3.Scale(boundsExtents,new Vector3(1,1,1));
		//右下前
		vertices[7]=boundsCenter+Vector3.Scale(boundsExtents,new Vector3(1,-1,1));
		return vertices;
	}
    
    /// <summary>
    /// 将世界坐标点数组投射到以原点为中心的平面
    /// </summary>
    /// <param name="points">世界坐标点数组</param>
    /// <param name="pointCount">坐标点数量</param>
    /// <param name="planeNormal">平面法线</param>
    /// <returns></returns>
    public static Vector3[] WorldPointsToPlane(Vector3[] points,int pointCount,Vector3 planeNormal){
        for(int i=0;i<pointCount;i++){
            var vertex=points[i];
            points[i]=Vector3.ProjectOnPlane(vertex,planeNormal);
        }
        return points;
    }
    
    /// <summary>
    /// 将世界坐标点数组转换为屏幕坐标
    /// </summary>
    /// <param name="points">世界坐标点数组</param>
    /// <param name="pointCount">坐标点数量</param>
    /// <param name="camera">用于转换的相机</param>
    /// <returns></returns>
	public static Vector3[] WorldPointsToScreen(Vector3[] points,int pointCount,Camera camera){
		for(int i=0;i<pointCount;i++){
            var vertex=points[i];
            points[i]=camera.WorldToScreenPoint(vertex);
        }
		return points;
	}



	/// <summary>
	/// 获取DontDestroyOnLoad的所有游戏对象
	/// <br>注意：这个方法很低效</br>
	/// </summary>
	/// <returns></returns>
	public static GameObject[] GetDontDestroyOnLoadGameObjects(){
		var allGameObjects=new List<GameObject>();
		allGameObjects.AddRange(Object.FindObjectsOfType<GameObject>());
		//移除所有场景包含的对象
		for(var i=0;i<SceneManager.sceneCount;i++){
			var scene=SceneManager.GetSceneAt(i);
			var objs=scene.GetRootGameObjects();
			for(var j=0;j<objs.Length;j++){
				allGameObjects.Remove(objs[j]);
			}
		}
		//移除父级不为null的对象
		int k=allGameObjects.Count;
		while(--k>=0){
			if(allGameObjects[k].transform.parent!=null){
				allGameObjects.RemoveAt(k);
			}
		}
		return allGameObjects.ToArray();
	}

	/// <summary>
	/// 获取秒转换为xx:xx的时钟形式字符
	/// </summary>
	/// <param name="secondCount">秒数</param>
	/// <param name="isHour">如果true那么转换为xx:xx:xx形式否则xx:xx形式</param>
	/// <returns></returns>
	public static string GetClockString(int secondCount,bool isHour=false) {
		string result="";
		if(isHour){
			int hour=(int)(secondCount/60.0f/60.0f);
			string hourString=hour<10?"0"+hour.ToString():hour.ToString();

			int minute=(int)(secondCount/60.0f-hour*60.0f);
			string minuteString=minute<10?"0"+minute.ToString():minute.ToString();

			int second=(int)(secondCount-hour*60.0f*60.0f-minute*60.0f);
			string secondString=second<10?"0"+second.ToString():second.ToString();

			result=hourString+":"+minuteString+":"+secondString;
		}else{
			int minute=(int)(secondCount/60.0f);
			string minuteString=minute<10?"0"+minute.ToString():minute.ToString();

			int second=(int)(secondCount-minute*60.0f);
			string secondString=second<10?"0"+second.ToString():second.ToString();

			result=minuteString+":"+secondString;
		}
		return result; 
	}
	
	/// <summary>
	/// 返回一个游戏对象的包围盒(将跳过计算未激活的Renderer)
	/// </summary>
	/// <param name="gameObject">游戏对象</param>
	/// <param name="filterChildren">需要过滤的子级对象</param>
	/// <returns></returns>
	public static Bounds GetGameObjectBounds(GameObject gameObject,params GameObject[]filterChildren){
		int filterChildCount=filterChildren.Length;
		Renderer[] filterChildRenderers=new Renderer[filterChildCount];
		for(int i=0;i<filterChildCount;i++){
			filterChildRenderers[i]=filterChildren[i].GetComponent<Renderer>();
		}
		
		Bounds bounds=new Bounds();
		Renderer rootRenderer=gameObject.GetComponent<Renderer>();
		if(rootRenderer!=null&&rootRenderer.enabled){
			bounds=rootRenderer.bounds;
		}
		Renderer[] subRenderers=gameObject.GetComponentsInChildren<Renderer>();
		int j=subRenderers.Length;
		while(--j>=0){
			Renderer renderer=subRenderers[j];
			if(!renderer.enabled)continue;
			if(System.Array.IndexOf(filterChildRenderers,renderer)>-1)continue;
			if(bounds.min.magnitude==0f && bounds.max.magnitude==0f){
				bounds=renderer.bounds;
			}else{
				bounds.Encapsulate(renderer.bounds);
			}
		}
		return bounds;
	}
	
}
