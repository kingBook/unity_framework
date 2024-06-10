/// <summary>
///  <see cref="Level"/> 的有限状态机
/// </summary>
public class FsmLevel : Fsm {

    public StateLevelStart stateLevelStart { get; private set; }
    public StateLevelRunning stateLevelRunning { get; private set; }
    public StateLevelVictory stateLevelVictory { get; private set; }
    public StateLevelFailure stateLevelFailure { get; private set; }

    public Level level { get; private set; }

    public void Init(Level level, System.Action<State, State> onStateChanged = null) {
        stateLevelStart = gameObject.AddComponent<StateLevelStart>();
        stateLevelRunning = gameObject.AddComponent<StateLevelRunning>();
        stateLevelVictory = gameObject.AddComponent<StateLevelVictory>();
        stateLevelFailure = gameObject.AddComponent<StateLevelFailure>();

        this.level = level;
        base.Init(onStateChanged);
    }
}