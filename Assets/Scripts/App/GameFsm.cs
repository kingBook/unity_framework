using System;
using UnityEngine;

/// <summary>
///  <see cref="Game"/> 的有限状态机
/// </summary>
public class GameFsm : Fsm {

    public Game game { get; private set; }

    private void Awake() {
        AddState<StateGameTitle>();
        AddState<StateGameLevel>();
        Init();
        ChangeStateTo(nameof(StateGameTitle));
        
        game = App.instance.fsm.GetCurrentState<Game>();
    }

}