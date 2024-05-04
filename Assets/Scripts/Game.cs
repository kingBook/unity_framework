using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏类
/// <br>管理游戏全局变量、本地数据、场景切换。</br>
/// <br>可以通过 <see cref="SetCurrentLevel(Level)"/> 方法设置持有当前关卡的引用，但不访问关卡内的对象</br>
/// </summary>
public sealed class Game : BaseGame {

    public FsmGame fsmGame { get; private set; }
    public int levelNumber { get; private set; }
    public Level currentLevel { get; private set; }
    public int moneyCount => LocalManager.GetMoneyCount();


    public void SetMoneyCount(int value) {
        LocalManager.SetMoneyCount(value);
    }

    public void SetCurrentLevel(Level level) {
        currentLevel = level;
    }

    public void GotoTitleScene() {
        App.instance.sceneLoader.Load("Scenes/Title");
    }

    public void GotoLevelScene(int levelNum) {
        levelNumber = levelNum;

        App.instance.sceneLoader.LoadAsync("Scenes/Level_0");
    }

    /// <summary> 重新开始当前关 </summary>
    public void RestartCurrentLevel(Scene sceneUnload) {
        SceneManager.UnloadSceneAsync(sceneUnload);
        GotoLevelScene(levelNumber);
    }

    /// <summary> 下一关 </summary>
    public void NextLevel(Scene sceneUnload) {
        SceneManager.UnloadSceneAsync(sceneUnload);
        GotoLevelScene(levelNumber + 1);
    }


    private void Start() {
        fsmGame = new FsmGame(StateGameLevel.instance, this);
    }

    private void FixedUpdate() {
        fsmGame.FixedUpdate();
    }

    private void Update() {
        fsmGame.Update();
    }

    private void LateUpdate() {
        fsmGame.LateUpdate();
    }

    private void OnDestroy() {
        fsmGame.OnDestroy();
        currentLevel = null;
    }


}