using UnityEngine;

/// <summary> 有限状态机 </summary>
public class Fsm : MonoBehaviour {

    /// <summary> 状态发生改变后的回调函数，格式：<code> void OnStateChangedHandler(IState old, IState current) </code> </summary>
    protected System.Action<State, State> m_onStateChangedHandler;

    public State currentState { get; protected set; }
    public StateDefault stateDefault { get; private set; }


    public static T Create<T>(GameObject bind) where T : Fsm {
        var gameObj = new GameObject(typeof(T).Name);
        gameObj.transform.SetParent(bind.transform);
        var fsm = gameObj.AddComponent<T>();
        return fsm;
    }

    public void Init(System.Action<State, State> onStateChanged = null) {
        if (!stateDefault) {
            stateDefault = gameObject.AddComponent<StateDefault>();
        }
        m_onStateChangedHandler = onStateChanged;
        ChangeStateTo(stateDefault);
    }

    public void ChangeStateTo(State state) {
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
    }


}