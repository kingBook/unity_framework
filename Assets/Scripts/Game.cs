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

    public StateGameTitle stateGameTitle { get; private set; }
    public StateGameLevel stateGameLevel { get; private set; }

    public FsmGame fsm { get; private set; }
    public Level currentLevel { get; private set; }


    public void SetCurrentLevel(Level level) {
        currentLevel = level;
    }

    private void Awake() {
        stateGameTitle = gameObject.AddComponent<StateGameTitle>();
        stateGameLevel = gameObject.AddComponent<StateGameLevel>();
    }

    private void Start() {
        stateGameLevel.SetLevelNumber(1);
        fsm = new FsmGame(stateGameLevel, this);
    }

    private void FixedUpdate() {
        fsm.FixedUpdate();
    }

    private void Update() {
        fsm.Update();
    }

    private void LateUpdate() {
        fsm.LateUpdate();
    }

    private void OnDestroy() {
        fsm.OnDestroy();
        currentLevel = null;
    }


}