using UnityEngine;
using System.Collections;
/// <summary>
/// 关卡类
/// <br>管理关卡内的对象。</br>
/// </summary>
public sealed class Level:BaseMonoBehaviour{

	private Game m_game;

	protected override void Start(){
		base.Start();
		m_game=App.instance.game;
	}

	public void Victory(){
		
	}

	public void Failure(){
		
	}


	protected override void OnDestroy(){
		base.OnDestroy();
	}

}
