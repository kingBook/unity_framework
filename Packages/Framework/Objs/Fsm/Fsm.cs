using UnityEngine;
using UnityEngine.Events;

/// <summary> 有限状态机 </summary>
public class Fsm {

    /// <summary> 状态发生改变后的回调函数，格式：<code> void OnStateChangedHandler(IState old, IState current) </code> </summary>
    private UnityAction<IState, IState> m_onStateChangedHandler;


    public IState currentState { get; protected set; }


    public Fsm(IState defaultState) {
        ChangeStateTo(defaultState);
    }

    public Fsm(IState defaultState, UnityAction<IState, IState> onStateChanged) {
        m_onStateChangedHandler = onStateChanged;
        ChangeStateTo(defaultState);
    }

    public void ChangeStateTo(IState state) {
        if (currentState == state) return;
        var old = currentState;
        // 状态退出
        if (old != null) {
            old.OnStateExit(this);
        }
        // 
        currentState = state;
        // 改变状态时的回调
        m_onStateChangedHandler?.Invoke(old, state);
        // 状态进入
        currentState.OnStateEnter(this);
    }

    public void FixedUpdate() {
        currentState.OnStateFixedUpdate(this);
    }

    public void Update() {
        currentState.OnStateUpdate(this);
    }

    public void LateUpdate() {
        currentState.OnStateLateUpdate(this);
    }

    public void OnDestroy() {
        currentState = null;
        m_onStateChangedHandler = null;
    }


}
