using System.Collections.Generic;
using UnityEngine.SceneManagement;
/// <summary>
/// 游戏
/// </summary>
public class Game:BaseMonoBehaviour{
	protected override void init(Dictionary<string,object> info) {
		base.init(info);
		gotoTitle();
	}

	public void gotoTitle(){
		SceneManager.LoadScene("Scenes/title",LoadSceneMode.Additive);
		App.instance.soundManager.play();
	}
}