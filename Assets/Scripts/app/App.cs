
using UnityEngine;
/// <summary>
/// 整个应用程序的单例类
/// <br>此类不可以实现以下方法：Awake、Start、FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject。</br>
/// <br>将override以下方法代替：init(代替Awake和Start)、FixedUpdate2、Update2、LateUpdate2、OnGUI2、OnRenderObject2。</br>
/// </summary>
public class App:BaseApp<App>{
	
	private Game _game;
	
	protected override void init(){
		base.init();
		_game=create<Game>(new GameObject("Game"));
	}

	public Game game{ get => _game; }
}

