using UnityEngine;

/// <summary>
///  <see cref="Game"/> 的有限状态机
/// </summary>
public class FsmGame : Fsm {

    public Game game { get; private set; }


    public FsmGame(State defaultState, Game game) {
        this.game = game;
        ChangeStateTo(defaultState);
    }

    public FsmGame(State defaultState, Game game, System.Action<State, State> onStateChanged) {
        this.game = game;
        m_onStateChangedHandler = onStateChanged;
        ChangeStateTo(defaultState);
    }
}