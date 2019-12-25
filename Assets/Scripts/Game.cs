using System.Collections.Generic;
/// <summary>
/// 游戏类
/// <br>管理游戏全局变量、本地数据、场景切换。</br>
/// <br>不访问关卡内的对象</br>
/// </summary>
public sealed class Game:BaseGame{
	
	protected override void Start() {
		base.Start();
		if(!App.instance.isDebug){
			GotoTitleScene();
		}
	}

	public void GotoTitleScene(){
		App.instance.sceneLoader.Load("Scenes/title");
	}

	public void GotoLevelScene(){
		App.instance.sceneLoader.LoadAsync("Scenes/level");
	}

	protected override void OnDestroy() {
		base.OnDestroy();
	}


}
