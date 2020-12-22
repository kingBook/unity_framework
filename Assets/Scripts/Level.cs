using UnityEngine;
using System.Collections;
/// <summary>
/// 关卡类
/// <br>管理关卡内的对象。</br>
/// </summary>
public class Level:MonoBehaviour{

	protected Game m_game;

	private void Start(){
		m_game=App.instance.GetGame<Game>();
	}

	public void Victory(){
		
	}

	public void Failure(){
		
	}


	private void OnDestroy(){
		
	}

}
