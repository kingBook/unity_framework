using System.Collections.Generic;
using UnityEngine;

/// <summary> 有限状态机 </summary>
public class Fsm : MonoBehaviour {

    private Dictionary<string, IState> m_states = new();

    /// <summary> 状态发生改变后的回调函数，格式：<code> void OnStateChangedHandler(State old, State current) </code> </summary>
    protected System.Action<State, State> m_onStateChangedHandler;

    public IState currentState { get; protected set; }


    public static T Create<T>(GameObject bind) where T : Fsm {
        var gameObj = new GameObject(typeof(T).Name);
        gameObj.transform.SetParent(bind.transform);
        var fsm = gameObj.AddComponent<T>();
        return fsm;
    }

    public void Init(System.Action<State, State> onStateChanged = null) {
        AddState<StateDefault>();
        m_onStateChangedHandler = onStateChanged;
        ChangeStateTo(nameof(StateDefault));
    }

    public void AddState<T>() where T : State {
        var state = gameObject.AddComponent<T>();
        m_states.Add(typeof(T).Name, state);
    }

    public T GetState<T>() where T : State {
        return (T)m_states[typeof(T).Name];
    }

    public void ChangeStateTo(string stateName) {
        var state = m_states[stateName];
        if (currentState == state) return;
        var old = currentState;
        // 状态退出
        if (old != null) {
            old.OnStateExit(this);
        }
        // 
        currentState = state;
        // 改变状态时的回调
        m_onStateChangedHandler?.Invoke((State)old, (State)state);
        // 状态进入
        currentState.OnStateEnter(this);
    }

    private void FixedUpdate() {
        currentState.OnStateFixedUpdate(this);
    }

    private void Update() {
        currentState.OnStateUpdate(this);
    }

    private void LateUpdate() {
        currentState.OnStateLateUpdate(this);
    }

    private void OnDestroy() {
        currentState = null;
        m_onStateChangedHandler = null;
        m_states = null;
    }


}