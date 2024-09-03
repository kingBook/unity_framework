/// <summary>
///  <see cref="Level"/> 的有限状态机
/// </summary>
public class LevelFsm : Fsm {


    public Level level { get; private set; }

    public void Init(Level level, System.Action<State, State> onStateChanged = null) {
        AddState<StateLevelStart>();
        AddState<StateLevelRunning>();
        AddState<StateLevelVictory>();
        AddState<StateLevelFailure>();

        this.level = level;
        base.Init(onStateChanged);
    }
}