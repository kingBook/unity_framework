using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FuncUtil{
	/// <summary>
	/// 获取DontDestroyOnLoad的所有游戏对象
	/// <br>注意：这个方法很低效</br>
	/// </summary>
	/// <returns></returns>
	public static GameObject[] getDontDestroyOnLoadGameObjects(){
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
	
}
