using UnityEngine;

/// <summary>
///  <see cref="Game"/> 的有限状态机
/// </summary>
public class FsmGame : Fsm {

    public StateGameTitle stateGameTitle { get; private set; }
    public StateGameLevel stateGameLevel { get; private set; }

    public Game game { get; private set; }


    public void Init(Game game, System.Action<State, State> onStateChanged = null) {
        this.game = game;
        stateGameTitle = gameObject.AddComponent<StateGameTitle>();
        stateGameLevel = gameObject.AddComponent<StateGameLevel>();
        base.Init(onStateChanged);
    }
}