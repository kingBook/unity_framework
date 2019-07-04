﻿
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 整个应用程序的单例类
/// <br>此类的以下方法：FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject，</br>
/// <br>将使用以下代替：FixedUpdate2、Update2、LateUpdate2、OnGUI2、OnRenderObject2。</br>
/// </summary>
public class App:BaseApp<App>{
	
	private Game _game;

	
	protected override void Awake(){
		base.Awake();
		_game=create<Game>(new GameObject("Game"));
		_game.transform.parent=transform;
	}

	public Game game{ get => _game; }
}

