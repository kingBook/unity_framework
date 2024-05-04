using UnityEngine;
/// <summary>
///  <see cref="Game"/> 的有限状态机
/// </summary>
public class FsmGame : Fsm {

    public Game game { get; private set; }


    public FsmGame(IState defaultState, Game game) {
        this.game = game;
        ChangeStateTo(defaultState);
    }

    public FsmGame(IState defaultState, Game game, System.Action<IState, IState> onStateChanged) {
        this.game = game;
        m_onStateChangedHandler = onStateChanged;
        ChangeStateTo(defaultState);
    }
}