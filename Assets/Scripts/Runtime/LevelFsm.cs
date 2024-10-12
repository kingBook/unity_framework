using System;

/// <summary>
///  <see cref="Level"/> 的有限状态机
/// </summary>
public class LevelFsm : Fsm {

    private void Awake() {
        AddState<StateLevelStart>();
        AddState<StateLevelRunning>();
        AddState<StateLevelVictory>();
        AddState<StateLevelFailure>();
        Init();
        
        ChangeStateTo(nameof(StateLevelStart));
    }
    
}