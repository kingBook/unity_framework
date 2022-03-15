using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Fsm : MonoBehaviour {

    public StateBase defaultState;
    /// <summary> 状态发生改变后事件，回调函数格式：<code> void OnStateChangedHandler(StateBase old, StateBase current) </code> </summary>
    public UnityAction<StateBase, StateBase> onStateChangedEvent;

    protected StateBase m_currentState;


    public StateBase currentState => m_currentState;

    public void SwitchTo (StateBase state) {
        if (m_currentState == state) return;
        var old = m_currentState;
        old.OnStateExit(this);
        m_currentState = state;
        onStateChangedEvent?.Invoke(old, state);
        m_currentState.OnStateEnter(this);
    }

    private void Start () {
        SwitchTo(defaultState);
    }

    private void Update () {
        m_currentState.OnStateUpdate(this);
    }

    private void OnDestroy () {
        m_currentState = null;
    }


}
