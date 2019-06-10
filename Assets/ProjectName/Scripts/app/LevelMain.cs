﻿using framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMain : BaseBehaviour
{
	protected override void Start() {
		base.Start();
		var objs=getDontDestroyOnLoadGameObjects();
		//Debug2.Log(objs[0].name,objs.Length);

		Invoke("loadMainScene",1.5f);
	}

	private void loadMainScene(){
		SceneManager.LoadScene("main");
	}

	protected override void OnDestroy() {
		base.OnDestroy();
	}




}
