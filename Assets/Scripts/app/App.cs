
using UnityEngine;
/// <summary>
/// 整个应用程序的单例类
/// </summary>
public class App:BaseApp<App>{
	
	private Game _game;
	
	protected override void init(){
		base.init();
		_game=create<Game>(new GameObject("Game"));
	}

	public Game game{ get => _game; }
}

