/// <summary>
///  <see cref="Level"/> 的有限状态机
/// </summary>
public class FsmLevel : Fsm {

    public Level level { get; private set; }

    public FsmLevel(State defaultState, Level level) {
        this.level = level;
        ChangeStateTo(defaultState);
    }

    public FsmLevel(State defaultState, Level level, System.Action<State, State> onStateChanged) {
        this.level = level;
        m_onStateChangedHandler = onStateChanged;
        ChangeStateTo(defaultState);
    }
}