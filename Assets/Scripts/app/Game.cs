using System.Collections.Generic;
using UnityEngine.SceneManagement;
/// <summary>
/// 游戏
/// </summary>
public class Game:BaseMonoBehaviour{
	protected override void onCreate(Dictionary<string,object> info) {
		base.onCreate(info);
		gotoTitle();
	}

	public void gotoTitle(){
		App.instance.sceneLoader.load("Scenes/title");
		//App.instance.soundManager.play();
	}
}