using UnityEngine;
using System.Collections;
/// <summary>
/// 关卡类
/// <br>管理关卡全局变量、关卡内的UI、关卡胜利/失败/过关/通关。</br>
/// </summary>
public sealed class Level:BaseMonoBehaviour{
	private Game _game;

	protected override void Start() {
		base.Start();
		_game=App.instance.game;
	}

	public void vectory(){
		
	}

	public void failure(){
		

	}


	protected override void OnDestroy() {
		base.OnDestroy();
	}

}
