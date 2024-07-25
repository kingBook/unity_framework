using UnityEngine;

/// <summary>
///  <see cref="Game"/> 的有限状态机
/// </summary>
public class GameFsm : Fsm {

    public Game game { get; private set; }


    public void Init(Game game, System.Action<State, State> onStateChanged = null) {
        this.game = game;
        AddState<StateGameTitle>();
        AddState<StateGameLevel>();
        base.Init(onStateChanged);
    }
}