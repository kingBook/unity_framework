/// <summary>
///  <see cref="Level"/> 的有限状态机
/// </summary>
public class FsmLevel : Fsm {

    public Level level { get; private set; }

    public FsmLevel(IState defaultState, Level level) {
        this.level = level;
        ChangeStateTo(defaultState);
    }

    public FsmLevel(IState defaultState, Level level, System.Action<IState, IState> onStateChanged) {
        this.level = level;
        m_onStateChangedHandler = onStateChanged;
        ChangeStateTo(defaultState);
    }
}