using System;
using UnityEngine;

/// <summary>
///  <see cref="Game"/> 的有限状态机
/// </summary>
public class GameFsm : Fsm {

    private void Awake() {
        AddState<StateGameTitle>();
        AddState<StateGameLevel>();
        Init();
        ChangeStateTo(nameof(StateGameTitle));
        
    }

}