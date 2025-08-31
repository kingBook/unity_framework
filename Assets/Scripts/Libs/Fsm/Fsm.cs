using System.Collections.Generic;
using UnityEngine;

/// <summary> 有限状态机 </summary>
public class Fsm : MonoBehaviour {

    /// <summary> 存储状态的字典 </summary>
    private Dictionary<string, IState> m_states = new();

    /// <summary> 状态发生改变后的回调函数，格式：<code> void OnStateChanged(State old, State current) </code> </summary>
    protected System.Action<State, State> m_onStateChanged;

    /// <summary> 当前状态 </summary>
    public IState currentState { get; protected set; }

    /// <summary>
    ///  当前状态
    /// </summary>
    /// <typeparam name="T"> 状态类型 </typeparam>
    /// <returns> 返回类型转换后的当前状态 </returns>
    public T GetCurrentState<T>() where T : IState {
        return (T)currentState;
    }

    /// <summary>
    /// 初始化状态机
    /// </summary>
    /// <param name="onStateChanged"> 当前状态已改变时的回调，函数格式为: <c> void OnChanged(State old, State current) </c> </param>
    public void Init(System.Action<State, State> onStateChanged = null) {
        AddState<StateDefault>();
        m_onStateChanged = onStateChanged;
        // 切换到默认状态
        ChangeStateTo(nameof(StateDefault));
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <typeparam name="T"> 状态类型 </typeparam>
    public void AddState<T>() where T : State {
        var state = gameObject.AddComponent<T>();
        m_states.Add(typeof(T).Name, state);
    }

    /// <summary>
    /// 获取状态
    /// </summary>
    /// <typeparam name="T"> 状态类型 </typeparam>
    /// <returns> 以状态类型名称作为 key, 从状态字典里获取状态 </returns>
    public T GetState<T>() where T : State {
        return (T)m_states[typeof(T).Name];
    }

    /// <summary>
    /// 切换到指定状态
    /// </summary>
    /// <param name="stateName"> 目标状态名称，如: <code> nameof(StateTitle) </code> </param>
    /// <param name="onChanged"> 回调函数，格式：<code> void OnChanged(State old, State current) </code> </param>
    public void ChangeStateTo(string stateName, System.Action<State, State> onChanged = null) {
        var state = m_states[stateName];
        if (state == null) {
            Debug.LogError("状态 " + stateName + " 未添加，使用 AddState(stateName) 方法进行添加");
            return;
        }
        //if (currentState == state) return;
        var old = currentState;
        // 状态退出
        old?.OnStateExit();
        // 
        currentState = state;
        // 改变状态时的回调
        m_onStateChanged?.Invoke((State)old, (State)state);
        onChanged?.Invoke((State)old, (State)state);
        // 状态进入
        currentState.OnStateEnter(this);
    }

    private void FixedUpdate() {
        currentState.OnStateFixedUpdate();
    }

    private void Update() {
        currentState.OnStateUpdate();
    }

    private void LateUpdate() {
        currentState.OnStateLateUpdate();
    }

    private void OnDestroy() {
        currentState = null;
        m_onStateChanged = null;
        m_states = null;
    }


}