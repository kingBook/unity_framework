using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏类
/// <br>管理游戏全局变量、本地数据、场景切换。</br>
/// <br>不访问关卡内的对象</br>
/// </summary>
public sealed class Game : BaseGame {

    public int levelNumber { get; private set; }

    public void GotoTitleScene () {
        App.instance.sceneLoader.Load("Scenes/Title");
    }

    public void GotoLevelScene (int levelNum) {
        levelNumber = levelNum;

        App.instance.sceneLoader.LoadAsync("Scenes/Level_0");
    }

    /// <summary> 重新开始当前关 </summary>
    public void RestartCurrentLevel (Scene sceneUnload) {
        SceneManager.UnloadSceneAsync(sceneUnload);
        GotoLevelScene(levelNumber);
    }

    /// <summary> 下一关 </summary>
    public void NextLevel (Scene sceneUnload) {
        SceneManager.UnloadSceneAsync(sceneUnload);
        GotoLevelScene(levelNumber + 1);
    }

    private void Start () {
        // 非调试时，才加载其它场景
        if (!App.instance.isDebug) {
            //GotoTitleScene();
            GotoLevelScene(1);
        }
    }

    private void OnDestroy () {

    }


}
