using System.Collections.Generic;
/// <summary>
/// 游戏类
/// <br>管理游戏全局变量、本地数据、场景切换。</br>
/// <br>不访问关卡内的对象</br>
/// </summary>
public sealed class Game : BaseGame {

    private void Start () {
		// 非调试时，才加载其它场景
        if (!App.instance.isDebug) {
            //GotoTitleScene();
            GotoLevelScene();
        }
    }

    public void GotoTitleScene () {
        App.instance.sceneLoader.Load("Scenes/Title");
    }

    public void GotoLevelScene () {
        App.instance.sceneLoader.LoadAsync("Scenes/Level_0");
    }

    private void OnDestroy () {

    }


}
