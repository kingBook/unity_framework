using framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test:MonoBehaviour {

	private void Start() {
		//SpriteRenderer renderer=createObjAddComponent<SpriteRenderer>("testObj");
		//Debug.Log(renderer);//output: testObj (UnityEngine.SpriteRenderer)
		Invoke("loadLevelScene",1.5f);
		
	}

	private T createObjAddComponent<T>(string name)where T:Component{
		GameObject obj=new GameObject(name);
		T component=obj.AddComponent<T>();
		return component;
	}


	private void loadLevelScene(){
		SceneManager.LoadScene("level");
		
		

	}

}