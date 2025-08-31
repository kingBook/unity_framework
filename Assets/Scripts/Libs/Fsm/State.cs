using UnityEngine;

/// <summary>
/// 最简单、无任何实现的状态 (继承MonoBehaviour使子类能使用Invoke、StartCoroutine等函数)
/// </summary>
public class State : MonoBehaviour, IState {

    protected virtual void OnStateEnter(Fsm fsm) {
    }

    protected virtual void OnStateFixedUpdate() {
    }

    protected virtual void OnStateUpdate() {
    }

    protected virtual void OnStateLateUpdate() {
    }

    protected virtual void OnStateExit() {
    }

    // ‘显式’方式，实现 IState 接口的方法，又用 protected 进行封装，使用各方法在子类中不公开

    void IState.OnStateEnter(Fsm fsm) {
        OnStateEnter(fsm);
    }

    void IState.OnStateFixedUpdate() {
        OnStateFixedUpdate();
    }

    void IState.OnStateUpdate() {
        OnStateUpdate();
    }

    void IState.OnStateLateUpdate() {
        OnStateLateUpdate();
    }

    void IState.OnStateExit() {
        OnStateExit();
    }
}