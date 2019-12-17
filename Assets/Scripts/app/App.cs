using UnityEngine;
/// <summary>
/// 整个应用程序的单例类
/// <br>此类的以下方法：FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject，</br>
/// <br>将使用以下代替：FixedUpdate2、Update2、LateUpdate2、OnGUI2、OnRenderObject2。</br>
/// </summary>
public sealed class App:BaseApp<App>{
	
	public Game game{ get; private set; }

	protected override void Awake(){
		base.Awake();
		var gameObject=new GameObject("Game");
		game=gameObject.AddComponent<Game>();
		game.transform.parent=transform;
	}
}

